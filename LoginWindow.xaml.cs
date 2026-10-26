using System.Linq;
using System.Windows;
using ServiceCenterCRM.Data;

namespace ServiceCenterCRM
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Будь ласка, заповніть усі поля!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                // Шукаємо працівника в базі за логіном і паролем
                var employee = db.Employees
                    .FirstOrDefault(emp => emp.Username == username && emp.PasswordHash == password);

                if (employee != null)
                {
                    MessageBox.Show($"Успішний вхід! Вітаємо, {employee.FirstName} {employee.LastName}.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Тут пізніше ми відкриватимемо головне вікно (Dashboard) і передаватимемо туди об'єкт employee для ролей
                    // MainWindow main = new MainWindow(employee);
                    // main.Show();
                    // this.Close();
                }
                else
                {
                    MessageBox.Show("Невірний логін або пароль!", "Помилка входу", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}