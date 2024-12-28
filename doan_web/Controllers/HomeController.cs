
using doan_web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace doan_web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult DangKy()
        {
            return View(new KhachHang());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangKy(KhachHang khachHang)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError(error.ErrorMessage);
                }
                return View(khachHang);
            }

            var existingUser = _context.KhachHang
                .FirstOrDefault(k => k.TaiKhoan == khachHang.TaiKhoan || k.Email == khachHang.Email);

            if (existingUser != null)
            {
                ViewBag.ThongBao = "Tài khoản hoặc email đã tồn tại.";
                return View(khachHang);
            }

            try
            {
                _context.KhachHang.Add(khachHang);
                _context.SaveChanges();

                var gioHang = new GioHang
                {
                    MaKH = khachHang.Id,
                    SoTien = 0
                };

                _context.GioHang.Add(gioHang);
                _context.SaveChanges();

                HttpContext.Session.SetString("TaiKhoan", khachHang.TaiKhoan);
                HttpContext.Session.SetString("UserRole", "KhachHang");

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during registration process: {ex.Message}");
                ViewBag.ThongBao = "Đã có lỗi xảy ra trong quá trình đăng ký.";
                return View(khachHang);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        // GET: DangNhap
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult MyOrders()
        {
            string taiKhoan = HttpContext.Session.GetString("TaiKhoan");

            if (string.IsNullOrEmpty(taiKhoan))
            {
                return RedirectToAction("Login", "Home");
            }

            var customer = _context.KhachHang
                .Include(k => k.DonHangs) // Include orders for the customer
                .FirstOrDefault(k => k.TaiKhoan == taiKhoan);

            if (customer == null)
            {
                return View("Error");
            }

            var orders = customer.DonHangs; // Get the customer's orders

            return View(orders); // Pass orders to the view
        }

        public IActionResult GetOrderDetails(int id)
        {
            // Truy vấn đơn hàng cùng với các chi tiết liên quan
            var order = _context.DonHang
                .Include(d => d.ChiTietDonHangs)
                .ThenInclude(c => c.SanPham)
                .Include(d => d.KhachHang)  // Thêm thông tin khách hàng
                .Include(d => d.NhanVien)   // Thêm thông tin nhân viên (nếu có)
                .FirstOrDefault(d => d.Id == id);

            if (order == null)
            {
                return Content("Không tìm thấy đơn hàng.");
            }

            // Tạo HTML để hiển thị thông tin đơn hàng
            var orderDetailsHtml = "<h4>Thông tin đơn hàng</h4>";

            // Thông tin khách hàng và nhân viên
            orderDetailsHtml += "<p><strong>Khách hàng:</strong> " + order.KhachHang?.Ten ?? "Chưa có tên khách hàng" + "</p>";

            // Thông tin đơn hàng
            orderDetailsHtml += "<p><strong>Mã đơn hàng:</strong> " + order.Id + "</p>";
            orderDetailsHtml += "<p><strong>Ngày đặt:</strong> " + order.NgayDat.ToString("dd/MM/yyyy") + "</p>";
            orderDetailsHtml += "<p><strong>Địa chỉ giao hàng:</strong> " + order.DiaChi + ", " + order.QuanHuyen + ", " + order.ThanhPho + "</p>";
            orderDetailsHtml += "<p><strong>Số điện thoại:</strong> " + order.SoDienThoai + "</p>";
            orderDetailsHtml += "<p><strong>Trạng thái:</strong> " +
    (order.Status == -1 ? "Chờ xác nhận" :
    order.Status == 0 ? "Đã huỷ" :
    order.Status == 1 ? "Đang giao hàng" :
    order.Status == 2 ? "Đã giao hàng" :
    "Không xác định") + "</p>";

            // Bảng thông tin chi tiết đơn hàng
            orderDetailsHtml += "<h3>Chi tiết sản phẩm trong đơn hàng</h3>";
            orderDetailsHtml += "<table class='table table-striped'><thead><tr><th>Sản phẩm</th><th>Số lượng</th><th>Đơn giá</th><th>Tổng</th></tr></thead><tbody>";

            foreach (var item in order.ChiTietDonHangs)
            {
                var productName = item.SanPham?.Ten ?? "Chưa có tên sản phẩm";
                var quantity = item.SoLuong; // Kiểm tra số lượng có thể null
                var unitPrice = item.DonGia; // Kiểm tra đơn giá có thể null

                orderDetailsHtml += $"<tr><td>{productName}</td><td>{quantity}</td><td>{unitPrice} VND</td><td>{(quantity * unitPrice)} VND</td></tr>";
            }

            orderDetailsHtml += "</tbody></table>";

            var totalAmount = order.TongTien;
            orderDetailsHtml += $"<p><strong>Tổng tiền:</strong> {totalAmount} VND</p>";

            return Content(orderDetailsHtml);
        }




        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        // POST: DangNhap
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string TaiKhoan, string MatKhau)
        {
            if (ModelState.IsValid)
            {
                var customer = _context.KhachHang
                    .FirstOrDefault(k => k.TaiKhoan == TaiKhoan && k.MatKhau == MatKhau);

                if (customer != null)
                {
                    HttpContext.Session.SetString("TaiKhoan", customer.TaiKhoan);
                    HttpContext.Session.SetString("UserRole", "KhachHang");

                    return RedirectToAction("Index", "Home");
                }

                var employee = _context.NhanVien
                    .FirstOrDefault(n => n.TaiKhoan == TaiKhoan && n.MatKhau == MatKhau);

                if (employee != null)
                {
                    if (_context.KhachHang.Any(k => k.TaiKhoan == TaiKhoan))
                    {
                        ViewBag.ThongBao = "Bạn không thể đăng nhập là nhân viên khi bạn là khách hàng!";
                        return View();
                    }

                    HttpContext.Session.SetString("TaiKhoan", employee.TaiKhoan);
                    HttpContext.Session.SetString("UserRole", "NhanVien");

                    return RedirectToAction("Dashboard", "Admin");
                }

                ViewBag.ThongBao = "Tài khoản hoặc mật khẩu không đúng!";
            }

            return View();
        }
    }
}
