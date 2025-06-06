using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Keyless]
[Table("TEMP_TRANSFER")]
public partial class TempTransfer
{
    [Column("TYPE")]
    [StringLength(20)]
    [Unicode(false)]
    public string Type { get; set; } = null!;

    [Column("WORDPRESSID")]
    public int Wordpressid { get; set; }

    [Column("CMSID")]
    public int Cmsid { get; set; }

    [Column("ROUTING")]
    [StringLength(100)]
    [Unicode(false)]
    public string Routing { get; set; } = null!;

    [Column("SITEID")]
    public int Siteid { get; set; }
}
