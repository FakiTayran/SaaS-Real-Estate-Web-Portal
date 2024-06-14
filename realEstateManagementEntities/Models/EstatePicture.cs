using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace realEstateManagementEntities.Models
{
    public class EstatePicture : BaseEntity
    {
        public required byte[] img { get; set; }

        [ForeignKey("Estate")]
        public int EstateId { get; set; }
        public virtual required Estate Estate { get; set; }
    }
}
