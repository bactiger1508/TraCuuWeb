using System;
using System.Collections.Generic;

namespace TraCuuWeb.Data;

public partial class NhomGoiCuocDiDong
{
    public int Id { get; set; }

    public string TenNhom { get; set; } = null!;

    public int? IdNhomLon { get; set; }

    public virtual ICollection<GoiCuoc> GoiCuocs { get; set; } = new List<GoiCuoc>();

    public virtual NhomLon? IdNhomLonNavigation { get; set; }
}
