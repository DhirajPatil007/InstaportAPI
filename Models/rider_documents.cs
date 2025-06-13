using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class rider_documents
{
    public Guid document_id { get; set; }

    public Guid rider_id { get; set; }

    public string? document_type { get; set; }

    public string? url { get; set; }

    public string? status { get; set; }

    public string? type { get; set; }

    public string? reason { get; set; }

    public virtual riders rider { get; set; } = null!;
}
