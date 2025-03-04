using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_APP_SNAPSHOT_OLD")]
[Index("Urlhash", Name = "IX_SNAPSHOT_", IsUnique = true)]
public partial class TAppSnapshotOld
{
    [Key]
    [Column("URLHASH")]
    [StringLength(50)]
    [Unicode(false)]
    public string Urlhash { get; set; } = null!;

    [Column("URL")]
    [StringLength(2000)]
    [Unicode(false)]
    public string Url { get; set; } = null!;

    [Column("HTML")]
    public string? Html { get; set; }

    [Column("COLUMN0")]
    public int? Column0 { get; set; }

    [Column("COLUMN6")]
    public int? Column6 { get; set; }

    [Column("COLUMN7")]
    public int? Column7 { get; set; }

    [Column("COLUMN1")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column1 { get; set; }

    [Column("COLUMN2")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column2 { get; set; }

    [Column("COLUMN3")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column3 { get; set; }

    [Column("COLUMN4")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column4 { get; set; }

    [Column("COLUMN5")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column5 { get; set; }

    [Column("EXPIREDATE")]
    [Precision(6)]
    public DateTime? Expiredate { get; set; }

    [Column("CREATEDDATE")]
    [Precision(6)]
    public DateTime? Createddate { get; set; }

    [Column("ISDELETED")]
    public int Isdeleted { get; set; }
}
