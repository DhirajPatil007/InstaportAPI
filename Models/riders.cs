using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class riders
{
    public Guid _id { get; set; }

    public string fullname { get; set; } = null!;

    public string mobileno { get; set; } = null!;

    public string password { get; set; } = null!;

    public string role { get; set; } = null!;

    public bool verified { get; set; }

    public string? status { get; set; }

    public decimal? wallet_amount { get; set; }

    public decimal? holdAmount { get; set; }

    public string? token { get; set; }

    public string? reason { get; set; }

    public bool? applied { get; set; }

    public bool? approve { get; set; }

    public string? orders { get; set; }

    public bool? isDue { get; set; }

    public decimal? requestedAmount { get; set; }

    public string? reference_contact_1 { get; set; }

    public string? fcmtoken { get; set; }

    public int? age { get; set; }

    public long? timestamp { get; set; }

    public string? IPID { get; set; }

    public DateTime? createdAt { get; set; }

    public DateTime? updatedAt { get; set; }

    public int? __v { get; set; }

    public virtual ICollection<rider_documents> rider_documents { get; set; } = new List<rider_documents>();
}
