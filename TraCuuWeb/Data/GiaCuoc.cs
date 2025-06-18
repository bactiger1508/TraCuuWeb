using System;
using System.Collections.Generic;

namespace TraCuuWeb.Data;

public partial class GiaCuoc
{
    public int Id { get; set; }

    public int IdGoiCuoc { get; set; }

    public string Gia { get; set; } = null!;

    public int? GiaHan { get; set; }

    public string? ThoiHan { get; set; }

    public int? ChuKy { get; set; }

    public virtual GoiCuoc IdGoiCuocNavigation { get; set; } = null!;

    public virtual ICollection<LichSuDangKy> LichSuDangKies { get; set; } = new List<LichSuDangKy>();
}
