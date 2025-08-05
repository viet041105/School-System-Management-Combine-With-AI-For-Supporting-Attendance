using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backendd.ModelFromDB;

[Table("TblSchedule")]
public partial class TblSchedule
{
    [Key]
    [Column("sc_id")]
    public Guid ScId { get; set; }

    [Column("sc_subjectid")]
    public Guid ScSubjectid { get; set; }

    [Column("sc_starttime", TypeName = "datetime")]
    public DateTime ScStarttime { get; set; }

    [Column("sc_endtime", TypeName = "datetime")]
    public DateTime ScEndtime { get; set; }

    [Column("sc_numstudent")]
    public int ScNumstudent { get; set; }

    [ForeignKey("ScSubjectid")]
    [InverseProperty("TblSchedules")]
    public virtual TblSubject ScSubject { get; set; } = null!;

    [InverseProperty("SuSchedule")]
    public virtual ICollection<TblScheduleUser> TblScheduleUsers { get; set; } = new List<TblScheduleUser>();



    // Mối quan hệ với TblClass
    [ForeignKey("ScClassId")]
    [InverseProperty("TblSchedules")]
    public virtual TblClass Class { get; set; } = null!;
    // Thêm cột ScClassId vào bảng này
    [Column("sc_classid")]
    public Guid ScClassId { get; set; }
}
