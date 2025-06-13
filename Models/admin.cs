using System;
using System.Collections.Generic;

namespace InstaportApi.Models;

public partial class admin
{
    public Guid _id { get; set; }

    public string username { get; set; } = null!;

    public string password { get; set; } = null!;

    public string role { get; set; } = null!;

    public int? __v { get; set; }
}
