using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class customer_transactions
{
    public Guid _id { get; set; }

    public decimal amount { get; set; }

    public Guid? customer_id { get; set; }

    public Guid? order_id { get; set; }

    public long? timestamp { get; set; }

    public string? status { get; set; }

    public string? payment_method_type { get; set; }

    public string? type { get; set; }

    public bool? debit { get; set; }

    public bool? wallet { get; set; }

    public DateTime? createdAt { get; set; }

    public DateTime? updatedAt { get; set; }

    public int? __v { get; set; }
}
