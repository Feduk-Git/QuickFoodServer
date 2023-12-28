using System.ComponentModel.DataAnnotations;

namespace QuickFoodServer.Models
{
    public class OrdersView
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
        public decimal Price { get; set; }
        [Required]
        public string City { get; set; }
        public string? AdminEmail { get; set; }
    }
}
