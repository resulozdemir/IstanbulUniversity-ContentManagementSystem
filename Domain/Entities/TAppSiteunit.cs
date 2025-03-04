using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Keyless]
[Table("T_APP_SITEUNIT")]
public partial class TAppSiteunit
{
    [Column("ID")]
    public int Id { get; set; }

    [Column("SITEID")]
    public int Siteid { get; set; }

    [Column("TYPE")]
    public int Type { get; set; }

    [Column("UNITID")]
    public int Unitid { get; set; }

    [Column("COLUMN1")]
    public int? Column1 { get; set; }

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
