using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickFoodServer.Models.DTO;
using QuickFoodServer.Models;
using System.Collections;

namespace QuickFoodServer.Controllers.API
{
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly Context _context;

        public CategoryController(Context context)
        {
            _context = context;
        }

        [HttpGet("get_popular_categories/{categoriesCount}")]
        public ICollection GetPopularCategories(int categoriesCount)
        {
            List<Category> categories = _context.Categories.Include(c => c.Products).OrderByDescending(c => c.Products.Sum(p => p.OrdersCount)).Take(categoriesCount).ToList();
            List<CategoryDto> outCategories = new List<CategoryDto>();

            foreach (Category category in categories)
            {
                CategoryDto categoryDto = new CategoryDto();
                categoryDto.Id = category.Id;
                categoryDto.Name = category.Name;

                outCategories.Add(categoryDto);
            }

            return outCategories;
        }

        [HttpGet("get_categories")]
        public ICollection GetCategories()
        {
            List<Category> categories = _context.Categories.Include(c => c.Products).OrderByDescending(c => c.Products.Sum(p => p.OrdersCount)).ToList();
            List<CategoryDto> outCategories = new List<CategoryDto>();

            foreach (Category category in categories)
            {
                CategoryDto categoryDto = new CategoryDto();
                categoryDto.Id = category.Id;
                categoryDto.Name = category.Name;

                outCategories.Add(categoryDto);
            }

            return outCategories;
        }
    }
}
