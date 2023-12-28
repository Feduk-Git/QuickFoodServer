using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickFoodServer.Models;
using QuickFoodServer.Utils;

namespace QuickFoodServer.Controllers
{
    [Authorize]
    public class ProductController: Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(int page = 1)
        {
            List<Product> products = _context.Products.Include(p => p.Categories).ToList();

            const int pageSize = 5;
            if (page < 1)
                page = 1;

            int recsCount = products.Count();
            var pager = new Pager(recsCount, page, pageSize);
            int recSkip = (page - 1) * pageSize;
            var data = products.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;
            ViewBag.Categories = _context.Categories.ToList();

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(string name, string priceStr, string description, List<int> categoryIds, IFormFile image)
        {
            if (!decimal.TryParse(priceStr, out decimal price))
            {
                TempData["ErrorMessage"] = "Error! Invalid price";
                return RedirectToAction("Index");
            }

            Product? lastProduct = _context.Products.OrderBy(c => c.Id).LastOrDefault();

            string absolutePath;
            string relativePath;
            if (lastProduct != null)
            {
                absolutePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Products", $"product{lastProduct.Id + 1}" + Path.GetExtension(image.FileName));
                relativePath = Path.Combine("..", "Images", "Products", $"product{lastProduct.Id + 1}" + Path.GetExtension(image.FileName));
            }
            else
            {
                absolutePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Products", $"product0" + Path.GetExtension(image.FileName));
                relativePath = Path.Combine("..", "Images", "Products", $"product0" + Path.GetExtension(image.FileName));
            }

            List<Category> categories = _context.Categories.Where(c => categoryIds.Contains(c.Id)).ToList();

            Product product = new Product();
            product.Name = name;    
            product.Price = price;
            product.Description = description;
            product.ImagePathAbsolute = absolutePath;
            product.ImagePathRelative = relativePath;
            product.Categories = categories;
            
            _context.Products.AddRange(product);
            _context.SaveChanges();

            using (var stream = new FileStream(absolutePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            System.IO.File.Delete(product.ImagePathAbsolute);
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateAsync(int id, string name, IFormFile image, string description, List<int> categoryIds, string priceStr)
        {
            if (!decimal.TryParse(priceStr, out decimal price))
            {
                TempData["ErrorMessage"] = "Error! Invalid price";
                return RedirectToAction("Index");
            }

            Product product = _context.Products.Include(p => p.Categories).FirstOrDefault(p => p.Id == id);

            if (image != null)
            {
                string absolutePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Products", $"product{product.Id}" + Path.GetExtension(image.FileName));
                string relativePath = Path.Combine("..", "Images", "Products", $"product{product.Id}" + Path.GetExtension(image.FileName));

                System.IO.File.Delete(product.ImagePathAbsolute);

                product.ImagePathAbsolute = absolutePath;
                product.ImagePathRelative = relativePath;

                using (var stream = new FileStream(absolutePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
            }

            product.Name = name;
            product.Description = description;
            product.Price = price;

            if (categoryIds.Count != 0)
            {
                List<Category> categories = _context.Categories.Where(c => categoryIds.Contains(c.Id)).ToList();
                product.Categories = categories;
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
