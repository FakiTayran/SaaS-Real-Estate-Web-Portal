using System;
using Microsoft.AspNetCore.Http;

namespace realEstateManagementEntities.Models.Dtos
{
    public class UpdateProfileDto
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
}

