using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using realEstateManagementAPI;
using realEstateManagementBusinessLayer.Abstract;
using realEstateManagementDataLayer.EntityFramework;
using realEstateManagementEntities.Models;
using realEstateManagementEntities.Models.Dtos;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace realEstateManagementAuthAPI.Controllers
{
    public class AuthController : Controller
    {

        private readonly UserManager<AdminUser> _userManager;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly RealEstateManagementDbContext _dbContext;
        private readonly IEstateCompanyService _estateCompanyService;

        public AuthController(UserManager<AdminUser> userManager, IOptions<AppSettings> appSettings, RealEstateManagementDbContext dbContext, IEstateCompanyService estateCompanyService)
        {
            _userManager = userManager;
            _appSettings = appSettings;
            _dbContext = dbContext;
            _estateCompanyService = estateCompanyService;
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
                    userPP = user.userPP ?? new byte[0], // Null kontrolü
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

        private async Task AddUserToEstateCompany(string email, RealEstateCompany addedEstate)
        {
            var identityUser = await _userManager.FindByNameAsync(email);
            if (identityUser != null)
            {
                addedEstate.EstateAgents.Add(identityUser);
                await _dbContext.SaveChangesAsync();
            }
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

        [HttpGet("IsAuthorize")]
        [Authorize]
        public IActionResult IsAuthorize()
        {
            return Ok();
        }
    }
}

