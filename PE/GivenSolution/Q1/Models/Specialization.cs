using System;
using System.Collections.Generic;

namespace Q1.Models;

public partial class Specialization
{
    public int SpecId { get; set; }

    public string? SpecName { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();
}
