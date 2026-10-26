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
    }
}