using System;
using System.Collections.Generic;

namespace TraCuuWeb.Data;

public partial class GoiCuoc
{
    public int Id { get; set; }

    public string TenGoi { get; set; } = null!;

    public int? IdNhomDiDong { get; set; }

    public int? IdNhomBangRong { get; set; }

    public string? QuyDinh { get; set; }

    public string? GhiChu { get; set; }

    public string NoiDung { get; set; } = null!;

    public string? DoiTuong { get; set; }

    public string? DichVuBundle { get; set; }

    public string? HinhAnh { get; set; }

    public virtual ICollection<GiaCuoc> GiaCuocs { get; set; } = new List<GiaCuoc>();

    public virtual NhomGoiCuocBangRong? IdNhomBangRongNavigation { get; set; }

    public virtual NhomGoiCuocDiDong? IdNhomDiDongNavigation { get; set; }

    public virtual ICollection<LichSuDangKy> LichSuDangKies { get; set; } = new List<LichSuDangKy>();
}
