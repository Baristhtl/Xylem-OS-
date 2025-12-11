using System;
using System.Collections.Generic;

namespace FluxOS.Models;

public partial class Topic
{
    public int TopicId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public int? ViewCount { get; set; }

    public bool? IsSolved { get; set; }

    public int? UserId { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Reply> Replies { get; set; } = new List<Reply>();

    public virtual User? User { get; set; }
}
