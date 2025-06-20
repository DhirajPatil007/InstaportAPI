using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class rider_pointsystem
{
    public int riderpointid { get; set; }

    public string riderid { get; set; } = null!;

    public int? points { get; set; }

    public string? events { get; set; }

    public DateTime datecreated { get; set; }
}
