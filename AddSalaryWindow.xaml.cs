using Service_Center.Data;
using Service_Store.Models;
using System;
using System.Linq;
using System.Windows;

namespace Service_Store
{
    public partial class AddSalaryWindow : Window
    {
        /// <summary>
        /// Ініціалізує вікно додавання виплати зарплати.
        /// </summary>
        public AddSalaryWindow()
        {
            InitializeComponent();
            LoadEmployees();
        }

        /// <summary>
        /// Завантажує працівників до списку вибору.
        /// </summary>
        private void LoadEmployees()
        {
            using (var db = new AppDbContext())
            {
                cbEmployees.ItemsSource = db.Employees.ToList();
            }
        }

        /// <summary>
        /// Перевіряє та зберігає виплату зарплати.
        /// </summary>
        private void SaveSalary_Click(object sender, RoutedEventArgs e)
        {
            if (cbEmployees.SelectedValue == null || string.IsNullOrWhiteSpace(txtAmount.Text) || string.IsNullOrWhiteSpace(txtPeriod.Text))
            {
                MessageBox.Show("Заповніть обов'язкові поля (Працівник, Сума, Період)!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Введіть коректну суму виплати!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                var record = new SalaryRecord
                {
                    EmployeeId = (int)cbEmployees.SelectedValue,
                    Amount = amount,
                    Period = txtPeriod.Text.Trim(),
                    PaymentDate = DateTime.Now,
                    Status = string.IsNullOrWhiteSpace(txtNotes.Text) ? "Виплачено" : txtNotes.Text.Trim()
                };

                db.SalaryRecords.Add(record);
                db.SaveChanges();
            }

            MessageBox.Show("Виплату зарплати успішно зареєстровано!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }
    }
}