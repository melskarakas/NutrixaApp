using ORM.Models.Models;
using System;

namespace APP.API.Models
{
    public class AuthenticateResponse
    {
        public Guid Id { get; set; }
        public string NameSurname { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }


        public AuthenticateResponse(users user, string token)
        {
            Id = user.id;
            NameSurname = user.name_surname;
            Username = user.user_name;
            Token = token;
        }
    }
}
