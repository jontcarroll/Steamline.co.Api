using System;

namespace Steamline.co.Api.V1.Services.Options
{
    public class JwtOptions
    {
        public string Issuer {get; set; } 
        public string Audience { get; set; }
        public int ExpirationInMinutes { get; set; }
        public string SigningKey { get; set; }
    }
}