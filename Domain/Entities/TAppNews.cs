using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_APP_NEWS")]
public partial class TAppNews
{
    [Key]
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
    [StringLength(4000)]
    [Unicode(false)]
    public string? Img { get; set; }

    [Column("TAG")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? Tag { get; set; }

    [Column("INSLIDER")]
    public int Inslider { get; set; }

    [Column("TITLE1")]
    [StringLength(500)]
    [Unicode(false)]
    public string? Title1 { get; set; }

    [Column("TITLE2")]
    [StringLength(500)]
    [Unicode(false)]
    public string? Title2 { get; set; }

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

    [Column("TITLE3")]
    [StringLength(500)]
    [Unicode(false)]
    public string? Title3 { get; set; }

    [Column("GALLERY")]
    [StringLength(4000)]
    [Unicode(false)]
    public string? Gallery { get; set; }

    [Column("SITEID")]
    public int Siteid { get; set; }

    [Column("ISPUBLIC")]
    public int Ispublic { get; set; }

    [Column("AUTHOR")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Author { get; set; }

    [Column("CONTENTINNER")]
    public string? Contentinner { get; set; }

    [Column("PRIORITYORDER")]
    public int? Priorityorder { get; set; }

    [Column("ISPUBLISH")]
    public int Ispublish { get; set; }

    [Column("AMFIID")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Amfiid { get; set; }

    [Column("ISAMFI")]
    public int Isamfi { get; set; }

    [Column("TRANSACTION")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Transaction { get; set; }

    [Column("ISBLANK")]
    public int Isblank { get; set; }

    [ForeignKey("Siteid")]
    [InverseProperty("TAppNews")]
    public virtual TAppSite Site { get; set; } = null!;
}
