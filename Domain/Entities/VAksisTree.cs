using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Keyless]
[Table("V_AKSIS_TREE")]
public partial class VAksisTree
{
    [Column("BIRIMKOD")]
    public int Birimkod { get; set; }

    [Column("PARENTID")]
    public int? Parentid { get; set; }

    [Column("AD")]
    [StringLength(600)]
    [Unicode(false)]
    public string Ad { get; set; } = null!;

    [Column("BIRIMTURID")]
    public int? Birimturid { get; set; }

    [Column("BIRIMTUR")]
    [StringLength(250)]
    public string? Birimtur { get; set; }

    [Column("OGRETIMTURID")]
    public int? Ogretimturid { get; set; }

    [Column("OGRETIMTUR")]
    [StringLength(250)]
    public string? Ogretimtur { get; set; }

    [Column("OGRENIMSURE")]
    public int? Ogrenimsure { get; set; }

    [Column("ISDELETED")]
    public int? Isdeleted { get; set; }
}
