using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_APP_TRANSLATE")]
public partial class TAppTranslate
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("THEMECOMPONENTID")]
    public int Themecomponentid { get; set; }

    [Column("CULTURE")]
    [StringLength(10)]
    [Unicode(false)]
    public string Culture { get; set; } = null!;

    [Column("JSON")]
    public string? Json { get; set; }

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
}
