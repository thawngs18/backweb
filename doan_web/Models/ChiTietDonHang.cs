using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace doan_web.Models
{
    public class ChiTietDonHang
    {

        public int? MaDonHang { get; set; }
        public DonHang? DonHang { get; set; }

        public int? MaSP { get; set; }
        public SanPham? SanPham { get; set; }

        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
    }
}
