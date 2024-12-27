using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace doan_web.Models
{
    public class NhanVien
    {
        public int Id { get; set; } // Changed from string to int
        public string? Ten { get; set; } // Changed from string to int
        public string? DiaChi { get; set; } // Changed from string to int
        public string? Email { get; set; } // Changed from string to int
        public string? SDT { get; set; } // Changed from string to int
        public string? TaiKhoan { get; set; } // Changed from string to int
        public string? MatKhau { get; set; } // Changed from string to int

        public ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
    }
}
