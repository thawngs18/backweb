using doan_web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
namespace doan_web.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SanPhamController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var sanPhams = _context.SanPham
    .Include(sp => sp.LoaiSanPham)  // Ensure LoaiSanPham is loaded with each SanPham
    .ToList();

            return View(sanPhams);
        }

        public IActionResult Create()
        {
            var loaiSanPhams = _context.LoaiSanPham.ToList();
            ViewBag.LoaiSanPhams = new SelectList(loaiSanPhams, "Id", "Ten");

            // Pass default values for Create form if necessary (can be left empty for the user to fill in)
            ViewData["Ten"] = "";
            ViewData["SoLuong"] = 0;
            ViewData["GiaBan"] = 0;
            ViewData["IDLoaiSP"] = 0;
            ViewData["Img"] = null; // or an empty string if you want
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Ten, int SoLuong, decimal GiaBan, int IDLoaiSP, IFormFile img)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var sanPham = new SanPham
                    {
                        Ten = Ten,
                        SoLuong = SoLuong,
                        GiaBan = GiaBan,
                        IDLoaiSP = IDLoaiSP
                    };

                    // Handle file upload if an image is provided
                    if (img != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(img.FileName);
                        string extension = Path.GetExtension(img.FileName);
                        fileName = fileName + "_" + Guid.NewGuid().ToString() + extension;

                        string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "imgs", fileName);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await img.CopyToAsync(stream);
                        }
                        sanPham.img = fileName;
                    }

                    // Save the new SanPham to the database
                    _context.Add(sanPham);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while creating product.");
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                }
            }

            // Repopulate the dropdown list for the LoaiSanPhams if the model is not valid
            ViewBag.LoaiSanPhams = new SelectList(_context.LoaiSanPham, "Id", "Ten", IDLoaiSP);
            return View();
        }






        public IActionResult Edit(int id)
        {
            _logger.LogInformation("Attempting to edit product with ID: {id}", id);

            var sanPham = _context.SanPham
                                  .Include(sp => sp.LoaiSanPham)
                                  .FirstOrDefault(sp => sp.Id == id);

            if (sanPham == null)
            {
                _logger.LogWarning("Product with ID {id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Found product: Id = {productId}, Ten = {productName}, SoLuong = {productQuantity}, GiaBan = {productPrice}, IDLoaiSP = {productLoaiSP}, Img = {productImg}",
                sanPham.Id, sanPham.Ten, sanPham.SoLuong, sanPham.GiaBan.ToString("C", new System.Globalization.CultureInfo("vi-VN")), sanPham.IDLoaiSP, sanPham.img);

            ViewBag.LoaiSanPhams = new SelectList(_context.LoaiSanPham, "Id", "Ten", sanPham.IDLoaiSP);

            return View(sanPham);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string Ten, int SoLuong, decimal GiaBan, int IDLoaiSP, IFormFile img)
        {
            var sanPham = _context.SanPham.Find(id);
            if (sanPham == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    sanPham.Ten = Ten;
                    sanPham.SoLuong = SoLuong;
                    sanPham.GiaBan = GiaBan;
                    sanPham.IDLoaiSP = IDLoaiSP;

                    if (img != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(img.FileName);
                        string extension = Path.GetExtension(img.FileName);
                        fileName = fileName + "_" + Guid.NewGuid().ToString() + extension;

                        string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "imgs", fileName);

                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await img.CopyToAsync(stream);
                        }

                        sanPham.img = fileName;
                    }

                    _context.Update(sanPham);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while editing product.");
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                }
            }

            ViewBag.LoaiSanPhams = new SelectList(_context.LoaiSanPham, "Id", "Ten", sanPham.IDLoaiSP);
            return View(sanPham);
        }
        // POST: SanPham/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var sanPham = _context.SanPham.Find(id);
            if (sanPham == null)
            {
                return NotFound();
            }

            _context.SanPham.Remove(sanPham);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
