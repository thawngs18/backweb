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
                ViewBag.ThongBao = "Tài kho?n ho?c email dã t?n t?i.";
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
                ViewBag.ThongBao = "Ðã có l?i x?y ra trong quá trình dang ký.";
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
            // Truy v?n don hàng cùng v?i các chi ti?t liên quan
            var order = _context.DonHang
                .Include(d => d.ChiTietDonHangs)
                .ThenInclude(c => c.SanPham)
                .Include(d => d.KhachHang)  // Thêm thông tin khách hàng
                .Include(d => d.NhanVien)   // Thêm thông tin nhân viên (n?u có)
                .FirstOrDefault(d => d.Id == id);

            if (order == null)
            {
                return Content("Không tìm th?y don hàng.");
            }

            // T?o HTML d? hi?n th? thông tin don hàng
            var orderDetailsHtml = "<h4>Thông tin don hàng</h4>";

            // Thông tin khách hàng và nhân viên
            orderDetailsHtml += "<p><strong>Khách hàng:</strong> " + order.KhachHang?.Ten ?? "Chua có tên khách hàng" + "</p>";

            // Thông tin don hàng
            orderDetailsHtml += "<p><strong>Mã don hàng:</strong> " + order.Id + "</p>";
            orderDetailsHtml += "<p><strong>Ngày d?t:</strong> " + order.NgayDat.ToString("dd/MM/yyyy") + "</p>";
            orderDetailsHtml += "<p><strong>Ð?a ch? giao hàng:</strong> " + order.DiaChi + ", " + order.QuanHuyen + ", " + order.ThanhPho + "</p>";
            orderDetailsHtml += "<p><strong>S? di?n tho?i:</strong> " + order.SoDienThoai + "</p>";
            orderDetailsHtml += "<p><strong>Tr?ng thái:</strong> " +
    (order.Status == -1 ? "Ch? xác nh?n" :
    order.Status == 0 ? "Ðã hu?" :
    order.Status == 1 ? "Ðang giao hàng" :
    order.Status == 2 ? "Ðã giao hàng" :
    "Không xác d?nh") + "</p>";

            // B?ng thông tin chi ti?t don hàng
            orderDetailsHtml += "<h3>Chi ti?t s?n ph?m trong don hàng</h3>";
            orderDetailsHtml += "<table class='table table-striped'><thead><tr><th>S?n ph?m</th><th>S? lu?ng</th><th>Ðon giá</th><th>T?ng</th></tr></thead><tbody>";

            foreach (var item in order.ChiTietDonHangs)
            {
                var productName = item.SanPham?.Ten ?? "Chua có tên s?n ph?m";
                var quantity = item.SoLuong; // Ki?m tra s? lu?ng có th? null
                var unitPrice = item.DonGia; // Ki?m tra don giá có th? null

                orderDetailsHtml += $"<tr><td>{productName}</td><td>{quantity}</td><td>{unitPrice} VND</td><td>{(quantity * unitPrice)} VND</td></tr>";
            }

            orderDetailsHtml += "</tbody></table>";

            var totalAmount = order.TongTien;
            orderDetailsHtml += $"<p><strong>T?ng ti?n:</strong> {totalAmount} VND</p>";

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
                        ViewBag.ThongBao = "B?n không th? dang nh?p là nhân viên khi b?n là khách hàng!";
                        return View();
                    }

                    HttpContext.Session.SetString("TaiKhoan", employee.TaiKhoan);
                    HttpContext.Session.SetString("UserRole", "NhanVien");

                    return RedirectToAction("Dashboard", "Admin");
                }

                ViewBag.ThongBao = "Tài kho?n ho?c m?t kh?u không dúng!";
            }

            return View();
        }
    }
}
