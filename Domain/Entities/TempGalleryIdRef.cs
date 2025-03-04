using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Domain.Entities;

[Keyless]
[Table("TEMP_GALLERY_ID_REF")]
public partial class TempGalleryIdRef
{
    [Column("ID")]
    public int Id { get; set; }

    [Column("URL")]
    [StringLength(4000)]
    [Unicode(false)]
    public string Url { get; set; } = null!;

    [Column("SITE_ID")]
    public int? SiteId { get; set; }

    [Column("CREATEDDATE")]
    [Precision(6)]
    public DateTime? Createddate { get; set; }

    [Column("CREATEDUSER")]
    public int? Createduser { get; set; }

    [Column("ISDELETED")]
    public int Isdeleted { get; set; }
}
