using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class user_pointsystem
{
    public int userpointid { get; set; }

    public string customerid { get; set; } = null!;

    public int? points { get; set; }

    public string? events { get; set; }

    public DateTime datecreated { get; set; }
}
