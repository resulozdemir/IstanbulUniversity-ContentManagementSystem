using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_AUTH_USERINROLE")]
public partial class TAuthUserinrole
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("USERID")]
    public int Userid { get; set; }

    [Column("ROLEID")]
    public int Roleid { get; set; }

    [Column("CREATEDDATE")]
    [Precision(6)]
    public DateTime? Createddate { get; set; }

    [Column("CREATEDUSER")]
    public int? Createduser { get; set; }

    [Column("MODIFIEDDATE")]
    [Precision(6)]
    public DateTime? Modifieddate { get; set; }

    [Column("MODIFIEDUSER")]
    public int? Modifieduser { get; set; }

    [Column("ISDELETED")]
    public int Isdeleted { get; set; }

    [Column("SITEID")]
    public int Siteid { get; set; }

    [ForeignKey("Roleid")]
    [InverseProperty("TAuthUserinroles")]
    public virtual TAuthRole Role { get; set; } = null!;

    [ForeignKey("Userid")]
    [InverseProperty("TAuthUserinroles")]
    public virtual TAuthUser User { get; set; } = null!;
}
