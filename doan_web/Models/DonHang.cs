using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace doan_web.Models
{
    public class DonHang
    {
        public int Id { get; set; } // Changed from string to int
        public DateTime NgayDat { get; set; } // Changed from string to int
        public int? MaNV { get; set; }
        public NhanVien? NhanVien { get; set; }

        public int? MaKH { get; set; }
        public decimal TongTien { get; set; }
        public int? Status { get; set; }
        public string? DiaChi { get; set; }
        public string? ThanhPho { get; set; }
        public string? QuanHuyen { get; set; }
        public string? SoDienThoai { get; set; }
        public KhachHang? KhachHang { get; set; }
        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    }
}
