using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class coupons
{
    public Guid _id { get; set; }

    public string code { get; set; } = null!;

    public long timestamp { get; set; }

    public bool disabled { get; set; }

    public int percentOff { get; set; }

    public decimal? maxAmount { get; set; }

    public decimal? minimumCartValue { get; set; }

    public DateTime? createdAt { get; set; }

    public DateTime? updatedAt { get; set; }

    public int? __v { get; set; }
}
