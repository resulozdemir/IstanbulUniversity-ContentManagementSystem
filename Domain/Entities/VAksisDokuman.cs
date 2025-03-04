using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Keyless]
[Table("V_AKSIS_DOKUMAN")]
public partial class VAksisDokuman
{
    [Column("KIMLIKNO")]
    [StringLength(22)]
    [Unicode(false)]
    public string? Kimlikno { get; set; }

    [Column("TARIH")]
    public DateOnly Tarih { get; set; }

    [Column("ID")]
    [StringLength(100)]
    public string? Id { get; set; }

    [Column("DOSYAADI")]
    [StringLength(300)]
    public string? Dosyaadi { get; set; }

    [Column("ACIKLAMA")]
    [StringLength(300)]
    public string Aciklama { get; set; } = null!;

    [Column("ISDELETED")]
    public int? Isdeleted { get; set; }
}
