using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickFoodServer.Models;
using QuickFoodServer.Utils;

namespace QuickFoodServer.Controllers
{
    [Authorize]
    public class CityController : Controller
    {
        private readonly Context _context;

        public CityController(Context context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1)
        {
            List<City> cities = _context.Cities.ToList();

            const int pageSize = 10;
            if (page < 1)
                page = 1;

            int recsCount = cities.Count();
            var pager = new Pager(recsCount, page, pageSize);
            int recSkip = (page - 1) * pageSize;
            var data = cities.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(data);
        }

        public IActionResult Create(string name)
        {
            City city = new City{ Name = name };
            _context.Cities.Add(city);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id) 
        {
            City city = _context.Cities.FirstOrDefault(c => c.Id == id);
            _context.Cities.Remove(city);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {

            }

            return RedirectToAction("Index");
        }

        public IActionResult Update(int id, string name)
        {
            City city = _context.Cities.FirstOrDefault(c => c.Id == id);
            city.Name = name;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
