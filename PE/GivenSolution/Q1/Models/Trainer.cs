using System;
using System.Collections.Generic;

namespace Q1.Models;

public partial class Trainer
{
    public int TrainerId { get; set; }

    public string? TrainerName { get; set; }

    public string? Email { get; set; }

    public int? ExperienceYears { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Specialization> Specs { get; set; } = new List<Specialization>();
}
