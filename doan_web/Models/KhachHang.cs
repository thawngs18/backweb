using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace doan_web.Models
{
    public class KhachHang
    {
        public int Id { get; set; } // Changed from string to int
        public string? Ten { get; set; }
        public string? DiaChi { get; set; }
        public string? Email { get; set; }
        public string? SDT { get; set; }
        public string? TaiKhoan { get; set; }
        public string? MatKhau { get; set; }

        public ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
        public ICollection<PhanHoi> PhanHois { get; set; } = new List<PhanHoi>();

        // One-to-one relationship with GioHang
        public GioHang? GioHang { get; set; } // Navigation property
    }

}
