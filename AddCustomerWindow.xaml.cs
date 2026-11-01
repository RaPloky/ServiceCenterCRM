using Service_Center.Data;
using Service_Store.Models;
using System.Windows;

namespace Service_Store
{
    public partial class AddCustomerWindow : Window
    {
        /// <summary>
        /// Ініціалізує вікно додавання клієнта.
        /// </summary>
        public AddCustomerWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Перевіряє та зберігає нового клієнта.
        /// </summary>
        private void SaveCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Будь ласка, заповніть обов'язкові поля (Прізвище, Ім'я, Телефон)!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                var newCustomer = new Customer
                {
                    LastName = txtLastName.Text.Trim(),
                    FirstName = txtFirstName.Text.Trim(),
                    MiddleName = txtMiddleName.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    BonusBalance = 0.00m
                };

                db.Customers.Add(newCustomer);
                db.SaveChanges();
            }

            MessageBox.Show("Клієнта успішно додано!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }
    }
}