using Microsoft.AspNetCore.Mvc;

using doan_web.Models;

namespace doan_web.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Dashboard()
        {
            var productCount = _context.SanPham.Count();

            var employeeCount = _context.NhanVien.Count();

            var customerCount = _context.KhachHang.Count();

            var orderCount = _context.DonHang.Count();

            ViewBag.ProductCount = productCount;
            ViewBag.EmployeeCount = employeeCount;
            ViewBag.CustomerCount = customerCount;
            ViewBag.OrderCount = orderCount;

            return View();
        }


        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }
    }

}
