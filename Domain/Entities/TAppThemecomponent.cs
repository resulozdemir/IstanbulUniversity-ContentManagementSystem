using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_APP_THEMECOMPONENT")]
public partial class TAppThemecomponent
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("THEMEID")]
    public int? Themeid { get; set; }

    [Column("COMPONENTID")]
    public int Componentid { get; set; }

    [Column("TEMPLATE")]
    public string? Template { get; set; }

    [Column("STYLE")]
    public string? Style { get; set; }

    [Column("JAVASCRIPT")]
    public string? Javascript { get; set; }

    [Column("NAME")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Name { get; set; }

    [Column("DESCRIPTION")]
    [StringLength(4000)]
    [Unicode(false)]
    public string? Description { get; set; }

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

    [Column("FORMJSON")]
    public string? Formjson { get; set; }

    [Column("FORMHTML")]
    public string? Formhtml { get; set; }

    [Column("FORMJS")]
    public string? Formjs { get; set; }

    [InverseProperty("Themecomponent")]
    public virtual ICollection<TAppSitecomponentdata> TAppSitecomponentdata { get; set; } = new List<TAppSitecomponentdata>();

    [ForeignKey("Themeid")]
    [InverseProperty("TAppThemecomponents")]
    public virtual TAppTheme? Theme { get; set; }
}
