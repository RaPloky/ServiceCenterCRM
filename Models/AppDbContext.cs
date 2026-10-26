using Microsoft.EntityFrameworkCore;
using Service_Store.Models;
using Service_Center.Models;

namespace Service_Center.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Repair> Repairs { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<WarehouseStock> WarehouseStocks { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Заміни рядок підключення на свої дані (Server, Database, User ID, Password)
            string connectionString = "Server=localhost;Database=service_center_db;Uid=root;Pwd=;";

            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
    }
}