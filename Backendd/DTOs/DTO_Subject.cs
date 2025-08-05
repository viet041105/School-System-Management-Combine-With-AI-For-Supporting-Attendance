using System.ComponentModel.DataAnnotations;

namespace Backendd.DTOs
{
    public class DTO_Subject
    {
        public Guid SbId { get; set; }

        [Required]
        [StringLength(255)]
        public string SbName { get; set; } = null!;

        // Danh sách lớp học (chỉ cần nếu cần)
        public List<Guid> CsClassIds { get; set; } = new List<Guid>();

        // Danh sách lịch học (chỉ cần nếu cần)
        public List<Guid> TblScheduleIds { get; set; } = new List<Guid>();
    }
}
