using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using QuickFoodServer.Models;
using Microsoft.EntityFrameworkCore;

namespace QuickFoodServer.Controllers
{
    public class LoginController : Controller
    {
        private Context _context;

        public LoginController(Context context)
        {
            this._context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Error! Enter email and password";
                return RedirectToAction("Index");
            }

            Admin admin = _context.VerifyPassword(email, password);

            if (admin == null)
            {
                TempData["ErrorMessage"] = "Error! Invalid email or password";
                return RedirectToAction("Index");
            }

            admin = _context.Admins.Include(a => a.Role).FirstOrDefault(a => a.Id == admin.Id)!;

            await Authenticate(admin);
            return RedirectToAction("Index", "Order");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        private async Task Authenticate(Admin admin)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, admin.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, admin.Role.Name)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
