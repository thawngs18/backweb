using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace doan_web.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<ChiTietDonHang> ChiTietDonHang { get; set; }
        public DbSet<DonHang> DonHang { get; set; }
        public DbSet<KhachHang> KhachHang { get; set; }
        public DbSet<LoaiSanPham> LoaiSanPham { get; set; }
        public DbSet<NhanVien> NhanVien { get; set; }
        public DbSet<SanPham> SanPham { get; set; }
        public DbSet<ChiTietGioHang> ChiTietGioHang { get; set; }
        public DbSet<GioHang> GioHang { get; set; }
        public DbSet<PhanHoi> PhanHoi { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
           

            modelBuilder.Entity<ChiTietDonHang>()
                .HasKey(cs => new { cs.MaDonHang, cs.MaSP });

            modelBuilder.Entity<ChiTietDonHang>()
                .HasOne(cs => cs.DonHang)
                .WithMany(s => s.ChiTietDonHangs)
                .HasForeignKey(cs => cs.MaDonHang)
                .OnDelete(DeleteBehavior.Restrict); // Vô hiệu hóa cascade delete


            modelBuilder.Entity<ChiTietDonHang>()
                .HasOne(cs => cs.SanPham)
                .WithMany(c => c.ChiTietDonHangs)
                .HasForeignKey(cs => cs.MaSP)
                .OnDelete(DeleteBehavior.Restrict); // Vô hiệu hóa cascade delete
            

            modelBuilder.Entity<DonHang>()
                .HasOne(u => u.KhachHang)
                .WithMany(r => r.DonHangs)
                .HasForeignKey(u => u.MaKH)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SanPham>()
               .HasOne(c => c.LoaiSanPham)
               .WithMany(t => t.SanPhams)
               .HasForeignKey(c => c.IDLoaiSP)
               .OnDelete(DeleteBehavior.Restrict); // Vô hiệu hóa cascade delete


            modelBuilder.Entity<DonHang>()
                .HasOne(u => u.NhanVien)
                .WithMany(r => r.DonHangs)
                .HasForeignKey(u => u.MaNV)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<ChiTietGioHang>()
              .HasKey(cs => new { cs.MaGioHang, cs.MaSP });


            modelBuilder.Entity<ChiTietGioHang>()
                .HasOne(cs => cs.GioHang)
                .WithMany(s => s.ChiTietGioHangs)
                .HasForeignKey(cs => cs.MaGioHang)
                .OnDelete(DeleteBehavior.Restrict); // Vô hiệu hóa cascade delete


            modelBuilder.Entity<ChiTietGioHang>()
                .HasOne(cs => cs.SanPham)
                .WithMany(c => c.ChiTietGioHangs)
                .HasForeignKey(cs => cs.MaSP)
                .OnDelete(DeleteBehavior.Restrict); // Vô hiệu hóa cascade delete



            modelBuilder.Entity<GioHang>()
         .HasOne(g => g.KhachHang)    // GioHang has one KhachHang
         .WithOne(k => k.GioHang)     // KhachHang has one GioHang
         .HasForeignKey<GioHang>(g => g.MaKH)   // Foreign Key in GioHang
         .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity <PhanHoi>()
                .HasOne(cs => cs.KhachHang)
                .WithMany(s => s.PhanHois)
                .HasForeignKey(cs => cs.MaKH)
                .OnDelete(DeleteBehavior.Restrict); // Vô hiệu hóa cascade delete


            modelBuilder.Entity<PhanHoi>()
                .HasOne(cs => cs.SanPham)
                .WithMany(c => c.PhanHois)
                .HasForeignKey(cs => cs.MaSP)
                .OnDelete(DeleteBehavior.Restrict); // Vô hiệu hóa cascade delete
                                                    // Thêm nhân viên "admin"
            modelBuilder.Entity<NhanVien>().HasData(
                new NhanVien
                {
                    Id = 1, // Đảm bảo giá trị này là duy nhất và tự động tăng
                    TaiKhoan = "admin",
                    MatKhau = "123", // Lưu ý: Mật khẩu nên được mã hóa trong thực tế
                                      // Thêm các thông tin khác của nhân viên (nếu cần)
                }
            );

            // Thêm các loại sản phẩm
            modelBuilder.Entity<LoaiSanPham>().HasData(
                new LoaiSanPham { Id = 1, Ten = "Món Chính" },
                new LoaiSanPham { Id = 2, Ten = "Khai Vị" },
                new LoaiSanPham { Id = 3, Ten = "Tráng Miệng" },
                new LoaiSanPham { Id = 4, Ten = "Đồ Uống" }
            );
        }


    }
}
