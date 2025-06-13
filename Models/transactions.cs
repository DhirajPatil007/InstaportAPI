using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class transactions
{
    public Guid _id { get; set; }

    public Guid userId { get; set; }

    public decimal amount { get; set; }

    public string type { get; set; } = null!;

    public string? razorpayPaymentId { get; set; }

    public DateTime createdAt { get; set; }

    public int? __v { get; set; }
}
