using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Keyless]
[Table("TEMP_EVENT")]
public partial class TempEvent
{
    [Column("ID")]
    public int Id { get; set; }

    [Column("HEADER")]
    [StringLength(500)]
    [Unicode(false)]
    public string Header { get; set; } = null!;

    [Column("CONTENT")]
    public string? Content { get; set; }

    [Column("LINK")]
    [StringLength(500)]
    [Unicode(false)]
    public string? Link { get; set; }

    [Column("ONDATE")]
    [Precision(6)]
    public DateTime Ondate { get; set; }

    [Column("IMG")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Img { get; set; }

    [Column("TAG")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? Tag { get; set; }

    [Column("GALLERY")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? Gallery { get; set; }

    [Column("COLUMN9")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Column9 { get; set; }

    [Column("SITEID")]
    public int Siteid { get; set; }

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

    [Column("SUMMARY")]
    [StringLength(400)]
    [Unicode(false)]
    public string? Summary { get; set; }

    [Column("MAP")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Map { get; set; }

    [Column("ISPUBLIC")]
    public int Ispublic { get; set; }

    [Column("AUTHOR")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Author { get; set; }

    [Column("ADDRESS")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Address { get; set; }

    [Column("CONTENTINNER")]
    public string? Contentinner { get; set; }
}
