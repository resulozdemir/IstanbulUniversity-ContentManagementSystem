using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_AUTH_LOGIN")]
public partial class TAuthLogin
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("USERID")]
    public int Userid { get; set; }

    [Column("TOKEN")]
    [StringLength(250)]
    [Unicode(false)]
    public string? Token { get; set; }

    [Column("TOKENEXPIREAT")]
    [Precision(6)]
    public DateTime? Tokenexpireat { get; set; }

    [Column("STATUS")]
    public int? Status { get; set; }

    [Column("CREATEDDATE")]
    [Precision(6)]
    public DateTime? Createddate { get; set; }

    [Column("ISDELETED")]
    public int Isdeleted { get; set; }

    [ForeignKey("Userid")]
    [InverseProperty("TAuthLogins")]
    public virtual TAuthUser User { get; set; } = null!;
}
