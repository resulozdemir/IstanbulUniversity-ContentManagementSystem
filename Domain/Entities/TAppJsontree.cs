using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_APP_JSONTREE")]
public partial class TAppJsontree
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("SITEID")]
    public int Siteid { get; set; }

    [Column("TYPE")]
    public int Type { get; set; }

    [Column("NAME")]
    [StringLength(500)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("DATA")]
    public string? Data { get; set; }

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

    [Column("PARENT1")]
    public int Parent1 { get; set; }

    [Column("PARENT2")]
    public int Parent2 { get; set; }

    [Column("PARENT3")]
    public int Parent3 { get; set; }

    [Column("PARENT4")]
    public int Parent4 { get; set; }

    [Column("PARENT5")]
    public int Parent5 { get; set; }

    [Column("PARENT6")]
    public int Parent6 { get; set; }

    [Column("PARENT7")]
    public int Parent7 { get; set; }
}
