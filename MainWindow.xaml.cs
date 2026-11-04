using Microsoft.EntityFrameworkCore;
using Service_Center.Data;
using Service_Center.Models;
using Service_Store;
using Service_Store.Helpers;
using Service_Store.Models;
using System.Windows;
using System.Windows.Controls;

namespace Service_Center
{
    public partial class MainWindow : Window
    {
        private Employee _currentEmployee;
        private List<Repair> _repairs = new();
        private List<Sale> _sales = new();
        private List<Customer> _customers = new();
        private List<CustomerDevice> _devices = new();

        /// <summary>
        /// Ініціалізує головне вікно для поточного працівника.
        /// </summary>
        public MainWindow(Employee employee)
        {
            InitializeComponent();
            _currentEmployee = employee;

            // Виводимо інформацію про поточного працівника у шапку
            txtUserInfo.Text = $"Працівник: {employee.FirstName} {employee.LastName} (Посада ID: {employee.PositionId})";

            // Ролеве розмежування доступу (RBAC):
            if (employee.PositionId == (int)PositionRole.SalesConsultant)
            {
                tabSuppliers.Visibility = Visibility.Collapsed;
                tabAdmin.Visibility = Visibility.Collapsed;
            }
            else if (employee.PositionId == (int)PositionRole.ServiceEngineer)
            {
                tabSales.Visibility = Visibility.Collapsed;
                tabSuppliers.Visibility = Visibility.Collapsed;
                tabAdmin.Visibility = Visibility.Collapsed;
            }
            else if (employee.PositionId != (int)PositionRole.Administrator)
            {
                tabAdmin.Visibility = Visibility.Collapsed;
            }

            LoadDashboard_Click(null, null);
        }

        /// <summary>
        /// Виконує вихід із системи та повертає до авторизації.
        /// </summary>
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // Кнопка виходу назад до вікна логіну
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }

