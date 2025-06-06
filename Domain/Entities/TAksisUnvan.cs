using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_AKSIS_UNVAN")]
public partial class TAksisUnvan
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("UNVAN")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Unvan { get; set; }

    [Column("SIRA")]
    public int? Sira { get; set; }

    [Column("UNVAN_EN")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UnvanEn { get; set; }

    [Column("UNVAN_AR")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UnvanAr { get; set; }
}
