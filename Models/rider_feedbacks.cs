using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class rider_feedbacks
{
    public int riderfeedbackid { get; set; }

    public string riderId { get; set; } = null!;

    public string feedbackgivenuserid { get; set; } = null!;

    public string? feedback { get; set; }

    public string? orderid { get; set; }

    public DateTime feedbackdate { get; set; }
}
