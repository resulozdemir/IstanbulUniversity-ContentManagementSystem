using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_APP_UPLOADFILE")]
[Index("Id", Name = "T_APP_UPLOADFILE_PK", IsUnique = true)]
public partial class TAppUploadfile
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("USERID")]
    public int Userid { get; set; }

    [Column("SITEID")]
    public int Siteid { get; set; }

    [Column("FILEID")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Fileid { get; set; }

    [Column("SALT")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Salt { get; set; }

    [Column("PATH")]
    [StringLength(500)]
    [Unicode(false)]
    public string? Path { get; set; }

    [Column("TYPE")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Type { get; set; }

    [Column("FILESIZE", TypeName = "numeric(18, 0)")]
    public decimal? Filesize { get; set; }

    [Column("FILENAME")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Filename { get; set; }

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
    public bool Isdeleted { get; set; }

    [ForeignKey("Siteid")]
    [InverseProperty("TAppUploadfiles")]
    public virtual TAppSite Site { get; set; } = null!;

    [ForeignKey("Userid")]
    [InverseProperty("TAppUploadfiles")]
    public virtual TAuthUser User { get; set; } = null!;
}
