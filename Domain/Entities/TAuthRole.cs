using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_AUTH_ROLE")]
[Index("Id", Name = "T_AUTH_ROLE_PK", IsUnique = true)]
public partial class TAuthRole
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("NAME")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Name { get; set; }

    [Column("COL1")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Col1 { get; set; }

    [Column("COL2")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Col2 { get; set; }

    [Column("CREATEDDATE")]
    [Precision(6)]
    public DateTime? Createddate { get; set; }

    [Column("CREATEDUSER")]
    public int? Createduser { get; set; }

    [Column("MODIFIEDDATE")]
    [Precision(6)]
    public DateTime? Modifieddate { get; set; }

    [Column("MODIFIEDUSER")]
    public int Modifieduser { get; set; }

    [Column("ISDELETED")]
    public int Isdeleted { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<TAuthUserinrole> TAuthUserinroles { get; set; } = new List<TAuthUserinrole>();
}
