using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class order_status_updates
{
    public Guid? order_id { get; set; }

    public long? timestamp { get; set; }

    public string? message { get; set; }

    public string? image { get; set; }

    public string? key { get; set; }

    public virtual orders? order { get; set; }
}
