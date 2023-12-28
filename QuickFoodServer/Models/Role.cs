using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickFoodServer.Models
{
    [Table("Role")]
    public class Role
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public ICollection<Admin> Admins { get; set; } = new List<Admin>();
    }
}
