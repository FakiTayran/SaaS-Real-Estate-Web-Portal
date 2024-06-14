using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using realEstateManagementAPI.UtilityHelper;
using realEstateManagementBusinessLayer.Abstract;
using realEstateManagementBusinessLayer.Concrete.Spesification;
using realEstateManagementDataLayer.EntityFramework;
using realEstateManagementEntities.Models;
using realEstateManagementEntities.Models.Dtos;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace realEstateManagementAPI.Controllers
{

    public class HomeController : Controller
    {
        private readonly IEstateService _estateService;
        private readonly RealEstateManagementDbContext? _context;
        private readonly IConfiguration _configuration;


        public HomeController(IConfiguration configuration,IEstateService estateService, RealEstateManagementDbContext realEstateManagementDbContext)
        {
            _estateService = estateService;
            _context = realEstateManagementDbContext;
            _configuration = configuration;
        }


        [HttpGet("GetAllEstates")]
        public async Task<IActionResult> GetAllEstates(EstateType? estateType = null, PropertyType? propertyType = null, int? numberOfRooms = null, string? city = null, string? postCode = null, string? searchText = null, string? adminUserId = null)
        {
            var spec = new EstateFilterSpesification(estateType, propertyType, numberOfRooms, city, postCode, searchText, adminUserId);
            List<EstateDto> estateDtos = await _estateService.ListEstatesAsync(spec); 
            
            return Ok(estateDtos);
        }

        [HttpPost("MailSender")]
        public IActionResult MailSender([FromBody] MailModelDto model)
        {
            try
            {
                var mailHelper = new MailHelper(_configuration);
                mailHelper.MailSender(model.Subject, model.To, model.Body, model.MailPriority);

                return Ok("E-mail sended successfuully.Check your mails include junk mails.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occured when trying to send e-mail: {ex.Message}");
            }
        }

    }
}

