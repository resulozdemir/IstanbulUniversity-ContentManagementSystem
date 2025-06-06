using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Keyless]
[Table("V_AKSIS_DERS")]
public partial class VAksisDer
{
    [Column("KIMLIKNO")]
    public int? Kimlikno { get; set; }

    [Column("AD")]
    [StringLength(100)]
    public string? Ad { get; set; }

    [Column("UNVAN")]
    [StringLength(100)]
    public string? Unvan { get; set; }

    [Column("UNVAN_EN")]
    [StringLength(100)]
    public string? UnvanEn { get; set; }

    [Column("SOYAD")]
    [StringLength(100)]
    public string? Soyad { get; set; }

    [Column("DERSID")]
    public int Dersid { get; set; }

    [Column("DERSKOD")]
    [StringLength(100)]
    public string Derskod { get; set; } = null!;

    [Column("DERS")]
    [StringLength(500)]
    public string Ders { get; set; } = null!;

    [Column("DERS_EN")]
    [StringLength(600)]
    public string? DersEn { get; set; }

    [Column("AKTS")]
    public double Akts { get; set; }

    [Column("OGRETIMTIP")]
    [StringLength(28)]
    public string Ogretimtip { get; set; } = null!;

    [Column("OGRETIMTIP_EN")]
    [StringLength(36)]
    public string OgretimtipEn { get; set; } = null!;

    [Column("BIRIMID")]
    public int? Birimid { get; set; }

    [Column("BIRIM")]
    [StringLength(600)]
    public string Birim { get; set; } = null!;

    [Column("BIRIM_EN")]
    [StringLength(600)]
    public string? BirimEn { get; set; }

    [Column("PROGRAMTUR")]
    [StringLength(14)]
    public string? Programtur { get; set; }

    [Column("PROGRAMTUR_EN")]
    [StringLength(13)]
    public string? ProgramturEn { get; set; }

    [Column("PROGRAMTURORDER")]
    public int? Programturorder { get; set; }

    [Column("DONEM")]
    [StringLength(8)]
    public string? Donem { get; set; }

    [Column("DONEM_EN")]
    [StringLength(6)]
    public string? DonemEn { get; set; }
}
