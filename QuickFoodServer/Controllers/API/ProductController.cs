using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickFoodServer.Models;
using QuickFoodServer.Models.DTO;
using System.Collections;

namespace QuickFoodServer.Controllers.API
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly Context _context;

        public ProductController(Context context)
        {
            _context = context;
        }

        //http://localhost:36985/api/product/get_popular_products/1
        [HttpGet("get_popular_products/{productsCount}")]
        public ICollection GetPopularProducts(int productsCount)
        {
            List<Product> products = _context.Products.Include(p => p.Categories).OrderByDescending(p => p.OrdersCount).Take(productsCount).ToList();
            List<ProductDto> outProducts = new List<ProductDto>();

            foreach (Product product in products) 
            {
                ProductDto productDto = new ProductDto();
                productDto.Id = product.Id;
                productDto.Name = product.Name;
                productDto.Description = product.Description;
                productDto.Price = product.Price;

                outProducts.Add(productDto);
            }

            return outProducts;
        }

        //http://localhost:36985/api/product/get_products_search/12
        [HttpGet("get_products_search/{searchString}")]
        public ICollection GetProductsSearch(string searchString)
        {
            List<Product> products = _context.SearchProducts(searchString);
            List<ProductDto> outProducts = new List<ProductDto>();

            foreach (Product product in products)
            {
                ProductDto productDto = new ProductDto();
                productDto.Id = product.Id;
                productDto.Name = product.Name;
                productDto.Description = product.Description;
                productDto.Price = product.Price;

                outProducts.Add(productDto);
            }

            return outProducts;
        }
    }
}
