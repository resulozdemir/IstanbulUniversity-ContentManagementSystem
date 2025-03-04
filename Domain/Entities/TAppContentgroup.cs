using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_APP_CONTENTGROUP")]
[Index("Id", Name = "T_APP_CONTENTGROUP_PK", IsUnique = true)]
[Index("Isdeleted", "Siteid", "Routing", Name = "T_APP_CONTENTGROUP_UK1", IsUnique = true)]
public partial class TAppContentgroup
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("SITEID")]
    public int? Siteid { get; set; }

    [Column("NAME")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Name { get; set; }

    [Column("VIEWCOUNT")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Viewcount { get; set; }

    [Column("TABTHEME")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Tabtheme { get; set; }

    [Column("ROUTING")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Routing { get; set; }

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

    [ForeignKey("Siteid")]
    [InverseProperty("TAppContentgroups")]
    public virtual TAppSite? Site { get; set; }

    [InverseProperty("Group")]
    public virtual ICollection<TAppContentpage> TAppContentpages { get; set; } = new List<TAppContentpage>();
}
