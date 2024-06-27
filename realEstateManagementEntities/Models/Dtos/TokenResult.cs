using System;
namespace realEstateManagementEntities.Models.Dtos
{
    public class TokenResult
    {
        public string? name { get; set; }
        public string? surname { get; set; }
        public string? access_token { get; set; }
        public string token_type => "Bearer";
        public int expires_in { get; set; }
        public string? companyName { get; set; }
        public int? companyId { get; set; }
        public byte[]? companyIcon { get; set; }
        public string? userName { get; set; }
        public byte[]? userPP { get; set; }

    }
}

