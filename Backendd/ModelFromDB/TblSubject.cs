using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backendd.ModelFromDB;

[Table("TblSubject")]
public partial class TblSubject
{
    [Key]
    [Column("sb_id")]
    public Guid SbId { get; set; }

    [Column("sb_name")]
    [StringLength(255)]
    public string SbName { get; set; } = null!;

    [InverseProperty("ScSubject")]
    public virtual ICollection<TblSchedule> TblSchedules { get; set; } = new List<TblSchedule>();

    [ForeignKey("CsSubjectId")]
    [InverseProperty("CsSubjects")]
    public virtual ICollection<TblClass> CsClasses { get; set; } = new List<TblClass>();
}
