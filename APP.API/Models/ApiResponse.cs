namespace APP.API.Models
{
    public class ApiResponse<T>
    {
        public int status_code { get; set; }
        public string message { get; set; } = string.Empty;
        public T? data { get; set; }
    }
}
