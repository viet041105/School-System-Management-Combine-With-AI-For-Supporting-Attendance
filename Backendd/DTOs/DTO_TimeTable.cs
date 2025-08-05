using System;
using Microsoft.EntityFrameworkCore;

namespace Backendd.DTOs
{
    public class TimeTableDto
    {
        public string Class_Name { get; set; }
        public string Class_Location { get; set; }
        public string Subject_Name { get; set; }
        public int Schedule_NumStudent { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Guid ScId { get; set; }
    }
}
