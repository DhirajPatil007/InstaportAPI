using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class price_manipulations
{
    public Guid _id { get; set; }

    public decimal per_kilometer_charge { get; set; }

    public decimal additional_per_kilometer_charge { get; set; }

    public decimal additional_pickup_charge { get; set; }

    public decimal security_fees_charges { get; set; }

    public decimal base_order_charges { get; set; }

    public decimal instaport_commission { get; set; }

    public decimal additional_drop_charge { get; set; }

    public decimal withdrawalCharges { get; set; }

    public decimal cancellationCharges { get; set; }

    public int? __v { get; set; }
}
