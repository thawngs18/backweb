using System.Collections.Generic;
using System;

namespace doan_web.Models
{
    public class PaymentSuccessModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string OrderDate { get; set; }
        public int TotalAmount { get; set; }
        public List<ChiTietDonHang> ChiTietDonHangs { get; set; }
    }
}