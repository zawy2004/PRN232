using System;
using System.Collections.Generic;

namespace Q1.Models;

public partial class BookingDetail
{
    public int BookingId { get; set; }

    public int MemberId { get; set; }

    public int? DurationMinutes { get; set; }

    public string? Status { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Member Member { get; set; } = null!;
}
