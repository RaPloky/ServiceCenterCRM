using Microsoft.EntityFrameworkCore;
using Service_Center.Data;
using Service_Center.Models;
using Service_Store;
using Service_Store.Models;
using System.Windows;
using System.Windows.Controls;

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

            // Ролеве розмежування доступу (RBAC):
            if (employee.PositionId == 2) // Продавець-консультант
            {
                // Ховаємо ремонтні та адмін-вкладки
                tabRepairs.Visibility = Visibility.Collapsed;
                tabDevices.Visibility = Visibility.Collapsed;
                tabSuppliers.Visibility = Visibility.Collapsed;
                tabAdmin.Visibility = Visibility.Collapsed;
            }
            else if (employee.PositionId == 3) // Інженер сервісного центру
            {
                // Ховаємо продажі, магазин та постачальників/адмін
                tabSales.Visibility = Visibility.Collapsed;
                tabStore.Visibility = Visibility.Collapsed;
                tabSuppliers.Visibility = Visibility.Collapsed;
                tabAdmin.Visibility = Visibility.Collapsed;
            }
            else if (employee.PositionId != 1) // Будь-яка інша / невідома посада
            {
                tabAdmin.Visibility = Visibility.Collapsed;
            }

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
            var selectedProduct = dgProducts.SelectedItem as Service_Store.Models.Product;

            if (selectedProduct == null)
            {
                MessageBox.Show("Будь ласка, виберіть товар із каталогу для продажу!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                // Візьмемо для прикладу клієнта з ID = 1 (або того, якого виберемо)
                int customerId = 1;
                var customer = db.Customers.Find(customerId);

                decimal saleAmount = selectedProduct.SalePrice;
                decimal bonusesEarned = saleAmount * 0.01m; // Нараховуємо 1% від суми покупки

                // 1. Створюємо запис про продаж
                var newSale = new Sale
                {
                    EmployeeId = _currentEmployee.EmployeeId,
                    CustomerId = customerId,
                    SaleDate = DateTime.Now,
                    TotalAmount = saleAmount,
                    BonusUsed = 0 // Поки без списання, у наступному кроці додамо діалог використання балів
                };

                db.Sales.Add(newSale);
                db.SaveChanges(); // Зберігаємо, щоб отримати SaleId

                // 2. Додаємо позицію до чека
                var saleItem = new SaleItem
                {
                    SaleId = newSale.SaleId,
                    ProductId = selectedProduct.ProductId,
                    Quantity = 1,
                    UnitPrice = saleAmount
                };
                db.SaleItems.Add(saleItem);

                // 3. Нараховуємо бонуси клієнту, якщо він існує
                if (customer != null)
                {
                    customer.BonusBalance += bonusesEarned;

                    // Записуємо транзакцію нарахування бонусів
                    var bonusTransaction = new BonusTransaction
                    {
                        CustomerId = customerId,
                        SaleId = newSale.SaleId,
                        TransactionType = "earn",
                        Amount = bonusesEarned,
                        Description = $"Нарахування 1% за покупку №{newSale.SaleId}",
                        CreatedAt = DateTime.Now
                    };
                    db.BonusTransactions.Add(bonusTransaction);
                }

                db.SaveChanges();

                MessageBox.Show($"Продаж успішно оформлено!\nТовар: {selectedProduct.Name}\nНараховано бонусів: {bonusesEarned:N2} грн", "Каса", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadSales_Click(null, null);
            }
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
            BonusSaleWindow saleWindow = new BonusSaleWindow(_currentEmployee.EmployeeId);
            if (saleWindow.ShowDialog() == true)
            {
                LoadSales_Click(null, null);
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

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Захист, щоб подія не спрацьовувала при виборі підвкладок всередині самих вкладок
            if (e.Source is TabControl tabControl && tabControl == mainTabControl)
            {
                // Перевіряємо, яка саме вкладка вибрана за її заголовком (Header)
                if (tabControl.SelectedItem is TabItem selectedTab)
                {
                    string header = selectedTab.Header?.ToString();

                    // Викликаємо відповідний метод завантаження залежно від активної вкладки
                    if (header == "Дашборд")
                    {
                        LoadDashboard_Click(null, null);
                    }
                    else if (header == "Ремонти (СЦ)")
                    {
                        LoadRepairs_Click(null, null);
                    }
                    else if (header == "Продажі" || header == "Магазин / Продажі")
                    {
                        LoadSales_Click(null, null);
                        LoadProducts_Click(null, null); // Оновлюємо і каталог товарів на випадок змін
                    }
                    else if (header == "Клієнти")
                    {
                        LoadCustomers_Click(null, null);
                    }
                    else if (header == "Склад")
                    {
                        LoadWarehouse_Click(null, null);
                    }
                    else if (header == "Пристрої клієнтів")
                    {
                        LoadDevices_Click(null, null);
                    }
                    else if (header == "Постачальники")
                    {
                        LoadSuppliersList_Click(null, null);
                        LoadSuppliers_Click(null, null);
                    }
                    else if (header == "Адміністрування")
                    {
                        LoadSalaries_Click(null, null);
                    }
                }
            }
        }
    }
}