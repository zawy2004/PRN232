using System;
using System.Collections.Generic;

namespace Q1.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? TrainerId { get; set; }

    public DateTime? BookingDate { get; set; }

    public DateTime? SessionTime { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual Trainer? Trainer { get; set; }
}
