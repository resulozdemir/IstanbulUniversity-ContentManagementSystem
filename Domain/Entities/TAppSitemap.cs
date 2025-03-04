using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_APP_SITEMAP")]
public partial class TAppSitemap
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("URL")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? Url { get; set; }

    [Column("DOMAIN")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Domain { get; set; }

    [Column("LANG")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Lang { get; set; }

    [Column("SITEID")]
    public int? Siteid { get; set; }

    [Column("ITEMID")]
    public int? Itemid { get; set; }

    [Column("COLUMN1")]
    public int? Column1 { get; set; }

    [Column("COLUMN2")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Column2 { get; set; }

    [Column("REDIRECTTO")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? Redirectto { get; set; }

    [Column("ACTIVE")]
    public int Active { get; set; }

    [Column("CREATEDUSER")]
    public int? Createduser { get; set; }

    [Column("CREATEDDATE")]
    [Precision(6)]
    public DateTime? Createddate { get; set; }

    [Column("MODIFIEDUSER")]
    public int? Modifieduser { get; set; }

    [Column("MODIFIEDDATE")]
    [Precision(6)]
    public DateTime? Modifieddate { get; set; }

    [Column("ISDELETED")]
    public int Isdeleted { get; set; }
}
