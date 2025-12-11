using System;
using System.Collections.Generic;

namespace FluxOS.Models;

public partial class News
{
    public int NewsId { get; set; }

    public string Title { get; set; } = null!;

    public string Summary { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public string Category { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }
}
