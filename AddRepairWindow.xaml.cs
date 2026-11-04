using Service_Center.Data;
using Service_Store.Helpers;
using Service_Store.Models;
using System;
using System.Linq;
using System.Windows;

namespace Service_Store
{
    public partial class AddRepairWindow : Window
    {
        private int _employeeId;

        /// <summary>
        /// Ініціалізує вікно створення ремонту для працівника.
        /// </summary>
        public AddRepairWindow(int employeeId)
        {
            InitializeComponent();
            _employeeId = employeeId;
        }

        /// <summary>
        /// Перевіряє дані та створює новий запис ремонту.
        /// </summary>
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

            try
            {
                using (var db = new AppDbContext())
                {
                    var customerDevice = db.CustomerDevices
                        .OrderBy(device => device.DeviceId)
                        .FirstOrDefault();
                    int? initialStatusId = db.RepairStatuses
                        .OrderBy(status => status.StatusId)
                        .Select(status => (int?)status.StatusId)
                        .FirstOrDefault();

                    if (customerDevice == null || initialStatusId == null)
                    {
                        MessageBox.Show("У базі даних не налаштовано клієнта, пристрій або статус ремонту!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var newRepair = new Repair
                    {
                        RepairNumber = $"REP-{DateTime.Now:yyMMdd-mmss}",
                        CustomerId = customerDevice.CustomerId,
                        DeviceId = customerDevice.DeviceId,
                        CreatedBy = _employeeId,
                        StatusId = initialStatusId.Value,
                        CustomerDescription = txtDescription.Text.Trim(),
                        EstimatedPrice = price,
                        ReceivedAt = DateTime.Now
                    };

                    db.Repairs.Add(newRepair);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseErrorHandler.Show(ex, "Не вдалося створити ремонт.");
                return;
            }

            MessageBox.Show("Ремонт успішно створено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true; // Закриваємо вікно з сигналом успіху
            this.Close();
        }
    }
}