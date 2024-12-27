using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
namespace doan_web.Models
{
    public class SanPham
    {
        public int Id { get; set; } // Changed from string to int
        public string? Ten { get; set; } // Changed from string to int
        public int SoLuong { get; set; } // Changed from string to int
        public decimal GiaBan { get; set; } // Changed from string to int

        public int? IDLoaiSP { get; set; }
        public string? img { get; set; }
        public string? MoTa { get; set; }
        public LoaiSanPham? LoaiSanPham { get; set; }

        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
        public ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();
        public ICollection<PhanHoi> PhanHois { get; set; } = new List<PhanHoi>();
    }
}
