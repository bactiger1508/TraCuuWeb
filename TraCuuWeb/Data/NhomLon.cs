using System;
using System.Collections.Generic;

namespace TraCuuWeb.Data;

public partial class NhomLon
{
    public int Id { get; set; }

    public string TenNhomLon { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<NhomGoiCuocBangRong> NhomGoiCuocBangRongs { get; set; } = new List<NhomGoiCuocBangRong>();

    public virtual ICollection<NhomGoiCuocDiDong> NhomGoiCuocDiDongs { get; set; } = new List<NhomGoiCuocDiDong>();
}
