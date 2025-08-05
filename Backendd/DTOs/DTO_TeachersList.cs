namespace Backendd.DTOs
{
    public class DTO_TeachersList
    {

        public Guid UsId { get; set; }

        public string UsEmail { get; set; } = null!;

        public string UsUsername { get; set; } = null!;

        public string UsPassword { get; set; } = null!;

        public string UsRole { get; set; } = null!;
    }
}
