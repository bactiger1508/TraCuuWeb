using System;
using System.Collections.Generic;

namespace TraCuuWeb.Data;

public partial class ThanhToan
{
    public int Id { get; set; }

    public int IdLichSuDangKy { get; set; }

    public int IdUser { get; set; }

    public decimal SoTien { get; set; }

    public string PhuongThucThanhToan { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public DateTime? ThoiGianThanhToan { get; set; }

    public string? MaGiaoDich { get; set; }

    public string? GhiChu { get; set; }

    public virtual LichSuDangKy IdLichSuDangKyNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
