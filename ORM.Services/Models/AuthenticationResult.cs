using System;

namespace ORM.Services.Models
{
    public class AuthenticationResult
    {
        public string Token { get; set; }
        public bool IsRegistration { get; set; }
    }
}
