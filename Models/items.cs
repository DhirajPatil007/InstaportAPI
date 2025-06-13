using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class items
{
    public Guid _id { get; set; }

    public string name { get; set; } = null!;

    public string? description { get; set; }

    public DateTime? date { get; set; }

    public int? __v { get; set; }
}
