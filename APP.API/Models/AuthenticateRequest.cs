using System.ComponentModel;

namespace APP.API.Models
{
    public class AuthenticateRequest
    {
        [DefaultValue("System")]
        public required string Email { get; set; }

        [DefaultValue("System")]
        public required string Password { get; set; }
		[DefaultValue("System")]
		public required bool IsPasswordMd5 { get; set; }
	}
}