        /// <summary>
        /// Завантажує список ремонтів.
        /// </summary>
        private void LoadRepairs_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                // Завантажуємо список ремонтів із бази даних
                _repairs = db.Repairs.ToList();
                ApplyRepairSearch();
            }
        }

        /// <summary>
        /// Оновлює фільтр ремонтів після зміни пошукового запиту.
        /// </summary>
        private void RepairSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyRepairSearch();
        }

        /// <summary>
        /// Фільтрує ремонти за номером квитанції.
        /// </summary>
        private void ApplyRepairSearch()
        {
            string searchTerm = NormalizeSearchTerm(txtRepairSearch?.Text);

            dgRepairs.ItemsSource = string.IsNullOrEmpty(searchTerm)
                ? _repairs
                : _repairs.Where(repair => NormalizeSearchTerm(repair.RepairNumber)
                    .Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
        }

        /// <summary>
        /// Відкриває вікно створення ремонту.
        /// </summary>
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

        /// <summary>
        /// Завантажує залишки товарів на складі.
        /// </summary>
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
        /// <summary>
        /// Завантажує список клієнтів.
        /// </summary>
        private void LoadCustomers_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                _customers = db.Customers.ToList();
                ApplyCustomerSearch();
            }
        }

        /// <summary>
        /// Оновлює фільтр клієнтів після зміни пошукового запиту.
        /// </summary>
        private void CustomerSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyCustomerSearch();
        }

        /// <summary>
        /// Фільтрує клієнтів за прізвищем.
        /// </summary>
        private void ApplyCustomerSearch()
        {
            string searchTerm = NormalizeSearchTerm(txtCustomerSearch?.Text);

            dgCustomers.ItemsSource = string.IsNullOrEmpty(searchTerm)
                ? _customers
                : _customers.Where(customer => NormalizeSearchTerm(customer.LastName)
                    .Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
        }

        /// <summary>
        /// Відкриває вікно додавання клієнта.
        /// </summary>
        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            AddCustomerWindow addWindow = new AddCustomerWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadCustomers_Click(sender, e);
            }
        }

        /// <summary>
        /// Завантажує пристрої клієнтів.
        /// </summary>
        private void LoadDevices_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                _devices = db.CustomerDevices
                    .Include(d => d.Customer) // Підтягуємо дані про власника пристрою
                    .ToList();

                ApplyDeviceCustomerSearch();
            }
        }

        /// <summary>
        /// Оновлює фільтр пристроїв після зміни пошукового запиту.
        /// </summary>
        private void DeviceCustomerSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyDeviceCustomerSearch();
        }

        /// <summary>
        /// Фільтрує пристрої за прізвищем власника.
        /// </summary>
        private void ApplyDeviceCustomerSearch()
        {
            string searchTerm = NormalizeSearchTerm(txtDeviceCustomerSearch?.Text);

            dgDevices.ItemsSource = string.IsNullOrEmpty(searchTerm)
                ? _devices
                : _devices.Where(device => device.Customer != null
                    && NormalizeSearchTerm(device.Customer.LastName)
                        .Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
        }

        /// <summary>
        /// Відкриває вікно додавання пристрою.
        /// </summary>
        private void AddDevice_Click(object sender, RoutedEventArgs e)
        {
            AddDeviceWindow addWindow = new AddDeviceWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadDevices_Click(sender, e);
            }
        }

        /// <summary>
        /// Відкриває вибраний ремонт для редагування.
        /// </summary>
        private void EditRepair_Click(object sender, RoutedEventArgs e)
        {
            // Отримуємо вибраний у таблиці ремонт
            var selectedRepair = dgRepairs.SelectedItem as Service_Store.Models.Repair;

            if (selectedRepair == null)
            {
                MessageBox.Show("Будь ласка, оберіть ремонт із таблиці для редагування!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EditRepairWindow editWindow = new EditRepairWindow(selectedRepair.RepairId, _currentEmployee);
            if (editWindow.ShowDialog() == true)
            {
                LoadRepairs_Click(sender, e);
            }
        }

        /// <summary>
        /// Завантажує історію продажів.
        /// </summary>
        private void LoadSales_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                _sales = db.Sales
                    .Include(s => s.Customer)     // Підтягуємо дані клієнта
                    .Include(s => s.Employee)     // Підтягуємо дані працівника
                    .OrderByDescending(s => s.SaleDate)
                    .ToList();

                ApplySalesCustomerSearch();
            }
        }

        /// <summary>
        /// Оновлює фільтр продажів після зміни пошукового запиту.
        /// </summary>
        private void SalesCustomerSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplySalesCustomerSearch();
        }

        /// <summary>
        /// Фільтрує продажі за прізвищем клієнта.
        /// </summary>
        private void ApplySalesCustomerSearch()
        {
            string searchTerm = NormalizeSearchTerm(txtSalesCustomerSearch?.Text);

            dgSales.ItemsSource = string.IsNullOrEmpty(searchTerm)
                ? _sales
                : _sales.Where(sale => sale.Customer != null
                    && NormalizeSearchTerm(sale.Customer.LastName)
                        .Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
        }

        /// <summary>
        /// Нормалізує пошуковий текст, видаляючи зайві пробіли.
        /// </summary>
        private static string NormalizeSearchTerm(string? value)
        {
            return value?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Відкриває вікно створення продажу.
        /// </summary>
        private void AddSale_Click(object sender, RoutedEventArgs e)
        {
            BonusSaleWindow saleWindow = new BonusSaleWindow(_currentEmployee.EmployeeId);
            if (saleWindow.ShowDialog() == true)
            {
                LoadSales_Click(null, null);
            }
        }

        /// <summary>
        /// Завантажує список постачальників.
        /// </summary>
        private void LoadSuppliersList_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                dgSuppliers.ItemsSource = db.Suppliers.ToList();
            }
        }

        /// <summary>
        /// Відкриває вікно додавання постачальника.
        /// </summary>
        private void AddSupplier_Click(object sender, RoutedEventArgs e)
        {
            AddSupplierWindow addWindow = new AddSupplierWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadSuppliersList_Click(sender, e);
                LoadSuppliers_Click(sender, e);
            }
        }

        /// <summary>
        /// Завантажує прайс-лист постачальників.
        /// </summary>
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

        /// <summary>
        /// Завантажує історію виплат зарплати.
        /// </summary>
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

        /// <summary>
        /// Відкриває вікно додавання виплати зарплати.
        /// </summary>
        private void AddSalary_Click(object sender, RoutedEventArgs e)
        {
            AddSalaryWindow salaryWindow = new AddSalaryWindow();
            if (salaryWindow.ShowDialog() == true)
            {
                LoadSalaries_Click(sender, e);
            }
        }

        /// <summary>
        /// Оновлює показники та дані панелі моніторингу.
        /// </summary>
        private void LoadDashboard_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                // 1. Звіт: Фінанси та продажі
                var salesReport = db.Sales
                    .Include(s => s.Customer)
                    .Include(s => s.Employee)
                    .OrderByDescending(s => s.SaleDate)
                    .ToList()
                    .Select(s => new
                    {
                        s.SaleId,
                        s.SaleNumber,
                        s.SaleDate,
                        EmployeeName = s.Employee != null ? $"{s.Employee.FirstName} {s.Employee.LastName}" : "Н/Д",
                        CustomerName = s.Customer != null ? $"{s.Customer.FirstName} {s.Customer.LastName}" : "Анонім",
                        s.TotalAmount,
                        s.BonusUsed,
                        PaymentMethods = string.Join(", ", db.Payments.Where(p => p.SaleId == s.SaleId).Select(p => p.PaymentMethod).Distinct())
                    })
                    .ToList();
                dgReportSales.ItemsSource = salesReport;

                // 2. Звіт: Зарплати та результативність (визначаємо посаду за PositionId)
                var employeesList = db.Employees.ToList();

                var payrollReport = employeesList.Select(emp =>
                {
                    int completedRepairs = db.Repairs.Count(r => r.AssignedEmployeeId == emp.EmployeeId);
                    decimal totalSales = db.Sales.Where(s => s.EmployeeId == emp.EmployeeId).Sum(s => (decimal?)s.TotalAmount) ?? 0;

                    string positionName = "Працівник";
                    decimal salary = 0;

                    // Приблизна логіка за ID посад (можеш підкоригувати цифри під свої дані)
                    if (emp.PositionId == (int)PositionRole.ServiceEngineer)
                    {
                        positionName = "Інженер / Майстер";
                        decimal repairsSum = db.Repairs.Where(r => r.AssignedEmployeeId == emp.EmployeeId).Sum(r => (decimal?)(r.FinalPrice ?? r.EstimatedPrice)) ?? 0;
                        salary = repairsSum * 0.30m; // 30% від ремонтів
                    }
                    else if (emp.PositionId == (int)PositionRole.SalesConsultant)
                    {
                        positionName = "Продавець";
                        salary = totalSales * 0.01m; // 1% від продажів
                    }
                    else if (emp.PositionId == (int)PositionRole.Administrator)
                    {
                        positionName = "Адміністратор";
                        salary = 15000; // Фіксована ставка
                    }

                    return new
                    {
                        emp.EmployeeId,
                        EmployeeName = $"{emp.FirstName} {emp.LastName}",
                        PositionName = positionName,
                        CompletedRepairs = completedRepairs,
                        TotalSalesAmount = totalSales,
                        CalculatedSalary = salary
                    };
                }).ToList();
                dgReportPayroll.ItemsSource = payrollReport;

                // 3. Звіт: Критичні залишки на складі
                var stocks = db.WarehouseStocks.ToList();
                var products = db.Products.ToList();

                var lowStockReport = stocks
                    .Where(w => w.Quantity <= w.MinimumQuantity || w.Quantity <= 3)
                    .Select(w =>
                    {
                        var prod = products.FirstOrDefault(p => p.ProductId == w.ProductId);
                        return new
                        {
                            w.ProductId,
                            SKU = prod != null ? prod.ProductId.ToString() : "-",
                            ProductName = prod?.Name ?? "Товар",
                            w.Quantity,
                            w.MinimumQuantity,
                            w.ReservedQuantity,
                            StatusText = w.Quantity == 0 ? "КРИТИЧНО (0 шт)" : "Потрібна закупка"
                        };
                    })
                    .ToList();
                dgReportLowStock.ItemsSource = lowStockReport;

                // 4. Звіт: Завантаженість та аналітика ремонтів
                var repairsList = db.Repairs
                    .Include(r => r.Customer)
                    .Include(r => r.AssignedEmployee)
                    .OrderByDescending(r => r.ReceivedAt)
                    .ToList();

                var devicesList = db.CustomerDevices.ToList();
                var statusesList = db.RepairStatuses.ToList();

                var repairsReport = repairsList.Select(r =>
                {
                    var dev = devicesList.FirstOrDefault(d => d.DeviceId == r.DeviceId);
                    var stat = statusesList.FirstOrDefault(s => s.StatusId == r.StatusId);

                    return new
                    {
                        r.RepairNumber,
                        DeviceInfo = dev != null ? $"{dev.Brand} {dev.Model}" : "Пристрій",
                        CustomerName = r.Customer != null ? $"{r.Customer.LastName} {r.Customer.FirstName}" : "-",
                        EngineerName = r.AssignedEmployee != null ? $"{r.AssignedEmployee.LastName} {r.AssignedEmployee.FirstName}" : "Не призначено",
                        StatusName = stat?.Name ?? "-",
                        FinalPrice = r.FinalPrice ?? r.EstimatedPrice,
                        r.ReceivedAt
                    };
                }).ToList();
                dgReportRepairs.ItemsSource = repairsReport;
            }
        }

        /// <summary>
        /// Експортує вибраний звіт із панелі моніторингу.
        /// </summary>
        private void ExportSelectedReport_Click(object sender, RoutedEventArgs e)
        {
            // Визначаємо, яка підвкладка зараз активна у Дашборді
            if (reportsTabControl.SelectedItem is TabItem selectedTab)
            {
                string header = selectedTab.Header?.ToString();

                if (selectedTab == tabReportSales)
                {
                    ExcelExporter.ExportDataGridToExcel(dgReportSales, "Звіт_Продажі_та_Фінанси");
                }
                else if (selectedTab == tabReportPayroll)
                {
                    ExcelExporter.ExportDataGridToExcel(dgReportPayroll, "Звіт_Зарплати_Персоналу");
                }
                else if (selectedTab == tabReportLowStock)
                {
                    ExcelExporter.ExportDataGridToExcel(dgReportLowStock, "Звіт_Дефіцит_Складу");
                }
                else if (selectedTab == tabReportRepairs)
                {
                    ExcelExporter.ExportDataGridToExcel(dgReportRepairs, "Звіт_Аналітика_Ремонтів");
                }
            }
        }

        /// <summary>
        /// Завантажує дані відповідно до вибраної вкладки.
        /// </summary>
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
                    else if (header == "Продажі")
                    {
                        LoadSales_Click(null, null);
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

        /// <summary>
        /// Відкриває вікно графіка роботи.
        /// </summary>
        private void OpenShifts_Click(object sender, RoutedEventArgs e)
        {
            EmployeeShiftsWindow shiftsWindow = new EmployeeShiftsWindow(_currentEmployee);
            shiftsWindow.ShowDialog();
        }

        /// <summary>
        /// Видаляє вибраний пристрій після перевірки залежностей.
        /// </summary>
        private void DeleteDevice_Click(object sender, RoutedEventArgs e)
        {
            // Отримуємо обраний пристрій із таблиці (DataGrid)
            // Зверни увагу: тип об'єкта залежить від того, як ти заповнюєш dgDevices (може бути анонімний тип або модель CustomerDevice)
            if (dgDevices.SelectedItem != null)
            {
                // Отримуємо ID пристрою через динамічне приведення або властивість моделі
                int deviceId = 0;
                string deviceName = "";

                if (dgDevices.SelectedItem is CustomerDevice device)
                {
                    deviceId = device.DeviceId;
                    deviceName = $"{device.Brand} {device.Model}";
                }
                else
                {
                    // Якщо використовується анонімний тип у завантаженні (Select)
                    var selectedItem = dgDevices.SelectedItem;
                    deviceId = (int)selectedItem.GetType().GetProperty("DeviceId").GetValue(selectedItem, null);
                    deviceName = $"{selectedItem.GetType().GetProperty("Brand").GetValue(selectedItem, null)} {selectedItem.GetType().GetProperty("Model").GetValue(selectedItem, null)}";
                }

                using (var db = new AppDbContext())
                {
                    // 1. Перевіряємо, чи є ремонти, пов'язані з цим пристроєм
                    bool hasRepairs = db.Repairs.Any(r => r.DeviceId == deviceId);

                    if (hasRepairs)
                    {
                        // Якщо ремонти є — виводимо попередження
                        MessageBox.Show("Цей пристрій наявний в історіях ремонту і не може бути видалений!",
                            "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // 2. Якщо ремонтів немає — виводимо діалогове вікно підтвердження
                    MessageBoxResult result = MessageBox.Show($"Ви дійсно хочете видалити пристрій: {deviceName}?",
                        "Підтвердження видалення", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        var deviceToDelete = db.CustomerDevices.Find(deviceId);
                        if (deviceToDelete != null)
                        {
                            db.CustomerDevices.Remove(deviceToDelete);
                            db.SaveChanges();
                        }

                        // Оновлюємо таблицю пристроїв на екрані
                        // Викликаємо твій існуючий метод завантаження (наприклад, LoadDevices_Click або аналогічний)
                        LoadDevices_Click(sender, e);

                        MessageBox.Show("Пристрій успішно видалено.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, оберіть пристрій у таблиці для видалення!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Друкує чек вибраного продажу.
        /// </summary>
        private void PrintReceipt_Click(object sender, RoutedEventArgs e)
        {
            if (dgSales.SelectedItem is Sale selectedSale)
            {
                DocumentPrinter.PrintSaleReceipt(selectedSale.SaleId);
            }
            else
            {
                MessageBox.Show("Оберіть продаж із таблиці для друку чека!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}