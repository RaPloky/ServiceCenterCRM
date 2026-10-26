using Service_Store;
using Service_Center.Data;
using Service_Center.Models;
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
    }
}