using System;
namespace realEstateManagementEntities.Models.Dtos
{
    public class TokenResult
    {
        public string access_token { get; set; }
        public string token_type => "Bearer";
        public int expires_in { get; set; }
    }
}

