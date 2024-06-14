using System;
using System.ComponentModel.DataAnnotations;

namespace realEstateManagementEntities.Models.Dtos
{
    public class RegisterDto
    {

        public required string Name { get; set; }

        public required string Surname { get; set; }

        public required string Email { get; set; }

   
        [MinLength(6)]
        public required string Password { get; set; }

      
        [MinLength(6)]
        public required string ConfirmPassword { get; set; }

    }
}

