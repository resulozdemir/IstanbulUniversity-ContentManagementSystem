using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_AVES_PROFILE")]
public partial class TAvesProfile
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("KIMLIKNO")]
    public int Kimlikno { get; set; }

    [Column("PROFILE")]
    [StringLength(200)]
    [Unicode(false)]
    public string Profile { get; set; } = null!;

    [Column("ISDELETED")]
    public int Isdeleted { get; set; }

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

    [Column("CHECKSTATUS")]
    public int Checkstatus { get; set; }

    [ForeignKey("Kimlikno")]
    [InverseProperty("TAvesProfiles")]
    public virtual TAuthUser KimliknoNavigation { get; set; } = null!;
}
