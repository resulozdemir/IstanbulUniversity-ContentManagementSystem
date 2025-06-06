using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Keyless]
[Table("V_APP_SEARCH")]
public partial class VAppSearch
{
    [Column("ID")]
    public int? Id { get; set; }

    [Column("SITEID")]
    public int? Siteid { get; set; }

    [Column("ROUTING")]
    [StringLength(208)]
    [Unicode(false)]
    public string? Routing { get; set; }

    [Column("TEXT")]
    public string? Text { get; set; }

    [Column("HEADER")]
    [StringLength(500)]
    [Unicode(false)]
    public string? Header { get; set; }

    [Column("CONTENT")]
    public string? Content { get; set; }

    [Column("ONDATE")]
    [Precision(6)]
    public DateTime? Ondate { get; set; }
}
