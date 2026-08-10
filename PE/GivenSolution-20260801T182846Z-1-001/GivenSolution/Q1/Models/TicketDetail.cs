using System;
using System.Collections.Generic;

namespace Q1.Models;

public partial class TicketDetail
{
    public int TicketId { get; set; }

    public int MovieId { get; set; }

    public string? SeatNumber { get; set; }

    public int? Quantity { get; set; }

    public virtual Movie Movie { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;
}
