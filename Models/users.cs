using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class users
{
    public Guid _id { get; set; }

    public string fullname { get; set; } = null!;

    public string mobileno { get; set; } = null!;

    public string password { get; set; } = null!;

    public string usecase { get; set; } = null!;

    public bool verified { get; set; }

    public string role { get; set; } = null!;

    public decimal? wallet { get; set; }

    public decimal? holdAmount { get; set; }

    public string? image { get; set; }

    public DateTime createdAt { get; set; }

    public DateTime updatedAt { get; set; }

    public int? __v { get; set; }

    public string? fcmtoken { get; set; }

    public string? token { get; set; }

    public virtual ICollection<customer_addresses> customer_addresses { get; set; } = new List<customer_addresses>();
}
