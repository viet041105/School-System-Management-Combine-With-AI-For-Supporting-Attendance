using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backendd.ModelFromDB;

[Table("TblResponse")]
public partial class TblResponse
{
    [Key]
    [Column("res_id")]
    public Guid ResId { get; set; }

    [Column("res_arrive_time", TypeName = "datetime")]
    public DateTime ResArriveTime { get; set; }

    [Column("res_detail")]
    [StringLength(1000)]
    public string ResDetail { get; set; } = null!;

    [Column("res_cameraid")]
    public Guid ResCameraid { get; set; }

    [ForeignKey("ResCameraid")]
    [InverseProperty("TblResponses")]
    public virtual TblCamera ResCamera { get; set; } = null!;
}
