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
                ViewBag.ThongBao = "T�i kho?n ho?c email d� t?n t?i.";
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
                ViewBag.ThongBao = "�� c� l?i x?y ra trong qu� tr�nh dang k�.";
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
            // Truy v?n don h�ng c�ng v?i c�c chi ti?t li�n quan
            var order = _context.DonHang
                .Include(d => d.ChiTietDonHangs)
                .ThenInclude(c => c.SanPham)
                .Include(d => d.KhachHang)  // Th�m th�ng tin kh�ch h�ng
                .Include(d => d.NhanVien)   // Th�m th�ng tin nh�n vi�n (n?u c�)
                .FirstOrDefault(d => d.Id == id);

            if (order == null)
            {
                return Content("Kh�ng t�m th?y don h�ng.");
            }

            // T?o HTML d? hi?n th? th�ng tin don h�ng
            var orderDetailsHtml = "<h4>Th�ng tin don h�ng</h4>";

            // Th�ng tin kh�ch h�ng v� nh�n vi�n
            orderDetailsHtml += "<p><strong>Kh�ch h�ng:</strong> " + order.KhachHang?.Ten ?? "Chua c� t�n kh�ch h�ng" + "</p>";

            // Th�ng tin don h�ng
            orderDetailsHtml += "<p><strong>M� don h�ng:</strong> " + order.Id + "</p>";
            orderDetailsHtml += "<p><strong>Ng�y d?t:</strong> " + order.NgayDat.ToString("dd/MM/yyyy") + "</p>";
            orderDetailsHtml += "<p><strong>�?a ch? giao h�ng:</strong> " + order.DiaChi + ", " + order.QuanHuyen + ", " + order.ThanhPho + "</p>";
            orderDetailsHtml += "<p><strong>S? di?n tho?i:</strong> " + order.SoDienThoai + "</p>";
            orderDetailsHtml += "<p><strong>Tr?ng th�i:</strong> " +
    (order.Status == -1 ? "Ch? x�c nh?n" :
    order.Status == 0 ? "�� hu?" :
    order.Status == 1 ? "�ang giao h�ng" :
    order.Status == 2 ? "�� giao h�ng" :
    "Kh�ng x�c d?nh") + "</p>";

            // B?ng th�ng tin chi ti?t don h�ng
            orderDetailsHtml += "<h3>Chi ti?t s?n ph?m trong don h�ng</h3>";
            orderDetailsHtml += "<table class='table table-striped'><thead><tr><th>S?n ph?m</th><th>S? lu?ng</th><th>�on gi�</th><th>T?ng</th></tr></thead><tbody>";

            foreach (var item in order.ChiTietDonHangs)
            {
                var productName = item.SanPham?.Ten ?? "Chua c� t�n s?n ph?m";
                var quantity = item.SoLuong; // Ki?m tra s? lu?ng c� th? null
                var unitPrice = item.DonGia; // Ki?m tra don gi� c� th? null

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
                        ViewBag.ThongBao = "B?n kh�ng th? dang nh?p l� nh�n vi�n khi b?n l� kh�ch h�ng!";
                        return View();
                    }

                    HttpContext.Session.SetString("TaiKhoan", employee.TaiKhoan);
                    HttpContext.Session.SetString("UserRole", "NhanVien");

                    return RedirectToAction("Dashboard", "Admin");
                }

                ViewBag.ThongBao = "T�i kho?n ho?c m?t kh?u kh�ng d�ng!";
            }

            return View();
        }
    }
}
