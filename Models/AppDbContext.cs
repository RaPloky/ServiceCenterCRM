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
        public DbSet<CustomerDevice> CustomerDevices { get; set; }
        public DbSet<RepairStatus> RepairStatuses { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierProduct> SupplierProducts { get; set; }
        public DbSet<SalaryRecord> SalaryRecords { get; set; }
        public DbSet<BonusTransaction> BonusTransactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Server=localhost;Database=service_center_db;Uid=root;Pwd=;";

            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
    }
}