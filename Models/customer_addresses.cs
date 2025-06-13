using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class customer_addresses
{
    public Guid customer_address_id { get; set; }

    public Guid? customer_id { get; set; }

    public string? type { get; set; }

    public string? text { get; set; }

    public double? latitude { get; set; }

    public double? longitude { get; set; }

    public string? key { get; set; }

    public string? building_and_flat { get; set; }

    public string? floor_and_wing { get; set; }

    public string? instructions { get; set; }

    public string? phone_number { get; set; }

    public string? address { get; set; }

    public string? name { get; set; }

    public bool? is_primary { get; set; }

    public DateTime createdAt { get; set; }

    public DateTime updatedAt { get; set; }

    public DateTime lastUsed { get; set; }

    public virtual users? customer { get; set; }
}
