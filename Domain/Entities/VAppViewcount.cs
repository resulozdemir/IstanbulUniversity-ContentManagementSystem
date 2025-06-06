using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Keyless]
[Table("V_APP_VIEWCOUNT")]
public partial class VAppViewcount
{
    [Column("NAME")]
    [StringLength(130)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("VALUE")]
    public int? Value { get; set; }
}
