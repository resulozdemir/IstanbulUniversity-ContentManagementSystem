using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_PBYS_TREE")]
public partial class TPbysTree
{
    [Key]
    [Column("BIRIMKOD")]
    public int Birimkod { get; set; }

    [Column("BOLUMAD")]
    [StringLength(500)]
    [Unicode(false)]
    public string? Bolumad { get; set; }

    [Column("ORGKODU")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Orgkodu { get; set; }

    [Column("DUZEY1")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Duzey1 { get; set; }

    [Column("DUZEY2")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Duzey2 { get; set; }

    [Column("DUZEY3")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Duzey3 { get; set; }

    [Column("DUZEY4")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Duzey4 { get; set; }

    [Column("PARENT_ID")]
    public int? ParentId { get; set; }

    [Column("COLUMN2")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column2 { get; set; }

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
    [StringLength(20)]
    [Unicode(false)]
    public string Isdeleted { get; set; } = null!;

    [InverseProperty("Birim")]
    public virtual ICollection<TPbysYonetselgrup> TPbysYonetselgrups { get; set; } = new List<TPbysYonetselgrup>();
}
