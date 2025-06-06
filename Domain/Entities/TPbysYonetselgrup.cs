using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_PBYS_YONETSELGRUP")]
public partial class TPbysYonetselgrup
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("BIRIMID")]
    public int? Birimid { get; set; }

    [Column("YONETSELGOREVID")]
    public int Yonetselgorevid { get; set; }

    [Column("GRUP")]
    [StringLength(100)]
    [Unicode(false)]
    public string Grup { get; set; } = null!;

    [Column("ISDELETED")]
    public int Isdeleted { get; set; }

    [Column("SIRA")]
    public int? Sira { get; set; }

    [Column("GRUP_EN")]
    [StringLength(100)]
    [Unicode(false)]
    public string GrupEn { get; set; } = null!;

    [Column("GRUP_AR")]
    [StringLength(100)]
    [Unicode(false)]
    public string? GrupAr { get; set; }

    [ForeignKey("Birimid")]
    [InverseProperty("TPbysYonetselgrups")]
    public virtual TPbysTree? Birim { get; set; }
}
