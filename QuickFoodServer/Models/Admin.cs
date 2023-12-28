using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickFoodServer.Models
{
    [Table("Admin")]
    public class Admin
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Surname { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public Role? Role { get; set; }
        [Required]
        public City? City { get; set; }
        [Required]
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
