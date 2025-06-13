using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class rider_transactions
{
    public Guid _id { get; set; }

    public decimal amount { get; set; }

    public long timestamp { get; set; }

    public string message { get; set; } = null!;

    public Guid rider { get; set; }

    public string? transactionID { get; set; }

    public bool completed { get; set; }

    public bool request { get; set; }

    public bool debit { get; set; }

    public Guid? order_id { get; set; }

    public int? __v { get; set; }
}
