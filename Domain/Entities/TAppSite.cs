using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_APP_SITE")]
public partial class TAppSite
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("NAME")]
    [StringLength(500)]
    [Unicode(false)]
    public string? Name { get; set; }

    [Column("ISTEMPLATE")]
    public int Istemplate { get; set; }

    [Column("DOMAIN")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Domain { get; set; }

    [Column("TEMPLATEID")]
    public int? Templateid { get; set; }

    [Column("THEMEID")]
    public int Themeid { get; set; }

    [Column("KEY")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Key { get; set; }

    [Column("LANGUAGE")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Language { get; set; }

    [Column("ANALYTICID")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Analyticid { get; set; }

    [Column("GOOGLESITEVERIFICATION")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Googlesiteverification { get; set; }

    [Column("ISPUBLISH")]
    public int Ispublish { get; set; }

    [Column("COLUMN11")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column11 { get; set; }

    [Column("WPADRESS")]
    [StringLength(400)]
    [Unicode(false)]
    public string? Wpadress { get; set; }

    [Column("COLUMN13")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column13 { get; set; }

    [Column("COLUMN14")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column14 { get; set; }

    [Column("COLUMN15")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column15 { get; set; }

    [Column("COLUMN16")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column16 { get; set; }

    [Column("COLUMN17")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column17 { get; set; }

    [Column("COLUMN18")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column18 { get; set; }

    [Column("COLUMN19")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column19 { get; set; }

    [Column("COLUMN20")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column20 { get; set; }

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

    [Column("PBYSID")]
    public int? Pbysid { get; set; }

    [Column("SUBUNITPBYS")]
    public int Subunitpbys { get; set; }

    [Column("AKSISID")]
    public int? Aksisid { get; set; }

    [Column("UNIID")]
    public int Uniid { get; set; }

    [InverseProperty("Site")]
    public virtual ICollection<TAppContentgroup> TAppContentgroups { get; set; } = new List<TAppContentgroup>();

    [InverseProperty("Site")]
    public virtual ICollection<TAppContentpage> TAppContentpages { get; set; } = new List<TAppContentpage>();

    [InverseProperty("Site")]
    public virtual ICollection<TAppEvent> TAppEvents { get; set; } = new List<TAppEvent>();

    [InverseProperty("Site")]
    public virtual ICollection<TAppMenu> TAppMenus { get; set; } = new List<TAppMenu>();

    [InverseProperty("Site")]
    public virtual ICollection<TAppNews> TAppNews { get; set; } = new List<TAppNews>();

    [InverseProperty("Site")]
    public virtual ICollection<TAppProjectNews> TAppProjectNews { get; set; } = new List<TAppProjectNews>();

    [InverseProperty("Site")]
    public virtual ICollection<TAppSitecomponentdata> TAppSitecomponentdata { get; set; } = new List<TAppSitecomponentdata>();

    [InverseProperty("Site")]
    public virtual ICollection<TAppSitedomain> TAppSitedomains { get; set; } = new List<TAppSitedomain>();

    [InverseProperty("Site")]
    public virtual ICollection<TAppSitepage> TAppSitepages { get; set; } = new List<TAppSitepage>();

    [InverseProperty("Site")]
    public virtual ICollection<TAppUploadfile> TAppUploadfiles { get; set; } = new List<TAppUploadfile>();

    [InverseProperty("Site")]
    public virtual ICollection<TAppUpload> TAppUploads { get; set; } = new List<TAppUpload>();
}
