using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_APP_THEME")]
public partial class TAppTheme
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("NAME")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Name { get; set; }

    [Column("HEADER")]
    public string? Header { get; set; }

    [Column("FOOTER")]
    public string? Footer { get; set; }

    [Column("COLUMN10")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column10 { get; set; }

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

    [InverseProperty("Theme")]
    public virtual ICollection<TAppThemecomponent> TAppThemecomponents { get; set; } = new List<TAppThemecomponent>();
}
