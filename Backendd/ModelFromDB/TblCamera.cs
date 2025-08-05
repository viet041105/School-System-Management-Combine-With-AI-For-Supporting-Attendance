using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backendd.ModelFromDB;

[Table("TblCamera")]
public partial class TblCamera
{
    [Key]
    [Column("cam_id")]
    public Guid CamId { get; set; }

    [Column("cam_http_url")]
    [StringLength(500)]
    public string CamHttpUrl { get; set; } = null!;

    [Column("cam_activate_day")]
    public DateOnly CamActivateDay { get; set; }

    [Column("cam_classid")]
    public Guid CamClassid { get; set; }

    [StringLength(500)]
    public string? CamStreamKey { get; set; }

    [ForeignKey("CamClassid")]
    [InverseProperty("TblCameras")]
    public virtual TblClass CamClass { get; set; } = null!;

    [InverseProperty("ResCamera")]
    public virtual ICollection<TblResponse> TblResponses { get; set; } = new List<TblResponse>();
}
