namespace Backendd.DTOs
{
    public class DTO_Schedule
    {
        public Guid ScId { get; set; }
        public Guid SubjectId { get; set; }

        public string? SubjectName { get; set; } = null!;
        public DateTime ScStarttime { get; set; }
        public DateTime ScEndtime { get; set; }
        public int ScNumstudent { get; set; }
        public string? ClassName { get; set; } = null!;
        public Guid ClassId { get; set; }
    }
}
