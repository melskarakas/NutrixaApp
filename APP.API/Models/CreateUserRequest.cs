namespace APP.API.Models
{
    public class CreateUserRequest
    {
        public required string Mail { get; set; }
        public required string Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        
    }
}