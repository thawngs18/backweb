using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace doan_web.Models
{
    public class ChiTietGioHang
    {

        public int? MaGioHang { get; set; }
        public GioHang? GioHang { get; set; }

        public int? MaSP { get; set; }
        public SanPham? SanPham { get; set; }

        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
    }
}
