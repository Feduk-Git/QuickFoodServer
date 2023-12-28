using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickFoodServer.Models
{
    [Table("Order")]
    public class Order
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Surname { get; set; } = string.Empty;
        [Required]
        public string Address { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.WAITING_CONFIRM;
        [Required]
        public DateTime DateTime { get; set; }
        [Required]
        public ICollection<Product> Products { get; set; } = new List<Product>();
        [Required]
        public decimal Price { get; set; }
        [Required]
        public City? City { get; set; }
        public Admin? Admin { get; set; }
    }
}
