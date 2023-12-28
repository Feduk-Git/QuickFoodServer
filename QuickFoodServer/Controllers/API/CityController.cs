using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickFoodServer.Models.DTO;
using QuickFoodServer.Models;
using System.Collections;

namespace QuickFoodServer.Controllers.API
{
    [Route("api/[controller]")]
    public class CityController : Controller
    {
        private readonly Context _context;

        public CityController(Context context)
        {
            _context = context;
        }

        [HttpGet("get_cities")]
        public ICollection GetCities()
        {
            return _context.Cities.ToList();
        }
    }
}
