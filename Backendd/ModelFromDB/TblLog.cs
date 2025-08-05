using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backendd.ModelFromDB;

[Table("TblLog")]
public partial class TblLog
{
    [Key]
    [Column("log_id")]
    public int LogId { get; set; }

    [Column("log_commiter_id")]
    public Guid LogCommiterId { get; set; }

    [Column("log_phone")]
    [StringLength(20)]
    public string LogPhone { get; set; } = null!;

    [Column("log_details")]
    [StringLength(1000)]
    public string LogDetails { get; set; } = null!;

    [ForeignKey("LogCommiterId")]
    [InverseProperty("TblLogs")]
    public virtual TblUser LogCommiter { get; set; } = null!;
}
