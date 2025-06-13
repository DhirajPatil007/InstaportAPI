using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class orders
{
    public Guid _id { get; set; }

    public string? delivery_type { get; set; }

    public string? parcel_weight { get; set; }

    public string? phone_number { get; set; }

    public bool? notify_sms { get; set; }

    public bool? courier_bag { get; set; }

    public string? vehicle { get; set; }

    public string? status { get; set; }

    public string? payment_method { get; set; }

    public string? package { get; set; }

    public long? time_stamp { get; set; }

    public decimal? parcel_value { get; set; }

    public decimal? amount { get; set; }

    public decimal? commission { get; set; }

    public string? reason { get; set; }

    public long? timer { get; set; }

    public DateTime? createdAt { get; set; }

    public DateTime? updatedAt { get; set; }

    public int? __v { get; set; }

    public Guid? customer { get; set; }

    public Guid? rider { get; set; }

    public string? orderId { get; set; }

    public virtual ICollection<order_addresses> order_addresses { get; set; } = new List<order_addresses>();
}
