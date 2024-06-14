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
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using realEstateManagementBusinessLayer.Abstract;
using realEstateManagementBusinessLayer.Concrete.Spesification;
using realEstateManagementDataLayer.EntityFramework;
using realEstateManagementEntities.Models;
using realEstateManagementEntities.Models.Dtos;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace realEstateManagementAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AdminUser> _userManager;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly RealEstateManagementDbContext _dbContext;

        public AccountController(UserManager<AdminUser> userManager, IOptions<AppSettings> appSettings, RealEstateManagementDbContext dbContext)
        {
            _userManager = userManager;
            _appSettings = appSettings;
            _dbContext = dbContext;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(dto.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                var authClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub,user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                };
                var userClaims = await _userManager.GetClaimsAsync(user);

                authClaims.AddRange(userClaims);

                var authSigningKey = new SymmetricSecurityKey(
                     Encoding.UTF8.GetBytes(_appSettings.Value.JwtSecret));

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
                    access_token = stringToken,
                    expires_in = 1
                });
            }

            return Ok(new GeneralResponse<string>
            {
                Result = "User name or password is invalid",
                IsError = true,
                Code = 1
            });
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password) || string.IsNullOrEmpty(dto.ConfirmPassword))
            {
                return Ok(new GeneralResponse<string>
                {
                    Result = "Null values are not allowed"
                });
            }
            else if (dto.Password != dto.ConfirmPassword)
            {
                return Ok(new GeneralResponse<string>
                {
                    Result = "Passwords are not same"
                });
            }

            var user = new AdminUser()
            {
                Name = dto.Name,
                Surname = dto.Surname,
                UserName = dto.Email,
                Email = dto.Email,
            };

            IdentityResult result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                //var identityUser = await _userManager.FindByNameAsync(dto.Email);
                //var resultIdentity = await _userManager.AddClaimAsync(identityUser, new Claim(dto.ClaimName, dto.ClaimName.ToLower()));
                //if (resultIdentity.Succeeded)
                //{
                return Ok(new GeneralResponse<string>
                {
                    Result = "User created successfully",
                    IsError = false
                });
                //}

            }


            return BadRequest(new GeneralResponse<string>
            {
                Result = "User was not created",
                IsError = true
            });
        }


        [HttpGet("GetAllAgents")]
        public IActionResult GetAllAgents()
        {
                var users = _userManager.Users
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
    }
}

