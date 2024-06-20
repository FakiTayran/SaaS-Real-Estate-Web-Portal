using System;
using Microsoft.AspNetCore.Identity;

namespace realEstateManagementEntities.Models
{
    public class AdminUser : IdentityUser
    {
        public required string Name { get; set; }

        public required string Surname { get; set; }

        public byte[]? userPP { get; set; }

        public string? Token { get; set; }

        public int? RealEstateCompanyId { get; set; }

        public RealEstateCompany? RealEstateCompany { get; set; }

    }
}

