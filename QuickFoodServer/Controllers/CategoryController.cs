using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickFoodServer.Models;
using QuickFoodServer.Utils;
using System.IO;

namespace QuickFoodServer.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryController(Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(int page = 1)
        {
            List<Category> categories = _context.Categories.ToList();

            const int pageSize = 5;
            if (page < 1)
                page = 1;

            int recsCount = categories.Count();
            var pager = new Pager(recsCount, page, pageSize);
            int recSkip = (page - 1) * pageSize;
            var data = categories.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name, IFormFile image)
        {
            Category category = new Category { Name = name };
            Category? lastCategory = _context.Categories.OrderBy(c => c.Id).LastOrDefault();

            string absolutePath;
            string relativePath;
            if (lastCategory != null)
            {
                absolutePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Categories", $"category{lastCategory.Id + 1}" + Path.GetExtension(image.FileName));
                relativePath = Path.Combine("..", "Images", "Categories", $"category{lastCategory.Id + 1}" + Path.GetExtension(image.FileName));
            }
            else
            {
                absolutePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Categories", $"category0" + Path.GetExtension(image.FileName));
                relativePath = Path.Combine("..", "Images", "Categories", $"category0" + Path.GetExtension(image.FileName));
            }

            category.ImagePathRelative = relativePath;
            category.ImagePathAbsolute = absolutePath;
            _context.Categories.Add(category);
            _context.SaveChanges();
    
            using (var stream = new FileStream(absolutePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            Category category = _context.Categories.FirstOrDefault(c => c.Id == id);
            System.IO.File.Delete(category.ImagePathAbsolute);
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateAsync(int id, string name, IFormFile image) 
        {
            Category category = _context.Categories.FirstOrDefault(c => c.Id == id);
            category.Name = name;

            if (image != null) 
            {
                string absolutePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Categories", $"category{category.Id}" + Path.GetExtension(image.FileName));
                string relativePath = Path.Combine("..", "Images", "Categories", $"category{category.Id}" + Path.GetExtension(image.FileName));

                System.IO.File.Delete(category.ImagePathAbsolute);

                category.ImagePathAbsolute = absolutePath;
                category.ImagePathRelative = relativePath;

                using (var stream = new FileStream(absolutePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
