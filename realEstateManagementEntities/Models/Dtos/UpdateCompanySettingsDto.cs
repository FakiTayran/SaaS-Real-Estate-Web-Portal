using System;
using Microsoft.AspNetCore.Http;

namespace realEstateManagementEntities.Models.Dtos
{
    public class UpdateCompanySettingsDto
    {
        public int id { get; set; }
        public string? CompanyName { get; set; }
        public IFormFile? CompanyIcon { get; set; }
    }
}

