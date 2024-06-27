using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using realEstateManagementAPI.UtilityHelper;
using realEstateManagementBusinessLayer.Abstract;
using realEstateManagementDataLayer.EntityFramework;
using realEstateManagementEntities.Models;
using realEstateManagementEntities.Models.Dtos;

namespace realEstateManagementAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AdminUser> _userManager;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly RealEstateManagementDbContext _dbContext;
        private readonly IEstateCompanyService _estateCompanyService;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<AdminUser> userManager, IOptions<AppSettings> appSettings, RealEstateManagementDbContext dbContext, IEstateCompanyService estateCompanyService,IConfiguration configuration)
        {
            _userManager = userManager;
            _appSettings = appSettings;
            _dbContext = dbContext;
            _estateCompanyService = estateCompanyService;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.Users
                    .Include(u => u.RealEstateCompany)
                    .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                var authClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                var userClaims = await _userManager.GetClaimsAsync(user);

                authClaims.AddRange(userClaims);

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Value.JwtSecret));

                var token = new JwtSecurityToken(
                    issuer: _appSettings.Value.JwtIssuer,
                    audience: _appSettings.Value.JwtIssuer,
                    expires: DateTime.Now.AddHours(1),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                var stringToken = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new TokenResult
                {
                    name = user.Name,
                    surname = user.Surname,
                    access_token = stringToken,
                    companyName = user.RealEstateCompany?.Name ?? string.Empty, // Null kontrolü
                    userName = user.UserName,
                    userPP = user.userPP?? new byte[0], // Null kontrolü
                    companyId = user.RealEstateCompanyId ?? 0, // Null kontrolü
                    companyIcon = user.RealEstateCompany?.Icon ?? new byte[0], // Null kontrolü
                    expires_in = 1
                });
            }

            return BadRequest();
            
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (HasNullValues(dto, out string errorMessage))
            {
                return Ok(new GeneralResponse<string>
                {
                    Result = errorMessage
                });
            }

            var estateCompany = new RealEstateCompany
            {
                Name = dto.CompanyName,
                TaxNumber = dto.TaxNumber
            };

            var addedEstate = await _estateCompanyService.AddEstateCompany(estateCompany);

            var userCreationResult = await CreateUser(dto, addedEstate);
            if (userCreationResult.Succeeded)
            {
                await AddUserToEstateCompany(dto.Email, addedEstate);
                return Ok(new GeneralResponse<string>
                {
                    Result = "User created successfully",
                    IsError = false
                });
            }

            return BadRequest(new GeneralResponse<string>
            {
                Result = "User was not created",
                IsError = true
            });
        }

        private bool HasNullValues(RegisterDto dto, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password) || string.IsNullOrEmpty(dto.ConfirmPassword))
            {
                errorMessage = "Null values are not allowed";
                return true;
            }

            if (dto.Password != dto.ConfirmPassword)
            {
                errorMessage = "Passwords are not the same";
                return true;
            }

            return false;
        }

       

        private async Task<RealEstateCompany> GetEstateCompanyById(int estateCompanyId)
        {
            return await _estateCompanyService.GetEstateCompanyById(estateCompanyId);
        }

        private async Task<IdentityResult> CreateUser(RegisterDto dto, RealEstateCompany addedEstate)
        {
            var user = new AdminUser
            {
                Name = dto.Name,
                Surname = dto.Surname,
                UserName = dto.Email,
                Email = dto.Email,
                RealEstateCompany = addedEstate
            };

            return await _userManager.CreateAsync(user, dto.Password);
        }

        private async Task AddUserToEstateCompany(string email, RealEstateCompany addedEstate)
        {
            var identityUser = await _userManager.FindByNameAsync(email);
            if (identityUser != null)
            {
                addedEstate.EstateAgents.Add(identityUser);
                await _dbContext.SaveChangesAsync();
            }
        }

        [HttpGet("GetAllAgents/{estateCompanyId}")]
        public IActionResult GetAllAgents(int estateCompanyId)
        {
            var users = _userManager.Users
                .Where(u => u.RealEstateCompanyId == estateCompanyId) // Assuming you want to filter by estateAgentId
                .Select(u => new
                {
                    Id = u.Id,
                    Email = u.Email,
                    UserName = u.UserName,
                    Name = u.Name,
                    Surname = u.Surname
                })
                .ToList();

            return Ok(users);
        }


        [HttpGet("IsAuthorize")]
        [Authorize]
        public IActionResult IsAuthorize()
        {
            return Ok();
        }


        [Authorize]
        [HttpPost("AddEstateAgent")]
        public async Task<IActionResult> AddEstateAgent(AddEstateAgentDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Name))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Result = "Email and Name are required",
                    IsError = true
                });
            }

            var estateCompany = await GetEstateCompanyById(dto.EstateCompanyId);
            if (estateCompany == null)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Result = "Invalid EstateCompanyId",
                    IsError = true
                });
            }

            var randomPassword = GenerateRandomPassword();

            var user = new AdminUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = dto.Name,
                Surname = dto.Surname,
                RealEstateCompanyId = dto.EstateCompanyId
            };

            var result = await _userManager.CreateAsync(user, randomPassword);

            if (result.Succeeded)
            {
                SendPasswordToEmail(user.Email, randomPassword);
                return Ok(new GeneralResponse<string>
                {
                    Result = "Estate agent added successfully",
                    IsError = false
                });
            }

            return BadRequest(new GeneralResponse<string>
            {
                Result = "Error adding estate agent",
                IsError = true
            });
        }

        private string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var password = new char[length];
            for (int i = 0; i < 10; i++)
            {
                password[i] = validChars[random.Next(validChars.Length)];
            }
            password[10] = '.';
            password[11] = 'L';



            return new string(password);
        }

        private void SendPasswordToEmail(string email, string password)
        {

            try
            {
                var mailHelper = new MailHelper(_configuration);
                var mailModel = new MailModelDto
                {
                    Subject = "Your new account password",
                    To = email,
                    Body = $"Your portal password is: {password}",
                    MailPriority = MailPriority.Normal
                };
                mailHelper.MailSender(mailModel.Subject, mailModel.To, mailModel.Body, mailModel.MailPriority);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            

        }
        [Authorize]
        [HttpDelete("DeleteEstateAgent/{id}")]
        public async Task<IActionResult> DeleteEstateAgent(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new GeneralResponse<string>
                {
                    Result = "Estate agent not found",
                    IsError = true
                });
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Ok(new GeneralResponse<string>
                {
                    Result = "Estate agent deleted successfully",
                    IsError = false
                });
            }

            return BadRequest(new GeneralResponse<string>
            {
                Result = "Error deleting estate agent",
                IsError = true
            });
        }

        [Authorize]
        [HttpPost("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                return NotFound(new GeneralResponse<string>
                {
                    Result = "User not found",
                    IsError = true
                });
            }

            user.Name = dto.Name;
            user.Surname = dto.Surname;
            user.Email = dto.Email;
            user.UserName = dto.Email;

            if (dto.ProfilePicture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await dto.ProfilePicture.CopyToAsync(memoryStream);
                    user.userPP = memoryStream.ToArray();
                }
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    user.Name,
                    user.Surname,
                    Email = user.Email,
                    UserPP = user.userPP != null ? Convert.ToBase64String(user.userPP) : null
                });
            }

            return BadRequest(new GeneralResponse<string>
            {
                Result = "Error updating profile",
                IsError = true
            });
        }


        [Authorize]
        [HttpPost("UpdateEstateCompany")]
        public async Task<IActionResult> UpdateEstateCompany([FromForm] UpdateCompanySettingsDto dto)
        {
            var company = await _estateCompanyService.GetEstateCompanyById(dto.id);
            if (company == null)
            {
                return NotFound(new GeneralResponse<string>
                {
                    Result = "Company not found",
                    IsError = true
                });
            }

            company.Name = dto.CompanyName;

            if (dto.CompanyIcon != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await dto.CompanyIcon.CopyToAsync(memoryStream);
                    company.Icon = memoryStream.ToArray();
                }
            }

            await _estateCompanyService.UpdateEstateCompany(company);

            return Ok(new
            {
                companyName = company.Name,
                icon = company.Icon != null ? company.Icon : null
            });
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new GeneralResponse<string>
                {
                    Result = "User not found",
                    IsError = true
                });
            }

            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new GeneralResponse<string>
                {
                    Result = "Password changed successfully",
                    IsError = false
                });
            }

            return BadRequest(new GeneralResponse<string>
            {
                Result = "Error changing password",
                IsError = true
            });
        }




    }
}
