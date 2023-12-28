using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QuickFoodServer.Models
{
    public class OrdersHasProducts
    {
        public int ProductsId { get; set; }
        public int OrdersId { get; set; }
        [Required]
        public int CountProducts { get; set; }
    }
}
