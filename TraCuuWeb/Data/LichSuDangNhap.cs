using System;
using System.Collections.Generic;

namespace TraCuuWeb.Data;

public partial class LichSuDangNhap
{
    public int Id { get; set; }

    public int IdUser { get; set; }

    public DateTime? LoginTime { get; set; }

    public string? Ipaddress { get; set; }

    public string? DeviceInfo { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;
}
