using Service_Center.Data;
using Service_Store.Models;
using System;
using System.Windows;

namespace Service_Store
{
    public partial class AddRepairWindow : Window
    {
        private int _employeeId;

        public AddRepairWindow(int employeeId)
        {
            InitializeComponent();
            _employeeId = employeeId;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDescription.Text) || string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Будь ласка, заповніть усі поля!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price))
            {
                MessageBox.Show("Введіть коректну ціну (число)!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var db = new AppDbContext())
            {
                var newRepair = new Repair
                {
                    RepairNumber = $"REP-{DateTime.Now:yyMMdd-mmss}",
                    CustomerId = 1, // Поки беремо базового клієнта, пізніше зробимо вибір із випадаючого списку
                    DeviceId = 1,   // Базовий пристрій клієнта
                    CreatedBy = _employeeId,
                    StatusId = 1,   // "Прийнято в СЦ"
                    CustomerDescription = txtDescription.Text.Trim(),
                    EstimatedPrice = price,
                    ReceivedAt = DateTime.Now
                };

                db.Repairs.Add(newRepair);
                db.SaveChanges();
            }

            MessageBox.Show("Ремонт успішно створено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true; // Закриваємо вікно з сигналом успіху
            this.Close();
        }
    }
}