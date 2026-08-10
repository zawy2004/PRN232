using System;
using System.Collections.Generic;

namespace Q1.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int? ViewerId { get; set; }

    public DateTime? BookingDate { get; set; }

    public DateTime? ShowTime { get; set; }

    public virtual ICollection<TicketDetail> TicketDetails { get; set; } = new List<TicketDetail>();

    public virtual Viewer? Viewer { get; set; }
}
