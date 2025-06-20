using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class rider_penalty
{
    public int riderpenaltyid { get; set; }

    public string riderid { get; set; } = null!;

    public int? penaltypoints { get; set; }

    public string? events { get; set; }

    public DateTime datecreated { get; set; }
}
