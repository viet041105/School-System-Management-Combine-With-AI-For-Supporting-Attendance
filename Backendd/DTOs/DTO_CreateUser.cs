namespace Backendd.DTOs
{
    public class DTO_CreateUser
    {
        public string UsEmail { get; set; } = null!;
        public string UsUsername { get; set; } = null!;
        public string UsPassword { get; set; } = null!;
        public string UsRole { get; set; } = null!;
    }
}
