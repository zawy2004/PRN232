using System;
using System.Collections.Generic;

namespace Q1.Models;

public partial class Movie
{
    public int MovieId { get; set; }

    public string? Title { get; set; }

    public int? Duration { get; set; }

    public decimal? BasePrice { get; set; }

    public virtual ICollection<TicketDetail> TicketDetails { get; set; } = new List<TicketDetail>();

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
