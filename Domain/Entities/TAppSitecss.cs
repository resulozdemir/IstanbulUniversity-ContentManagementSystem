using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_APP_SITECSS")]
public partial class TAppSitecss
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("CSS")]
    public string? Css { get; set; }

    [Column("SITEID")]
    public int Siteid { get; set; }

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
}
