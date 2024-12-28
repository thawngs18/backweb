using doan_web.Controllers;
using doan_web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace doan_web.Controllers
{
    public class DonHangController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public DonHangController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Fetch orders with related data
            var orders = _context.DonHang
                .Include(d => d.ChiTietDonHangs)
                .ThenInclude(c => c.SanPham)
                .Include(d => d.KhachHang)  // Thêm thông tin khách hàng
                .Include(d => d.NhanVien)
                .ToList();  // Ensure this is executed immediately to avoid deferred execution issues

            // Pass the status options to the view
            ViewBag.StatusOptions = new SelectList(new[]
            {
        new { Value = -1, Text = "Chờ xác nhận" },
        new { Value = 0, Text = "Đã huỷ" },
        new { Value = 1, Text = "Đang giao" },
        new { Value = 2, Text = "Đã giao" }
    }, "Value", "Text");

            return View(orders);
        }



        [HttpPost]
        public IActionResult UpdateStatus(int id, int status)
        {
            _logger.LogInformation("Updating order with ID: {id} to status: {status}", id, status);
            try
            {
                var order = _context.DonHang.Find(id);
                if (order == null)
                {
                    return NotFound();
                }

                order.Status = status;
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating order status");
                return StatusCode(500, "Internal server error while updating the order status.");
            }
        }


    }
}
