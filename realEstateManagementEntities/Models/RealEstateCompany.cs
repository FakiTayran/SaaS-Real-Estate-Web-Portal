using System;
namespace realEstateManagementEntities.Models
{
	public class RealEstateCompany : BaseEntity
	{
		public string Name { get; set; }

        public byte[]? Icon { get; set; }

        public string? TaxNumber { get; set; }

        public virtual ICollection<AdminUser> EstateAgents { get; set; } = new HashSet<AdminUser>();

    }
}

