using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_APP_SITEPAGE")]
public partial class TAppSitepage
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("SITEID")]
    public int Siteid { get; set; }

    [Column("NAME")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Name { get; set; }

    [Column("TEMPLATEID")]
    public int? Templateid { get; set; }

    [Column("ISDEFAULT")]
    public int Isdefault { get; set; }

    [Column("HTML")]
    public string? Html { get; set; }

    [Column("STYLE")]
    public string? Style { get; set; }

    [Column("JAVASCRIPT")]
    public string? Javascript { get; set; }

    [Column("ROUTING")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Routing { get; set; }

    [Column("VIRTUALPAGE")]
    public int Virtualpage { get; set; }

    [Column("COLUMN3")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column3 { get; set; }

    [Column("COLUMN4")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column4 { get; set; }

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

    [Column("HTMLDEV")]
    public string? Htmldev { get; set; }

    [Column("READONLY")]
    public int Readonly { get; set; }

    [Column("STYLEDEV")]
    public string? Styledev { get; set; }

    [Column("JAVASCRIPTDEV")]
    public string? Javascriptdev { get; set; }

    [ForeignKey("Siteid")]
    [InverseProperty("TAppSitepages")]
    public virtual TAppSite Site { get; set; } = null!;
}
