
using doan_web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatDoAn.Controllers
{
    public class MenuController : Controller
    {
        private readonly ILogger<MenuController> _logger;
        private readonly ApplicationDbContext _context;

        public MenuController(ILogger<MenuController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(string category)
        {
            // Retrieve all categories
            var categories = _context.LoaiSanPham.ToList();

            // Retrieve products based on selected category or all if category is null
            var productsQuery = _context.SanPham.Include(sp => sp.LoaiSanPham).AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                // Filter by category
                productsQuery = productsQuery.Where(sp => sp.LoaiSanPham.Ten.ToLower() == category.ToLower());
            }

            // Filter products where SoLuong > 0
            productsQuery = productsQuery.Where(sp => sp.SoLuong > 0);

            // Get the filtered products
            var products = productsQuery.ToList();

            // Pass both categories and products to the view
            var viewModel = new MenuViewModel
            {
                Categories = categories,
                Products = products
            };

            return View(viewModel);
        }



        // Action to display the details of a specific product
        public IActionResult Detail(int productId)
        {
            var product = _context.SanPham.Include(p => p.PhanHois).ThenInclude(ph => ph.KhachHang)
                .FirstOrDefault(p => p.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductDetailViewModel
            {
                Product = product,
                Feedbacks = product.PhanHois.ToList()
            };

            return View(model);
        }


        [HttpPost]
        public IActionResult SubmitFeedback(int productId, string content, int stars)
        {
            if (string.IsNullOrWhiteSpace(content) || stars < 1 || stars > 5)
            {
                return RedirectToAction("Detail", new { productId });
            }

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

            var profanityFilter = new ProfanityFilter(Path.Combine(Directory.GetCurrentDirectory(), "Data/en.txt"));
            if (profanityFilter.ContainsProfanity(content))
            {
                TempData["Error"] = "Nội dung phản hồi chứa từ ngữ không phù hợp. Vui lòng sửa lại.";
                return RedirectToAction("Detail", new { productId });
            }

            var feedback = new PhanHoi
            {
                MaSP = productId,
                MaKH = customer.Id,
                NoiDung = content,
                SoSao = stars,
                NgayBinhLuan = DateTime.Now
            };

            _context.PhanHoi.Add(feedback);
            _context.SaveChanges();

            TempData["Success"] = "Phản hồi của bạn đã được gửi thành công!";
            return RedirectToAction("Detail", new { productId });
        }

    }
}
