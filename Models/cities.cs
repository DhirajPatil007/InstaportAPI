using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class cities
{
    public Guid _id { get; set; }

    public string city_name { get; set; } = null!;

    public string slug { get; set; } = null!;

    public DateTime? createdAt { get; set; }

    public DateTime? updatedAt { get; set; }

    public int? __v { get; set; }
}
