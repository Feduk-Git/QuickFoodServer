using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuickFoodServer.Models;
using QuickFoodServer.Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QuickFoodServer.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly Context _context;

        public OrderController(Context context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1)
        {
            List<Admin> admins = _context.Admins.ToList();

            List<OrdersView> orders = null;

            if (User.IsInRole("Main administrator"))
            {
                orders = _context.OrdersViews.ToList();
            }
            else
            {
                string? cityName = _context.Admins.Include(a => a.City).Where(admin => admin.Email == User.Identity.Name)
                    .Select(admin => admin.City)
                    .FirstOrDefault()!.Name;

                if (cityName != null)
                {
                    orders = _context.OrdersViews.Where(ov => ov.City == cityName).ToList();
                }
            }


            const int pageSize = 10;
            if (page < 1)
                page = 1;

            int recsCount = orders.Count();
            var pager = new Pager(recsCount, page, pageSize);
            int recSkip = (page - 1) * pageSize;
            var data = orders.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(data);
        }

        public IActionResult ProductList(int id, int page = 1)
        {
            Order order = _context.Orders.Include(o => o.Products).ThenInclude(o => o.Categories).FirstOrDefault(o => o.Id == id);

            const int pageSize = 5;
            if (page < 1)
                page = 1;

            int recsCount = order.Products.Count();
            var pager = new Pager(recsCount, page, pageSize);
            int recSkip = (page - 1) * pageSize;
            
            List<Product> products = order.Products.Skip(recSkip).Take(pager.PageSize).ToList();
            List<int> productIds = products.Select(p => p.Id).ToList();
            List<OrdersHasProducts> ordersHasProducts = _context.OrdersHasProducts.Where(op => productIds.Contains(op.ProductsId)).ToList();

            List<ProductsListViewModel> data = new List<ProductsListViewModel>();

            foreach (Product item in products)
            {
                int countProducts = ordersHasProducts.FirstOrDefault(op => op.ProductsId == item.Id).CountProducts;

                data.Add(new ProductsListViewModel()
                {
                    Product = item,
                    Count = countProducts,
                });
            }

            ViewBag.Pager = pager;
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.OrderId = order.Id;

            return View(data);
        }

        public IActionResult Update(int id, string adminEmail, OrderStatus orderStatus)
        {
            int adminId = _context.Admins.FirstOrDefault(a => a.Email == adminEmail)!.Id;

            _context.UpdateOrderStatus(id, orderStatus, adminId);
            return RedirectToAction("Index");
        }
    }
}
