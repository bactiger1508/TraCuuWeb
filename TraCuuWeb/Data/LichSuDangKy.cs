using System;
using System.Collections.Generic;

namespace TraCuuWeb.Data;

public partial class LichSuDangKy
{
    public int Id { get; set; }

    public int IdUser { get; set; }

    public string SoDienThoai { get; set; } = null!;

    public int IdGoiCuoc { get; set; }

    public int IdGiaCuoc { get; set; }

    public string TrangThai { get; set; } = null!;

    public DateTime? ThoiGianDangKy { get; set; }

    public string? GhiChu { get; set; }

    public virtual GiaCuoc IdGiaCuocNavigation { get; set; } = null!;

    public virtual GoiCuoc IdGoiCuocNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
}
