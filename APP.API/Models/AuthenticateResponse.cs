using ORM.Models.Models;
using System;

namespace APP.API.Models
{
    public class AuthenticateResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }


        public AuthenticateResponse(users user, string token)
        {
            Id = user.id;
            FirstName = user.first_name;
            LastName = user.last_name;
            Token = token;
        }
    }
}
