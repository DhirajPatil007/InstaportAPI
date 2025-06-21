using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class user_feedbacks
{
    public int userfeedbackid { get; set; }

    public string userid { get; set; } = null!;

    public string feedbackgivenriderid { get; set; } = null!;

    public string? feedback { get; set; }

    public string? orderid { get; set; }

    public DateTime feedbackdate { get; set; }
}
