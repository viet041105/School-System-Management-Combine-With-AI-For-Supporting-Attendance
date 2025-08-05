namespace Backendd.DTOs
{
    public class DTO_UpdateStatusArrive
    {
        public Guid user_id { get; set; }
        public Guid schedule_id { get; set; }
        public bool isArrive { get; set; }
    }
}
