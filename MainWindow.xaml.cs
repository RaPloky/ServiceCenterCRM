using Microsoft.EntityFrameworkCore;
using Service_Center.Data;
using Service_Center.Models;
using Service_Store;
using Service_Store.Models;
using System.Windows;

namespace Service_Center
{
    public partial class MainWindow : Window
    {
        private Employee _currentEmployee;

        public MainWindow(Employee employee)
        {
            InitializeComponent();
            _currentEmployee = employee;

            // Виводимо інформацію про поточного працівника у шапку
            txtUserInfo.Text = $"Працівник: {employee.FirstName} {employee.LastName} (Посада ID: {employee.PositionId})";

            // Приклад реалізації розмежування доступу (RBAC):
            // Якщо посада не Адміністратор (наприклад, PositionId != 1), приховуємо вкладку адміністрування
            if (employee.PositionId != 1)
            {
                tabAdmin.Visibility = Visibility.Collapsed;
            }
            LoadRepairs_Click(null, null);
            LoadDashboard_Click(null, null);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // Кнопка виходу назад до вікна логіну
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private void LoadRepairs_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                // Завантажуємо список ремонтів із бази даних
                var repairsList = db.Repairs.ToList();
                dgRepairs.ItemsSource = repairsList;
            }
        }

        private void AddRepair_Click(object sender, RoutedEventArgs e)
        {
            // Відкриваємо вікно створення ремонту, передаючи ID поточного працівника
            AddRepairWindow addWindow = new AddRepairWindow(_currentEmployee.EmployeeId);
            if (addWindow.ShowDialog() == true)
            {
                // Якщо ремонт успішно збережено, оновлюємо таблицю
                LoadRepairs_Click(sender, e);
            }
        }

        private void LoadProducts_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var productsList = db.Products.ToList();
                dgProducts.ItemsSource = productsList;
            }
        }

        private void CreateSale_Click(object sender, RoutedEventArgs e)
        {
            // Отримуємо товар, який користувач виділив у таблиці
            var selectedProduct = dgProducts.SelectedItem as Service_Store.Models.Product;

            if (selectedProduct == null)
            {
                MessageBox.Show("Будь ласка, виберіть товар із каталогу для продажу!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                // 1. Створюємо запис про продаж
                var newSale = new Sale
                {
                    EmployeeId = _currentEmployee.EmployeeId, // Заточено під поточного залогіненого працівника
                    CustomerId = 1, // Базовий клієнт для прикладу
                    SaleDate = DateTime.Now,
                    TotalAmount = selectedProduct.SalePrice
                };

                db.Sales.Add(newSale);
                db.SaveChanges(); // Зберігаємо, щоб отримати згенерований SaleId

                // 2. Додаємо позицію до чек-листа продажу
                var saleItem = new SaleItem
                {
                    SaleId = newSale.SaleId,
                    ProductId = selectedProduct.ProductId,
                    Quantity = 1,
                    UnitPrice = selectedProduct.SalePrice
                };

                db.SaleItems.Add(saleItem);
                db.SaveChanges();
            }

            MessageBox.Show($"Успішно оформлено продаж товару: {selectedProduct.Name} на суму {selectedProduct.SalePrice} грн!", "Каса", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadWarehouse_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var stockList = db.WarehouseStocks
                    .Include(w => w.Product) // Підтягуємо зв'язаний товар за зовнішнім ключем
                    .ToList();

                dgWarehouse.ItemsSource = stockList;
            }
        }
        private void LoadCustomers_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var customersList = db.Customers.ToList();
                dgCustomers.ItemsSource = customersList;
            }
        }

        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            AddCustomerWindow addWindow = new AddCustomerWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadCustomers_Click(sender, e);
            }
        }

        private void LoadDevices_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var devicesList = db.CustomerDevices
                    .Include(d => d.Customer) // Підтягуємо дані про власника пристрою
                    .ToList();

                dgDevices.ItemsSource = devicesList;
            }
        }

        private void AddDevice_Click(object sender, RoutedEventArgs e)
        {
            AddDeviceWindow addWindow = new AddDeviceWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadDevices_Click(sender, e);
            }
        }

        private void EditRepair_Click(object sender, RoutedEventArgs e)
        {
            // Отримуємо вибраний у таблиці ремонт
            var selectedRepair = dgRepairs.SelectedItem as Service_Store.Models.Repair;

            if (selectedRepair == null)
            {
                MessageBox.Show("Будь ласка, оберіть ремонт із таблиці для редагування!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EditRepairWindow editWindow = new EditRepairWindow(selectedRepair.RepairId);
            if (editWindow.ShowDialog() == true)
            {
                LoadRepairs_Click(sender, e);
            }
        }

        private void LoadSales_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                dgSales.ItemsSource = db.Sales.OrderByDescending(s => s.SaleDate).ToList();
            }
        }

        private void AddSale_Click(object sender, RoutedEventArgs e)
        {
            CreateSaleWindow saleWindow = new CreateSaleWindow();
            if (saleWindow.ShowDialog() == true)
            {
                LoadSales_Click(sender, e);
            }
        }

        private void LoadSuppliersList_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                dgSuppliers.ItemsSource = db.Suppliers.ToList();
            }
        }

        private void AddSupplier_Click(object sender, RoutedEventArgs e)
        {
            AddSupplierWindow addWindow = new AddSupplierWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadSuppliersList_Click(sender, e);
                LoadSuppliers_Click(sender, e);
            }
        }

        private void LoadSuppliers_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var supplierProductsList = db.SupplierProducts
                    .Include(sp => sp.Supplier)
                    .Include(sp => sp.Product)
                    .ToList();

                dgSupplierProducts.ItemsSource = supplierProductsList;
            }
        }

        private void LoadSalaries_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                dgSalaries.ItemsSource = db.SalaryRecords
                    .Include(sr => sr.Employee)
                    .OrderByDescending(sr => sr.PaymentDate)
                    .ToList();
            }
        }

        private void AddSalary_Click(object sender, RoutedEventArgs e)
        {
            AddSalaryWindow salaryWindow = new AddSalaryWindow();
            if (salaryWindow.ShowDialog() == true)
            {
                LoadSalaries_Click(sender, e);
            }
        }

        private void LoadDashboard_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                // 1. Рахуємо загальну суму всіх продажів
                decimal totalSales = db.Sales.Sum(s => (decimal?)s.TotalAmount) ?? 0;
                txtTotalSales.Text = $"{totalSales:N2} грн";

                // 2. Рахуємо загальну кількість ремонтів
                int repairsCount = db.Repairs.Count();
                txtActiveRepairs.Text = repairsCount.ToString();

                // 3. Знаходимо товари з критичним залишком (менше або дорівнює 3)
                var lowStockItems = db.WarehouseStocks
                    .Include(w => w.Product)
                    .Where(w => w.Quantity <= 3)
                    .ToList();

                txtLowStock.Text = lowStockItems.Count.ToString();
                dgLowStock.ItemsSource = lowStockItems;
            }
        }
    }
}