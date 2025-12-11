using System;
using System.Collections.Generic;

namespace FluxOS.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Role { get; set; }

    public string? AvatarColor { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<Reply> Replies { get; set; } = new List<Reply>();

    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
