using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class order_past_riders
{
    public Guid? order_id { get; set; }

    public Guid? rider_id { get; set; }

    public virtual orders? order { get; set; }
}
