using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace doan_web.Models
{
    public class PhanHoi
    {
        public int Id { get; set; } // Changed from string to int
        public DateTime? NgayBinhLuan { get; set; } // Changed from string to int
        public int? SoSao {  get; set; } 
        public string? NoiDung {  get; set; } 

        public int? MaKH { get; set; }
        public KhachHang? KhachHang { get; set; }

        public int? MaSP { get; set; }
        public SanPham? SanPham { get; set; }
    }
}
