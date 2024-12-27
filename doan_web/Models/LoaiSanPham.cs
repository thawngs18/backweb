using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace doan_web.Models
{
    public class LoaiSanPham
    {
        public int Id { get; set; } // Changed from string to int
        public string? Ten { get; set; } // Changed from string to int
        public ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
    }
}
