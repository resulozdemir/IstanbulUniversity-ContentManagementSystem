using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Table("T_SPC_DEPLOY")]
public partial class TSpcDeploy
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("FILENAME")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Filename { get; set; }

    [Column("TITLE")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? Title { get; set; }

    [Column("CREATOR")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? Creator { get; set; }

    [Column("SUBJECT")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? Subject { get; set; }

    [Column("DESCRIPTION")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? Description { get; set; }

    [Column("PUBLISHER")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? Publisher { get; set; }

    [Column("CONTRIBUTOR")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? Contributor { get; set; }

    [Column("DATES")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? Dates { get; set; }

    [Column("TYPES")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? Types { get; set; }

    [Column("FORMAT")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? Format { get; set; }

    [Column("IDENTIFIER")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? Identifier { get; set; }

    [Column("LANGUAGE")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? Language { get; set; }

    [Column("ABSTRACT")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? Abstract { get; set; }

    [Column("DSPACEIDENTIFIER")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? Dspaceidentifier { get; set; }

    [Column("STATUS")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? Status { get; set; }

    [Column("ATTEMPTCOUNT")]
    public int Attemptcount { get; set; }

    [Column("COLUMN1")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column1 { get; set; }

    [Column("COLUMN2")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column2 { get; set; }

    [Column("COLUMN3")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column3 { get; set; }

    [Column("COLUMN4")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column4 { get; set; }

    [Column("COLUMN5")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column5 { get; set; }

    [Column("COLUMN6")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column6 { get; set; }

    [Column("COLUMN7")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Column7 { get; set; }

    [Column("ISDELETED")]
    public int Isdeleted { get; set; }

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
}
