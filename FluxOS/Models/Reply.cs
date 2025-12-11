using System;
using System.Collections.Generic;

namespace FluxOS.Models;

public partial class Reply
{
    public int ReplyId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public int? TopicId { get; set; }

    public int? UserId { get; set; }

    public virtual Topic? Topic { get; set; }

    public virtual User? User { get; set; }
}
