using System;
using System.Collections.Generic;

namespace new_cms.Domain.Entities;

public partial class TAppSitecomponentdata
{
    public int Id { get; set; }

    public int Siteid { get; set; }

    public int Themecomponentid { get; set; }

    public string? Data { get; set; }

    public string? Column1 { get; set; }

    public string? Column2 { get; set; }

    public string? Column3 { get; set; }

    public string? Column4 { get; set; }

    public DateTime? Createddate { get; set; }

    public int? Createduser { get; set; }

    public DateTime? Modifieddate { get; set; }

    public int? Modifieduser { get; set; }

    public int Isdeleted { get; set; }

    public virtual TAppSite Site { get; set; } = null!;

    public virtual TAppThemecomponent Themecomponent { get; set; } = null!;
}
