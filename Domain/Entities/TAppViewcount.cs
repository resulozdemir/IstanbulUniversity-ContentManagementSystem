using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Keyless]
[Table("T_APP_VIEWCOUNT")]
public partial class TAppViewcount
{
    [Column("KEY")]
    [StringLength(130)]
    [Unicode(false)]
    public string Key { get; set; } = null!;

    [Column("COUNT")]
    public int Count { get; set; }
}
