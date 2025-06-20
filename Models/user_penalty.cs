using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class user_penalty
{
    public int userpenaltyid { get; set; }

    public string customerid { get; set; } = null!;

    public int? penaltypoints { get; set; }

    public string? events { get; set; }

    public DateTime datecreated { get; set; }
}
