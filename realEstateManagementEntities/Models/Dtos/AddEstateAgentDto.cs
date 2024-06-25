using System;
namespace realEstateManagementEntities.Models.Dtos
{
    public class AddEstateAgentDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public int EstateCompanyId { get; set; }
    }

}

