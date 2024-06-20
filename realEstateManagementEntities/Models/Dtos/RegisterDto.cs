using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace realEstateManagementEntities.Models.Dtos
{
    public class RegisterDto
    {
        public int?    EstateCompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? TaxNumber { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}

