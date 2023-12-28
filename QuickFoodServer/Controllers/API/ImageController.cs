using Microsoft.AspNetCore.Mvc;
using QuickFoodServer.Models;
using System.Net;

namespace QuickFoodServer.Controllers.API
{
    [Route("api/[controller]")]
    public class ImageController : Controller
    {
        private readonly Context _context;

        public ImageController(Context context)
        {
            _context = context;
        }

        [HttpGet("get_category_image/{id}")]
        public IActionResult getCattegoryImage(int id)
        {
            Category category = _context.Categories.FirstOrDefault(c => c.Id == id);

            byte[] imageBytes = System.IO.File.ReadAllBytes(category.ImagePathAbsolute);

            return File(imageBytes, "image/jpeg");
        }

        [HttpGet("get_product_image/{id}")]
        public IActionResult getProductImage(int id)
        {
            Product product = _context.Products.FirstOrDefault(p => p.Id == id);

            byte[] imageBytes = System.IO.File.ReadAllBytes(product.ImagePathAbsolute);

            return File(imageBytes, "image/jpeg");
        }
    }
}
