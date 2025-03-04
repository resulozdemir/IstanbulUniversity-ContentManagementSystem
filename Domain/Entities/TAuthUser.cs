using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_AUTH_USER")]
[Index("Email", Name = "T_U_CONS_EMAIL", IsUnique = true)]
[Index("Kimlikno", Name = "T_U_CONS_KIMLIK_NO", IsUnique = true)]
public partial class TAuthUser
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("KIMLIKNO")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Kimlikno { get; set; }

    [Column("NAME")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Name { get; set; }

    [Column("SURNAME")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Surname { get; set; }

    [Column("EMAIL")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Email { get; set; }

    [Column("PHONE")]
    [StringLength(15)]
    [Unicode(false)]
    public string? Phone { get; set; }

    [Column("WEB")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Web { get; set; }

    [Column("BIRTHDATE")]
    public DateOnly? Birthdate { get; set; }

    [Column("PASSWORD")]
    [StringLength(500)]
    [Unicode(false)]
    public string? Password { get; set; }

    [Column("PASSWORDMODIFIEDDATE")]
    public DateOnly? Passwordmodifieddate { get; set; }

    [Column("COL2")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Col2 { get; set; }

    [Column("COL3")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Col3 { get; set; }

    [Column("COL4")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Col4 { get; set; }

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

    [InverseProperty("User")]
    public virtual ICollection<TAppUploadfile> TAppUploadfiles { get; set; } = new List<TAppUploadfile>();

    [InverseProperty("User")]
    public virtual ICollection<TAppUpload> TAppUploads { get; set; } = new List<TAppUpload>();

    [InverseProperty("User")]
    public virtual ICollection<TAuthLogin> TAuthLogins { get; set; } = new List<TAuthLogin>();

    [InverseProperty("User")]
    public virtual ICollection<TAuthUserinrole> TAuthUserinroles { get; set; } = new List<TAuthUserinrole>();

    [InverseProperty("KimliknoNavigation")]
    public virtual ICollection<TAvesProfile> TAvesProfiles { get; set; } = new List<TAvesProfile>();
}
