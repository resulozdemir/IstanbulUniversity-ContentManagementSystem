using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_APP_UPLOADAPP")]
public partial class TAppUploadapp
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("KEY")]
    [StringLength(10)]
    [Unicode(false)]
    public string Key { get; set; } = null!;

    [Column("NAME")]
    [StringLength(100)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("PATH")]
    [StringLength(500)]
    [Unicode(false)]
    public string Path { get; set; } = null!;

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

    [Column("PASSWORD")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Password { get; set; }

    [Column("MAXWIDTH")]
    public int Maxwidth { get; set; }

    [Column("MAXHEIGHT")]
    public int Maxheight { get; set; }

    [Column("THUMBNAILSIZE")]
    public int Thumbnailsize { get; set; }

    [Column("FILEPATH")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Filepath { get; set; }
}
