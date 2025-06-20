using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class MessageCentralToken
{
    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }
}
