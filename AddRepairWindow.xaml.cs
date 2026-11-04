using Service_Center.Data;
using Service_Store.Helpers;
using Service_Store.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            LoadCustomers();
        }

        /// <summary>
        /// Завантажує клієнтів до списку вибору.
        /// </summary>
        private void LoadCustomers()
        {
            using var db = new AppDbContext();
            cbCustomers.ItemsSource = db.Customers
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToList();
        }

        /// <summary>
        /// Завантажує пристрої вибраного клієнта.
        /// </summary>
        private void CbCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCustomers.SelectedValue is not int customerId)
            {
                cbDevices.ItemsSource = null;
                return;
            }

            using var db = new AppDbContext();
            cbDevices.ItemsSource = db.CustomerDevices
                .Where(device => device.CustomerId == customerId)
                .OrderBy(device => device.Brand)
                .ThenBy(device => device.Model)
                .ToList();
        }

        /// <summary>
        /// Перевіряє дані та створює новий запис ремонту.
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (cbCustomers.SelectedValue is not int customerId ||
                cbDevices.SelectedValue is not int deviceId ||
                string.IsNullOrWhiteSpace(txtDescription.Text) ||
                string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Оберіть клієнта та пристрій і заповніть усі поля!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    int? initialStatusId = db.RepairStatuses
                        .OrderBy(status => status.StatusId)
                        .Select(status => (int?)status.StatusId)
                        .FirstOrDefault();

                    if (initialStatusId == null)
                    {
                        MessageBox.Show("У базі даних не налаштовано статуси ремонтів!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var newRepair = new Repair
                    {
                        RepairNumber = $"REP-{DateTime.Now:yyMMdd-mmss}",
                        CustomerId = customerId,
                        DeviceId = deviceId,
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