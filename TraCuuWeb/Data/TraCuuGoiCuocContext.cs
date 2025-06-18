using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TraCuuWeb.Data;

public partial class TraCuuGoiCuocContext : DbContext
{
    public TraCuuGoiCuocContext()
    {
    }

    public TraCuuGoiCuocContext(DbContextOptions<TraCuuGoiCuocContext> options)
        : base(options)
    {
    }

    public virtual DbSet<GiaCuoc> GiaCuocs { get; set; }

    public virtual DbSet<GoiCuoc> GoiCuocs { get; set; }

    public virtual DbSet<LichSuDangKy> LichSuDangKies { get; set; }

    public virtual DbSet<LichSuDangNhap> LichSuDangNhaps { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<NhomGoiCuocBangRong> NhomGoiCuocBangRongs { get; set; }

    public virtual DbSet<NhomGoiCuocDiDong> NhomGoiCuocDiDongs { get; set; }

    public virtual DbSet<NhomLon> NhomLons { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<ThanhToan> ThanhToans { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GiaCuoc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GiaCuoc__3214EC07ED954F8C");

            entity.ToTable("GiaCuoc");

            entity.HasIndex(e => e.IdGoiCuoc, "IDX_GiaCuoc_IdGoiCuoc");

            entity.Property(e => e.Gia).HasMaxLength(50);
            entity.Property(e => e.ThoiHan).HasMaxLength(50);

            entity.HasOne(d => d.IdGoiCuocNavigation).WithMany(p => p.GiaCuocs)
                .HasForeignKey(d => d.IdGoiCuoc)
                .HasConstraintName("FK__GiaCuoc__IdGoiCu__4F7CD00D");
        });

        modelBuilder.Entity<GoiCuoc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GoiCuoc__3214EC079C6406F6");

            entity.ToTable("GoiCuoc");

            entity.Property(e => e.DichVuBundle).HasMaxLength(100);
            entity.Property(e => e.DoiTuong).HasMaxLength(255);
            entity.Property(e => e.HinhAnh).HasMaxLength(255);
            entity.Property(e => e.TenGoi).HasMaxLength(100);

            entity.HasOne(d => d.IdNhomBangRongNavigation).WithMany(p => p.GoiCuocs)
                .HasForeignKey(d => d.IdNhomBangRong)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__GoiCuoc__IdNhomB__4CA06362");

            entity.HasOne(d => d.IdNhomDiDongNavigation).WithMany(p => p.GoiCuocs)
                .HasForeignKey(d => d.IdNhomDiDong)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__GoiCuoc__IdNhomD__4BAC3F29");
        });

        modelBuilder.Entity<LichSuDangKy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LichSuDa__3214EC071E538EAE");

            entity.ToTable("LichSuDangKy");

            entity.HasIndex(e => e.IdGiaCuoc, "IDX_LSDK_IdGiaCuoc");

            entity.HasIndex(e => e.IdGoiCuoc, "IDX_LSDK_IdGoiCuoc");

            entity.HasIndex(e => e.IdUser, "IDX_LSDK_IdUser");

            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.ThoiGianDangKy)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.IdGiaCuocNavigation).WithMany(p => p.LichSuDangKies)
                .HasForeignKey(d => d.IdGiaCuoc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LichSuDan__IdGia__571DF1D5");

            entity.HasOne(d => d.IdGoiCuocNavigation).WithMany(p => p.LichSuDangKies)
                .HasForeignKey(d => d.IdGoiCuoc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LichSuDan__IdGoi__5629CD9C");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.LichSuDangKies)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK__LichSuDan__IdUse__5535A963");
        });

        modelBuilder.Entity<LichSuDangNhap>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LichSuDa__3214EC07252E953E");

            entity.ToTable("LichSuDangNhap");

            entity.Property(e => e.DeviceInfo).HasMaxLength(255);
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");
            entity.Property(e => e.LoginTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.LichSuDangNhaps)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK__LichSuDan__IdUse__5AEE82B9");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Logs__3214EC07968F781C");

            entity.HasIndex(e => e.LogDate, "IDX_Logs_LogDate");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TotalRevenue)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.WebsiteVisits).HasDefaultValue(0);
        });

        modelBuilder.Entity<NhomGoiCuocBangRong>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NhomGoiC__3214EC07891D0B52");

            entity.ToTable("NhomGoiCuocBangRong");

            entity.HasIndex(e => e.TenNhom, "UQ__NhomGoiC__2B432D0DBCA03FDF").IsUnique();

            entity.Property(e => e.TenNhom).HasMaxLength(100);

            entity.HasOne(d => d.IdNhomLonNavigation).WithMany(p => p.NhomGoiCuocBangRongs)
                .HasForeignKey(d => d.IdNhomLon)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__NhomGoiCu__IdNho__48CFD27E");
        });

        modelBuilder.Entity<NhomGoiCuocDiDong>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NhomGoiC__3214EC07C5B88F11");

            entity.ToTable("NhomGoiCuocDiDong");

            entity.HasIndex(e => e.TenNhom, "UQ__NhomGoiC__2B432D0D14CCFC6A").IsUnique();

            entity.Property(e => e.TenNhom).HasMaxLength(100);

            entity.HasOne(d => d.IdNhomLonNavigation).WithMany(p => p.NhomGoiCuocDiDongs)
                .HasForeignKey(d => d.IdNhomLon)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__NhomGoiCu__IdNho__44FF419A");
        });

        modelBuilder.Entity<NhomLon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NhomLon__3214EC07EA0890BE");

            entity.ToTable("NhomLon");

            entity.HasIndex(e => e.TenNhomLon, "UQ__NhomLon__67E6FF2670567E14").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TenNhomLon).HasMaxLength(100);
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Password__3214EC074BCC665F");

            entity.HasIndex(e => e.IdUser, "IDX_PasswordResetTokens_IdUser");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiresAt).HasColumnType("datetime");
            entity.Property(e => e.IsUsed).HasDefaultValue(false);
            entity.Property(e => e.Token).HasMaxLength(255);

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.PasswordResetTokens)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK__PasswordR__IdUse__6477ECF3");
        });

        modelBuilder.Entity<ThanhToan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ThanhToa__3214EC0780C91F18");

            entity.ToTable("ThanhToan");

            entity.HasIndex(e => e.IdLichSuDangKy, "IDX_ThanhToan_IdLichSuDangKy");

            entity.HasIndex(e => e.IdUser, "IDX_ThanhToan_IdUser");

            entity.HasIndex(e => e.ThoiGianThanhToan, "IDX_ThanhToan_ThoiGianThanhToan");

            entity.Property(e => e.MaGiaoDich).HasMaxLength(100);
            entity.Property(e => e.PhuongThucThanhToan).HasMaxLength(50);
            entity.Property(e => e.SoTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ThoiGianThanhToan)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.IdLichSuDangKyNavigation).WithMany(p => p.ThanhToans)
                .HasForeignKey(d => d.IdLichSuDangKy)
                .HasConstraintName("FK__ThanhToan__IdLic__6B24EA82");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.ThanhToans)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ThanhToan__IdUse__6C190EBB");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07884F0CE0");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E41F3ADA7A").IsUnique();

            entity.HasIndex(e => e.PhoneNumber, "UQ__Users__85FB4E388950E5E7").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValue("KhachHang");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Active");
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
