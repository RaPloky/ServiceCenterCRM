using Microsoft.EntityFrameworkCore;
using Service_Center.Data;
using Service_Store.Helpers;
using Service_Store.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Service_Store
{
    public partial class AddRepairWindow : Window
    {
        private int _employeeId;
        private List<Customer> _customers = new();
        private List<CustomerDevice> _devices = new();
        private Customer? _selectedCustomer;
        private CustomerDevice? _selectedDevice;

        /// <summary>
        /// Ініціалізує вікно створення ремонту для працівника.
        /// </summary>
        public AddRepairWindow(int employeeId)
        {
            InitializeComponent();
            _employeeId = employeeId;
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            try
            {
                using var db = new AppDbContext();
                _customers = db.Customers.OrderBy(customer => customer.LastName).ThenBy(customer => customer.FirstName).ToList();
                ApplyCustomerFilter();
            }
            catch (Exception ex)
            {
                DatabaseErrorHandler.Show(ex, "Не вдалося завантажити клієнтів.");
            }
        }

        private void ApplyCustomerFilter()
        {
            string search = txtCustomerSearch?.Text.Trim() ?? string.Empty;
            dgCustomers.ItemsSource = _customers.Where(customer =>
                string.IsNullOrWhiteSpace(search) ||
                customer.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                customer.LastName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                customer.Phone.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private void CustomerSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyCustomerFilter();

        private void Customers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgCustomers.SelectedItem is Customer customer)
            {
                _selectedCustomer = customer;
                _selectedDevice = null;
                btnAddDevice.IsEnabled = true;
                LoadDevices();
            }
        }

        private void LoadDevices()
        {
            _devices.Clear();
            dgDevices.ItemsSource = null;

            if (_selectedCustomer == null)
            {
                txtDeviceStatus.Text = "Для коригування запису ремонту, оберіть клієнта для відображення даних";
                return;
            }

            try
            {
                using var db = new AppDbContext();
                _devices = db.CustomerDevices
                    .Include(device => device.Customer)
                    .Where(device => device.CustomerId == _selectedCustomer.CustomerId)
                    .OrderBy(device => device.Brand)
                    .ThenBy(device => device.Model)
                    .ToList();
                ApplyDeviceFilter();
                txtDeviceStatus.Text = _devices.Count == 0
                    ? "У даного клієнта ще не зареєстровано пристроїв на ремонт"
                    : string.Empty;
            }
            catch (Exception ex)
            {
                txtDeviceStatus.Text = string.Empty;
                DatabaseErrorHandler.Show(ex, "Не вдалося завантажити пристрої клієнта.");
            }
        }

        private void ApplyDeviceFilter()
        {
            string search = txtDeviceSearch?.Text.Trim() ?? string.Empty;
            dgDevices.ItemsSource = _devices.Where(device =>
                string.IsNullOrWhiteSpace(search) ||
                device.DeviceType.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                device.Brand.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                device.Model.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (device.Customer?.LastName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
        }

        private void DeviceSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyDeviceFilter();

        private void Devices_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgDevices.SelectedItem is CustomerDevice device)
            {
                _selectedDevice = device;
            }
        }

        private void AddDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCustomer == null)
            {
                MessageBox.Show("Необхідно обрати клієнта для додавання пристрою", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var addDeviceWindow = new AddDeviceWindow(_selectedCustomer.CustomerId);
            if (addDeviceWindow.ShowDialog() == true)
            {
                LoadCustomers();
                if (_selectedCustomer != null)
                {
                    LoadDevices();
                }
            }
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

            if (_selectedCustomer == null || _selectedDevice == null)
            {
                MessageBox.Show("Оберіть клієнта та пристрій подвійним натисканням у відповідних таблицях!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                        MessageBox.Show("У базі даних не налаштовано клієнта, пристрій або статус ремонту!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var newRepair = new Repair
                    {
                        RepairNumber = $"REP-{DateTime.Now:yyMMdd-mmss}",
                        CustomerId = _selectedCustomer.CustomerId,
                        DeviceId = _selectedDevice.DeviceId,
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