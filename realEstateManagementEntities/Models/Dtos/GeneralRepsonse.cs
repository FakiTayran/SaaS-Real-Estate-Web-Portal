using System;
namespace realEstateManagementEntities.Models.Dtos
{
    public class GeneralResponse<T>
    {
        public T? Result { get; set; }
        public int Code { get; set; }
        public bool IsError { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

