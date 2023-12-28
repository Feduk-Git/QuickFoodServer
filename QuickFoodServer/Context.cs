using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MySqlConnector;
using QuickFoodServer.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QuickFoodServer
{
    public class Context : DbContext
    {
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<OrdersHasProducts> OrdersHasProducts { get; set; }
        public DbSet<OrdersView> OrdersViews { get; set; }

        public Context(DbContextOptions<Context> options) : base(options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
            //Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .HasMany(e => e.Products)
                .WithMany(e => e.Orders)
                .UsingEntity<OrdersHasProducts>();

            modelBuilder.Entity<Admin>().HasIndex(e => e.Email).IsUnique();

            modelBuilder.Entity<OrdersView>().HasNoKey().ToView(nameof(OrdersView));

            base.OnModelCreating(modelBuilder);
        }

        public List<Product> SearchProducts (string SearchString)
        {
            MySqlParameter parametr = new MySqlParameter("@SearchString", SearchString);
            List<Product> products = Set<Product>().FromSqlRaw("CALL SearchProducts(?)", parametr).ToList();

            foreach (Product product in products)
            {
                Entry(product)
                    .Collection(p => p.Categories)
                    .Load();
            }

            return products;
        }

        public void UpdateOrderStatus (int orderId, OrderStatus newStatus, int adminId)
        {
            MySqlParameter[] parametrs = 
            {
                new MySqlParameter("@orderId", orderId),
                new MySqlParameter("@newStatus", newStatus),
                new MySqlParameter("@adminId", adminId),
            };

            Database.ExecuteSqlRaw("CALL UpdateOrderStatus(?, ?, ?)", parametrs);
        }

        public Admin VerifyPassword(string email, string password)
        {
            MySqlParameter[] parametrs =
            {
                new MySqlParameter("@input_email", email),
                new MySqlParameter("@input_password", password),
            };

            Admin admin;
            try
            {
                admin = Set<Admin>().FromSqlRaw("CALL VerifyAdminPassword(?, ?)", parametrs)
                                       .AsEnumerable()
                                       .FirstOrDefault()!;
            }
            catch (Exception)
            {
                admin = null!;
            }

            return admin;
        }
    }
}
