using System.Windows;
using ServiceCenterCRM.Models;

namespace ServiceCenterCRM
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
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // Кнопка виходу назад до вікна логіну
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}