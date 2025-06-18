using System;
using System.Collections.Generic;

namespace TraCuuWeb.Data;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<LichSuDangKy> LichSuDangKies { get; set; } = new List<LichSuDangKy>();

    public virtual ICollection<LichSuDangNhap> LichSuDangNhaps { get; set; } = new List<LichSuDangNhap>();

    public virtual ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
}
