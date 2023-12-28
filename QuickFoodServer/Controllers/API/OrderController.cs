using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickFoodServer.Models;
using QuickFoodServer.Models.DTO;
using System.Text.Json;

namespace QuickFoodServer.Controllers.API
{
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private readonly Context _context;

        public OrderController(Context context)
        {
            _context = context;
        }

        [HttpPost("create_new_order")]
        public IActionResult CreateNewOrder([FromBody] JsonDocument requestBody)
        {
            string name = requestBody.RootElement.GetProperty("name").GetString();
            string surname = requestBody.RootElement.GetProperty("surname").GetString();
            string address = requestBody.RootElement.GetProperty("address").GetString();
            string phoneNumber = requestBody.RootElement.GetProperty("phoneNumber").GetString();
            string description = requestBody.RootElement.GetProperty("description").GetString();
            int cityId = requestBody.RootElement.GetProperty("cityId").GetInt32();
            List<ProductCount> productCountsList = JsonSerializer.Deserialize<List<ProductCount>>(requestBody.RootElement.GetProperty("productsCount").GetString());

            List<int> productIds = productCountsList.Select(p => p.ProductId).ToList();
            List<Product> products = _context.Products.Where(p => productIds.Contains(p.Id)).ToList();
            Admin admin = _context.Admins.FirstOrDefault(a => a.Id == 1);

            City city = _context.Cities.FirstOrDefault(c => c.Id == cityId);

            Order order = new Order()
            {
                Name = name,
                Surname = surname,
                Address = address,
                PhoneNumber = phoneNumber,
                Description = description,
                City = city,
                Products = products,
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            List<OrdersHasProducts> ordersHasProducts = _context.OrdersHasProducts.Where(ohp => ohp.OrdersId == order.Id).ToList();

            foreach (OrdersHasProducts item in ordersHasProducts)
            {
                ProductCount productCount = productCountsList.FirstOrDefault(pc => pc.ProductId == item.ProductsId);

                if (productCount != null)
                {
                    item.CountProducts = productCount.Count;
                }
            }
            _context.SaveChanges();

            List<CartItemDto> cartItemDtos = new List<CartItemDto>();

            foreach (Product product in products)
            {
                int productsCount = productCountsList.FirstOrDefault(pc => pc.ProductId == product.Id).Count;
                cartItemDtos.Add(new CartItemDto()
                {
                    ProductDto = new ProductDto()
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                    },
                    ItemsCount = productsCount,
                    Price = product.Price * productsCount,
                });
            }

            decimal totalPrice = 0;
            foreach (CartItemDto cartItemDto in cartItemDtos)
            {
                totalPrice += cartItemDto.Price;
            }

            OrderDto orderDto = new OrderDto()
            {
                Id = order.Id,
                Name = order.Name,
                Surname = order.Surname,
                Address = order.Address,
                PhoneNumber = order.PhoneNumber,
                Description = order.Description,
                DateTime = DateTime.Now.ToString("HH:mm dd.MM.yyyy"),
                CartItems = cartItemDtos,
                City = order.City,
                Price = totalPrice,
            };

            return Ok(orderDto);
        }

        [HttpGet("get_order_state_by_id/{orderId}")]
        public OrderDto GetOrderById(int orderId)
        {
            Order order = _context.Orders.Include(o => o.City).Include(o => o.Products)
                .FirstOrDefault(o => o.Id == orderId)!;

            if (order == null)
                return null;

            List<CartItemDto> cartItemDtos = new List<CartItemDto>();

            foreach (Product product in order.Products)
            {
                OrdersHasProducts orderProduct = _context.OrdersHasProducts
                    .Where(op => op.ProductsId == product.Id && op.OrdersId == order.Id).FirstOrDefault()!;

                if (orderProduct != null)
                {
                    ProductDto productDto = new ProductDto()
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                    };

                    CartItemDto cartItemDto = new CartItemDto()
                    {
                        ProductDto = productDto,
                        ItemsCount = orderProduct.CountProducts,
                        Price = productDto.Price * orderProduct.CountProducts
                    };

                    cartItemDtos.Add(cartItemDto);
                }
            }

            OrderDto orderDto = new OrderDto()
            {
                Id = order.Id,
                Name = order.Name,
                Surname = order.Surname,
                Address = order.Address,
                PhoneNumber = order.PhoneNumber,
                Description = order.Description,
                DateTime = order.DateTime.ToString("HH:mm dd.MM.yyyy"),
                Status = order.Status,
                CartItems = cartItemDtos,
                City = order.City,
                Price = order.Price
            };

            return orderDto;
        }
    }
}
