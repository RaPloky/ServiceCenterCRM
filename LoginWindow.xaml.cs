using System.Linq;
using System.Windows;
using Service_Center.Data;
using Service_Center.Security;

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

            try
            {
                using (var db = new AppDbContext())
                {
                    var employee = db.Employees
                        .FirstOrDefault(emp => emp.Username == username);

                    if (employee != null && PasswordHasher.VerifyPassword(password, employee.PasswordHash))
                    {
                        if (PasswordHasher.NeedsRehash(employee.PasswordHash))
                        {
                            employee.PasswordHash = PasswordHasher.HashPassword(password);
                            db.SaveChanges();
                        }

                        MainWindow mainWindow = new MainWindow(employee);
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Невірний логін або пароль!", "Помилка входу", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Service_Store.Helpers.DatabaseErrorHandler.Show(ex, "Не вдалося виконати авторизацію.");
            }
        }
    }
}