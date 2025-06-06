using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_APP_SITEDOMAIN")]
public partial class TAppSitedomain
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("SITEID")]
    public int Siteid { get; set; }

    [Column("DOMAIN")]
    [StringLength(250)]
    [Unicode(false)]
    public string Domain { get; set; } = null!;

    [Column("LANGUAGE")]
    [StringLength(20)]
    [Unicode(false)]
    public string Language { get; set; } = null!;

    [Column("KEY")]
    [StringLength(100)]
    [Unicode(false)]
    public string Key { get; set; } = null!;

    [Column("ANALYTICID")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Analyticid { get; set; }

    [Column("GOOGLESITEVERIFICATION")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Googlesiteverification { get; set; }

    [Column("COLUMN1")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column1 { get; set; }

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
    public int Isdeleted { get; set; }

    [ForeignKey("Siteid")]
    [InverseProperty("TAppSitedomains")]
    public virtual TAppSite Site { get; set; } = null!;
}
