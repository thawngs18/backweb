using System.Collections.Generic;

namespace doan_web.Models
{
    public class MenuViewModel
    {
        public List<LoaiSanPham> Categories { get; set; }
        public List<SanPham> Products { get; set; }
    }
}
