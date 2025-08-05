using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backendd.ModelFromDB;

[PrimaryKey("SuScheduleid", "SuUserid")]
[Table("TblScheduleUser")]
public partial class TblScheduleUser
{
    [Key]
    [Column("su_scheduleid")]
    public Guid SuScheduleid { get; set; }

    [Key]
    [Column("su_userid")]
    public Guid SuUserid { get; set; }

    [Column("su_is_arrive")]
    public bool SuIsArrive { get; set; }

    [ForeignKey("SuScheduleid")]
    [InverseProperty("TblScheduleUsers")]
    public virtual TblSchedule SuSchedule { get; set; } = null!;

    [ForeignKey("SuUserid")]
    [InverseProperty("TblScheduleUsers")]
    public virtual TblUser SuUser { get; set; } = null!;
}
