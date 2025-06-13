using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace InstaportApi.Models;

public partial class order_addresses
{
    public Guid order_address_id { get; set; }

    public Guid? order_id { get; set; }

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

    public string? date { get; set; }

    public string? totime { get; set; }

    public string? fromtime { get; set; }

    public double? distance { get; set; }

    [JsonIgnore]
    public virtual orders? order { get; set; }
}
