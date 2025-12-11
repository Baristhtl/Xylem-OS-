using System;
using System.Collections.Generic;

namespace FluxOS.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
