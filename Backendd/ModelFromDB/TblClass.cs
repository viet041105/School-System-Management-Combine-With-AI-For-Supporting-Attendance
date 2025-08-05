using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backendd.ModelFromDB;
[Table("TblClass")]
public partial class TblClass
{
    [Key]
    [Column("cl_id")]
    public Guid ClId { get; set; }

    [Column("cl_location")]
    [StringLength(255)]
    public string ClLocation { get; set; } = null!;

    [Column("cl_name")]
    [StringLength(255)]
    public string ClName { get; set; } = null!;

    [InverseProperty("CamClass")]
    public virtual ICollection<TblCamera> TblCameras { get; set; } = new List<TblCamera>();

    [ForeignKey("CsClassId")]
    [InverseProperty("CsClasses")]
    public virtual ICollection<TblSubject> CsSubjects { get; set; } = new List<TblSubject>();

    [ForeignKey("CuClassId")]
    [InverseProperty("CuClasses")]
    public virtual ICollection<TblUser> CuUsers { get; set; } = new List<TblUser>();



    // Thêm mối quan hệ với TblSchedule
    [InverseProperty("Class")]
    public virtual ICollection<TblSchedule> TblSchedules { get; set; } = new List<TblSchedule>();
}
