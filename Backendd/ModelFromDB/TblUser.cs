using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backendd.ModelFromDB;

[Table("TblUser")]
[Index("UsUsername", Name = "UQ__TblUser__07AF65FAF6AD6B69", IsUnique = true)]
[Index("UsEmail", Name = "UQ__TblUser__B28CDEB7B7993734", IsUnique = true)]
public partial class TblUser
{
    [Key]
    [Column("us_id")]
    public Guid UsId { get; set; }

    [Column("us_email")]
    [StringLength(255)]
    public string UsEmail { get; set; } = null!;

    [Column("us_username")]
    [StringLength(100)]
    public string UsUsername { get; set; } = null!;

    [Column("us_password")]
    [StringLength(255)]
    public string UsPassword { get; set; } = null!;

    [Column("us_role")]
    [StringLength(50)]
    public string UsRole { get; set; } = null!;

    [InverseProperty("LogCommiter")]
    public virtual ICollection<TblLog> TblLogs { get; set; } = new List<TblLog>();

    [InverseProperty("SuUser")]
    public virtual ICollection<TblScheduleUser> TblScheduleUsers { get; set; } = new List<TblScheduleUser>();

    [ForeignKey("CuUserId")]
    [InverseProperty("CuUsers")]
    public virtual ICollection<TblClass> CuClasses { get; set; } = new List<TblClass>();
}
