using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_APP_UPLOAD")]
public partial class TAppUpload
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("USERID")]
    public int Userid { get; set; }

    [Column("SITEID")]
    public int Siteid { get; set; }

    [Column("IMAGEID")]
    [StringLength(50)]
    [Unicode(false)]
    public string Imageid { get; set; } = null!;

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

    [ForeignKey("Siteid")]
    [InverseProperty("TAppUploads")]
    public virtual TAppSite Site { get; set; } = null!;

    [ForeignKey("Userid")]
    [InverseProperty("TAppUploads")]
    public virtual TAuthUser User { get; set; } = null!;
}
