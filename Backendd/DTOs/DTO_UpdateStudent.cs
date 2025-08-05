using System.ComponentModel.DataAnnotations.Schema;

namespace Backendd.DTOs
{
    [NotMapped]
    public class UpdateAttendanceDto
    {
        public Guid ScheduleId { get; set; }
        public List<AttendanceRecord> AttendanceRecords { get; set; }
    }

    [NotMapped]
    public class AttendanceRecord
    {
        public Guid UsId { get; set; }
        public bool IsArrive { get; set; }
    }
}
