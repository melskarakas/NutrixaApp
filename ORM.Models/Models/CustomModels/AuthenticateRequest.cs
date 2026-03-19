using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.Models.Models.CustomModels
{
    public class AuthenticateRequest
    {
        [DefaultValue("System")]
        public required string Username { get; set; }

        [DefaultValue("System")]
        public required string Password { get; set; }
        [DefaultValue("System")]
        public required bool IsPasswordMd5 { get; set; }
    }
}
