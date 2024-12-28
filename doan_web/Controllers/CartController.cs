using doan_web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
namespace doan_web.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly VnPayService _vnPayService;

        private string address = "", city = "", phone = "", district = "";

        public CartController(ApplicationDbContext context, ILogger<HomeController> logger, VnPayService vnPayService)
        {
            _context = context;
            _logger = logger;
            _vnPayService = vnPayService;
        }

        public IActionResult Index()
        {
            var userAccount = HttpContext.Session.GetString("TaiKhoan");

            if (string.IsNullOrEmpty(userAccount))
            {
                return RedirectToAction("Login", "Home");
            }

            var customer = _context.KhachHang.FirstOrDefault(k => k.TaiKhoan == userAccount);

            if (customer == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var cart = _context.GioHang
                               .Include(g => g.ChiTietGioHangs)
                               .ThenInclude(c => c.SanPham)
                               .FirstOrDefault(g => g.MaKH == customer.Id);

            if (cart == null)
            {
                cart = new GioHang { MaKH = customer.Id, SoTien = 0 };
            }

            cart.SoTien = cart.ChiTietGioHangs.Sum(c => (decimal)(c.SoLuong * c.SanPham.GiaBan));

            return View(cart);
        }
        public IActionResult AddToCart(int productId)
        {
            // Get the user account from the session
            var userAccount = HttpContext.Session.GetString("TaiKhoan");

            if (string.IsNullOrEmpty(userAccount))
            {
                return RedirectToAction("Login", "Home");
            }

            var customer = _context.KhachHang.FirstOrDefault(k => k.TaiKhoan == userAccount);

            if (customer == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var product = _context.SanPham.FirstOrDefault(p => p.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            var cart = _context.GioHang
                               .Include(g => g.ChiTietGioHangs)
                               .ThenInclude(c => c.SanPham)
                               .FirstOrDefault(g => g.MaKH == customer.Id);

            if (cart == null)
            {
                cart = new GioHang { MaKH = customer.Id, SoTien = 0 };
            }

            var cartItem = cart.ChiTietGioHangs.FirstOrDefault(c => c.MaSP == productId);

            if (cartItem == null)
            {
                cartItem = new ChiTietGioHang
                {
                    MaSP = productId,
                    SoLuong = 1,
                    MaGioHang = cart.Id,
                    DonGia = product.GiaBan
                };
                cart.ChiTietGioHangs.Add(cartItem);
            }
            else
            {
                cartItem.SoLuong += 1;  // Increase quantity if the product is already in the cart
            }

            _context.SaveChanges();

            // Recalculate total price
            cart.SoTien = cart.ChiTietGioHangs.Sum(c => c.SoLuong * c.SanPham.GiaBan);
            _context.SaveChanges();

            return RedirectToAction("Index", "Cart");  // Redirect to Cart page after adding the product
        }

        public IActionResult UpdateQuantity(int cartItemId, int newQuantity)
        {
            var cartItem = _context.ChiTietGioHang.FirstOrDefault(c => c.MaGioHang == cartItemId);

            if (cartItem == null || newQuantity < 1)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng hoặc số lượng không hợp lệ." });
            }

            // Update the quantity of the product
            cartItem.SoLuong = newQuantity;

            // Save changes to the cart item
            _context.SaveChanges();

            // Recalculate the total price for the cart
            var cart = _context.GioHang.Include(g => g.ChiTietGioHangs).FirstOrDefault(g => g.Id == cartItem.MaGioHang);

            if (cart != null)
            {
                cart.SoTien = cart.ChiTietGioHangs
                    .Where(c => c.SanPham != null)
                    .Sum(c => (decimal)(c.SoLuong * (c.SanPham?.GiaBan ?? 0)));

                // Save the updated cart with the recalculated total
                _context.SaveChanges();
            }

            return Json(new { success = true, message = "Cập nhật số lượng thành công." });
        }



        public IActionResult XoaSanPham(int cartItemId)
        {
            var cartItem = _context.ChiTietGioHang.FirstOrDefault(c => c.MaGioHang == cartItemId);

            if (cartItem != null)
            {
                var cart = _context.GioHang.Include(g => g.ChiTietGioHangs).FirstOrDefault(g => g.Id == cartItem.MaGioHang);

                if (cart != null)
                {
                    // Remove the cart item from the list of items in the cart
                    cart.ChiTietGioHangs.Remove(cartItem);

                    // Recalculate the total price for the cart
                    cart.SoTien = cart.ChiTietGioHangs
                        .Where(c => c.SanPham != null)
                        .Sum(c => (decimal)(c.SoLuong * (c.SanPham?.GiaBan ?? 0)));

                    // Save changes
                    _context.SaveChanges();
                }
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult Checkout(IFormCollection frm)
        {
            try
            {
                // Retrieve the logged-in user's account
                string taiKhoan = HttpContext.Session.GetString("TaiKhoan");

                // Fetch the customer with their cart, ensure it's included correctly
                KhachHang customer = _context.KhachHang
                    .Include(g => g.GioHang)
                    .ThenInclude(g => g.ChiTietGioHangs)  // Make sure to include cart details properly
                    .FirstOrDefault(k => k.TaiKhoan == taiKhoan);

                // Handle case where customer or cart is not found
                if (customer == null || customer.GioHang == null || !customer.GioHang.ChiTietGioHangs.Any())
                {
                    return Json(new { success = false, message = "Giỏ hàng của bạn hiện tại không có sản phẩm hoặc khách hàng không tồn tại!" });
                }

                GioHang gioHang = customer.GioHang;

                // Calculate the total amount of the cart
                decimal totalAmount = gioHang.ChiTietGioHangs.Sum(item => item.SoLuong * item.DonGia);

                totalAmount = Math.Round(totalAmount * 100);

                this.address = frm["address"].ToString();
                this.city = frm["city"].ToString();
                this.district = frm["district"].ToString();
                this.phone = frm["phone"].ToString();

                var model = new PaymentInformationModel
                {
                    OrderType = "other",
                    Amount = totalAmount,
                    OrderDescription = "Thanh toán đơn hàng",
                    Name = frm["fullName"].ToString()
                };

                HttpContext.Session.SetString("Address", frm["address"].ToString());
                HttpContext.Session.SetString("City", frm["city"].ToString());
                HttpContext.Session.SetString("District", frm["district"].ToString());
                HttpContext.Session.SetString("Phone", frm["phone"].ToString());
                HttpContext.Session.SetString("FullName", frm["fullName"].ToString());

                var paymentUrl = CreatePaymentUrl(model);

                return Redirect(paymentUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi thanh toán: {ex.Message}");

                return Json(new { success = false, message = "Có lỗi xảy ra. Vui lòng thử lại!" });
            }
        }



        public string CreatePaymentUrl(PaymentInformationModel model)
        {
            string vnp_Version = "2.1.0";
            string vnp_Command = "pay";
            string orderType = "other";
            string bankCode = "NCB";

            string vnp_TxnRef = GetRandomNumber(8);
            string vnp_IpAddr = "127.0.0.1";
            string vnp_TmnCode = "EKRUDLI9";

            var vnp_Params = new Dictionary<string, string>
    {
        { "vnp_Version", vnp_Version },
        { "vnp_Command", vnp_Command },
        { "vnp_TmnCode", vnp_TmnCode },
        { "vnp_Amount", (model.Amount).ToString() },
        { "vnp_CurrCode", "VND" },
        { "vnp_BankCode", bankCode },
        { "vnp_TxnRef", vnp_TxnRef },
        { "vnp_OrderInfo", "Thanhtoan" },
        { "vnp_OrderType", model.OrderType },
        { "vnp_Locale", "vn" },
        { "vnp_ReturnUrl", "https://localhost:7028/Cart/PaymentCallback" },
        { "vnp_IpAddr", vnp_IpAddr },
        { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") }
    };

            DateTime expireDate = DateTime.Now.AddMinutes(15);
            vnp_Params.Add("vnp_ExpireDate", expireDate.ToString("yyyyMMddHHmmss"));

            var fieldNames = vnp_Params.Keys.ToList();
            fieldNames.Sort();

            StringBuilder hashData = new StringBuilder();
            StringBuilder query = new StringBuilder();

            foreach (var fieldName in fieldNames)
            {
                string fieldValue = vnp_Params[fieldName];
                if (!string.IsNullOrEmpty(fieldValue))
                {
                    hashData.Append(fieldName).Append('=').Append(Uri.EscapeDataString(fieldValue));
                    query.Append(Uri.EscapeDataString(fieldName))
                         .Append('=')
                         .Append(Uri.EscapeDataString(fieldValue));

                    if (fieldNames.IndexOf(fieldName) < fieldNames.Count - 1)
                    {
                        query.Append('&');
                        hashData.Append('&');
                    }
                }
            }

            string secretKey = "43DZY80MSHLDF5TI93OQKLB7LAQIOK2N";
            string secureHash = HmacSHA512(secretKey, hashData.ToString());

            query.Append("&vnp_SecureHash=").Append(secureHash);

            string paymentUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?" + query.ToString();

            System.Diagnostics.Debug.WriteLine($"Payment URL: {paymentUrl}");

            return paymentUrl;
        }


        public string GetRandomNumber(int length)
        {
            Random rnd = new Random();
            const string chars = "0123456789";
            StringBuilder sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[rnd.Next(chars.Length)]);
            }

            return sb.ToString();
        }


        public static string HmacSHA512(string key, string data)
        {
            if (key == null || data == null)
            {
                throw new ArgumentNullException("Key and data must not be null.");
            }

            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                byte[] hashBytes = hmac.ComputeHash(dataBytes);

                StringBuilder sb = new StringBuilder(hashBytes.Length * 2);
                foreach (byte b in hashBytes)
                {
                    sb.AppendFormat("{0:x2}", b);
                }

                return sb.ToString();
            }
        }


        public ActionResult PaymentCallback()
        {
            try
            {
                var request = ControllerContext.HttpContext.Request;

                var response = _vnPayService.PaymentExecute(request);


                string taiKhoan = HttpContext.Session.GetString("TaiKhoan");

                if (string.IsNullOrEmpty(taiKhoan))
                {
                    return RedirectToAction("Login", "Home");
                }

                KhachHang customer = _context.KhachHang
                 .Include(g => g.GioHang)
                 .ThenInclude(g => g.ChiTietGioHangs)
                 .ThenInclude(d => d.SanPham)
                 .FirstOrDefault(k => k.TaiKhoan == taiKhoan);

                if (customer == null)
                {
                    return RedirectToAction("Login", "Home");
                }

                var gioHang = customer.GioHang;

                if (gioHang == null || gioHang.ChiTietGioHangs.Count == 0)
                {
                    return Json(new { success = false, message = "Giỏ hàng của bạn hiện tại không có sản phẩm!" });
                }

                decimal totalAmount = gioHang.ChiTietGioHangs.Sum(item => item.SoLuong * item.DonGia);

                this.address = HttpContext.Session.GetString("Address");
                this.city = HttpContext.Session.GetString("City");
                this.district = HttpContext.Session.GetString("District");
                this.phone = HttpContext.Session.GetString("Phone");

                // Create a new order for the customer
                var order = new DonHang
                {
                    MaKH = customer.Id,
                    TongTien = totalAmount,
                    DiaChi = this.address,
                    ThanhPho = this.city,
                    QuanHuyen = this.district,
                    SoDienThoai = this.phone,
                    NgayDat = DateTime.Now,
                    Status = -1
                };

                _context.DonHang.Add(order);
                _context.SaveChanges();

                List<ChiTietDonHang> orderItems = new List<ChiTietDonHang>();

                foreach (var item in gioHang.ChiTietGioHangs)
                {
                    ChiTietDonHang chiTietDonHang = new ChiTietDonHang()
                    {
                        MaDonHang = order.Id,
                        MaSP = item.MaSP,
                        SoLuong = item.SoLuong,
                        DonGia = item.DonGia
                    };

                    totalAmount += item.SoLuong * item.DonGia;
                    _context.ChiTietDonHang.Add(chiTietDonHang);
                    orderItems.Add(chiTietDonHang);
                }

                _context.SaveChanges();

                gioHang.ChiTietGioHangs.Clear();
                _context.SaveChanges();

                ViewBag.Message = "Thanh toán thành công!";
                ViewBag.OrderId = order.Id;
                ViewBag.OrderDate = order.NgayDat.ToString("dd/MM/yyyy");
                ViewBag.TotalAmount = totalAmount;
                ViewBag.OrderItems = orderItems; // Pass the order items to the view

                return View("PaymentSuccess"); // Redirect to success view

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                ViewBag.Message = "Đã xảy ra lỗi trong quá trình xử lý thanh toán.";
                return View("Error");
            }
        }





    }
}
