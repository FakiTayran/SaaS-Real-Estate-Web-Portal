using System;
namespace realEstateManagementEntities.Models.Dtos
{
    public class Pagination<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? TotalRecords { get; set; }
        public int? TotalPages => TotalRecords.HasValue ? (int)Math.Ceiling(TotalRecords.Value / (double)PageSize) : (int?)null;
        public IEnumerable<T> Content { get; set; }
    }
}

