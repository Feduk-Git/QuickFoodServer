using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickFoodServer.Models
{
    [Table("Category")]
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string ImagePathRelative { get; set; } = string.Empty;
        [Required]
        public string ImagePathAbsolute { get; set; } = string.Empty;
        [Required]
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
