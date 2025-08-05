using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backendd.ModelFromDB;

[Table("TblJWTBlacklist")]
public partial class TblJwtblacklist
{
    [Key]
    [Column("jwt_id")]
    public Guid JwtId { get; set; }

    [Column("jwt_string")]
    [StringLength(1000)]
    public string JwtString { get; set; } = null!;

    [Column("jwt_listed_time", TypeName = "datetime")]
    public DateTime JwtListedTime { get; set; }

    [Column("jwt_expired_time", TypeName = "datetime")]
    public DateTime JwtExpiredTime { get; set; }
}
