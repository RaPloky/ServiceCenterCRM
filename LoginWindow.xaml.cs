using System.Linq;
using System.Windows;
using Service_Center.Data;

namespace Service_Center
{
    public partial class LoginWindow : Window
    {
        /// <summary>
        /// Ініціалізує вікно авторизації.
        /// </summary>
        public LoginWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Перевіряє облікові дані та відкриває головне вікно.
        /// </summary>
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
                    // Замість старого MessageBox.Show(...) успішного входу робимо так:
                    MainWindow mainWindow = new MainWindow(employee);
                    mainWindow.Show();
                    this.Close(); // Закриваємо вікно авторизації
                }
                else
                {
                    MessageBox.Show("Невірний логін або пароль!", "Помилка входу", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}