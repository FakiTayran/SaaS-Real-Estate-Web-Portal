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

        public int NumberOfRooms { get; set; }

        public List<byte[]>? EstatePictures { get; set; }

        public required string City { get; set; }

        public required string PostCode { get; set; }

        public required string Description { get; set; }

        public required string Headline { get; set; }

        public PropertyType PropertyType { get; set; }

        public AdminUser? EstateAgent { get; set; }
    }
}

