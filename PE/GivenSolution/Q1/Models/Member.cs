using System;
using System.Collections.Generic;

namespace Q1.Models;

public partial class Member
{
    public int MemberId { get; set; }

    public string? FullName { get; set; }

    public int? PackageId { get; set; }

    public DateOnly? JoinDate { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual MembershipPackage? Package { get; set; }
}
