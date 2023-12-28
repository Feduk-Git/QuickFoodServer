using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickFoodServer.Models;
using QuickFoodServer.Utils;

namespace QuickFoodServer.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly Context _context;

        public AdminController(Context context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1)
        {
            List<Admin> admins = _context.Admins.Include(a => a.Role).Include(a => a.City).ToList();

            const int pageSize = 10;
            if (page < 1)
                page = 1;

            int recsCount = admins.Count();
            var pager = new Pager(recsCount, page, pageSize);
            int recSkip = (page - 1) * pageSize;
            var data = admins.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;
            ViewBag.Cities = _context.Cities;
            ViewBag.Roles = _context.Roles;

            return View(data);
        }

        public IActionResult Create(string name, string surname, string email, string password, int roleId, int cityId)
        {
            Admin existedAdmin = _context.Admins.FirstOrDefault(a => a.Email == email)!;
            if (existedAdmin != null)
            {
                TempData["ErrorMessage"] = "Error! Administrator with this email address is already exists";
                return RedirectToAction("Index");
            }

            City city = _context.Cities.FirstOrDefault(c => c.Id == cityId);
            Role role = _context.Roles.FirstOrDefault(r => r.Id == roleId);

            Admin admin = new Admin
            {
                Name = name,
                Surname = surname,
                Email = email,
                Password = password,
                City = city,
                Role = role
            };

            _context.Admins.Add(admin);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Update(int id, string email, string name, string surname, int roleId, int cityId) 
        {
            Admin admin = _context.Admins.FirstOrDefault(a => a.Id == id);
            City city = _context.Cities.FirstOrDefault(c => c.Id == cityId);
            Role role = _context.Roles.FirstOrDefault(r => r.Id == roleId);

            admin.Email = email;
            admin.Name = name;
            admin.Surname = surname;
            admin.Role = role;
            admin.City = city;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            Admin admin = _context.Admins.FirstOrDefault(a => a.Id == id);

            _context.Admins.Remove(admin);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
