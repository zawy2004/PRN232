using System;
using System.Collections.Generic;

namespace Q1.Models;

public partial class Viewer
{
    public int ViewerId { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public int? Age { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
