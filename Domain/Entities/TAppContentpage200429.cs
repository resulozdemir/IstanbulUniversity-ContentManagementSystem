using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Keyless]
[Table("T_APP_CONTENTPAGE200429")]
public partial class TAppContentpage200429
{
    [Column("ID")]
    public int Id { get; set; }

    [Column("SITEID")]
    public int? Siteid { get; set; }

    [Column("GROUPID")]
    public int Groupid { get; set; }

    [Column("HEADER")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Header { get; set; }

    [Column("CONTENT")]
    public string? Content { get; set; }

    [Column("ORDERBY")]
    public int? Orderby { get; set; }

    [Column("LINK")]
    [StringLength(500)]
    [Unicode(false)]
    public string? Link { get; set; }

    [Column("COLUMN8")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column8 { get; set; }

    [Column("COLUMN1")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column1 { get; set; }

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

    [Column("CONTENTDEV")]
    public string? Contentdev { get; set; }

    [Column("CONTENTINNER")]
    public string? Contentinner { get; set; }
}
