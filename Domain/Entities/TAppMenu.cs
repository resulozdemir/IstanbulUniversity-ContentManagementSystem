using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_APP_MENU")]
public partial class TAppMenu
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("SITEID")]
    public int Siteid { get; set; }

    [Column("PARENTID")]
    public int? Parentid { get; set; }

    [Column("NAME")]
    [StringLength(250)]
    [Unicode(false)]
    public string? Name { get; set; }

    [Column("LINK")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Link { get; set; }

    [Column("MENUORDER")]
    public int? Menuorder { get; set; }

    [Column("ICON")]
    [StringLength(40)]
    [Unicode(false)]
    public string? Icon { get; set; }

    [Column("STATUS")]
    public int? Status { get; set; }

    [Column("TYPE")]
    public int? Type { get; set; }

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

    [Column("GROUPID")]
    public int Groupid { get; set; }

    [Column("TARGET")]
    [StringLength(40)]
    [Unicode(false)]
    public string? Target { get; set; }

    [InverseProperty("Parent")]
    public virtual ICollection<TAppMenu> InverseParent { get; set; } = new List<TAppMenu>();

    [ForeignKey("Parentid")]
    [InverseProperty("InverseParent")]
    public virtual TAppMenu? Parent { get; set; }

    [ForeignKey("Siteid")]
    [InverseProperty("TAppMenus")]
    public virtual TAppSite Site { get; set; } = null!;
}
