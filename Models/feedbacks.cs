using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class feedbacks
{
    public Guid _id { get; set; }

    public Guid? rider_id { get; set; }

    public string? rider_name { get; set; }

    public Guid? customer_id { get; set; }

    public string? customer_name { get; set; }

    public Guid? order_id { get; set; }

    public int? rating { get; set; }

    public string? comments { get; set; }

    public string? feedback_type { get; set; }

    public DateTime? createdAt { get; set; }

    public int? __v { get; set; }
}
