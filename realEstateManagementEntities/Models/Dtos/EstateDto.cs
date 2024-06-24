using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace realEstateManagementEntities.Models.Dtos
{
	public class EstateDto
	{
        public int id { get; set; }

        public EstateType EstateType { get; set; }

        public int NumberOfBedRooms { get; set; }
        public int NumberOfBathRooms { get; set; }
        public int SquareMeter { get; set; }

        public bool Garden { get; set; }
        public bool Balcony { get; set; }

        public List<byte[]>? EstatePictures { get; set; }

        public required string City { get; set; }

        public decimal Price { get; set; }

        public required string PostCode { get; set; }

        public required string Description { get; set; }

        public required string Headline { get; set; }

        public PropertyType PropertyType { get; set; }

        public AdminUser? EstateAgent { get; set; }

        public RealEstateCompany? realEstateCompany { get; set; }

        public string? LandLordName { get; set; }
        public string? LandLordPhone { get; set; }
        public string? LandLordEmail { get; set; }

    }
}

