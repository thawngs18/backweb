using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace doan_web.Models 
{ 
    public class GioHang
    {
        public int Id { get; set; } // Changed from string to int
        public decimal SoTien { get; set; } // Changed from string to int

        public int? MaKH { get; set; } // Foreign Key for KhachHang
        public KhachHang? KhachHang { get; set; } // Navigation property

        public ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();
    }

}
