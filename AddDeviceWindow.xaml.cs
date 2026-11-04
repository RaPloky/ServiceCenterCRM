using Service_Center.Data;
using Service_Store.Models;
using System.Linq;
using System.Windows;

namespace Service_Store
{
    public partial class AddDeviceWindow : Window
    {
        private readonly int? _presetCustomerId;

        /// <summary>
        /// Ініціалізує вікно додавання пристрою.
        /// </summary>
        public AddDeviceWindow()
            : this(null)
        {
        }

        /// <summary>
        /// Ініціалізує вікно додавання пристрою з попередньо вибраним власником.
        /// </summary>
        public AddDeviceWindow(int customerId)
            : this((int?)customerId)
        {
        }

        private AddDeviceWindow(int? customerId)
        {
            _presetCustomerId = customerId;
            InitializeComponent();
            LoadCustomersForCombo();
        }

        /// <summary>
        /// Завантажує клієнтів до списку вибору власника.
        /// </summary>
        private void LoadCustomersForCombo()
        {
            using (var db = new AppDbContext())
            {
                // Завантажуємо список клієнтів для вибору власника
                cbCustomers.ItemsSource = db.Customers.ToList();

                if (_presetCustomerId.HasValue)
                {
                    cbCustomers.SelectedValue = _presetCustomerId.Value;
                    cbCustomers.IsHitTestVisible = false;
                    cbCustomers.Focusable = false;
                }
            }
        }

        /// <summary>
        /// Перевіряє та зберігає новий пристрій клієнта.
        /// </summary>
        private void SaveDevice_Click(object sender, RoutedEventArgs e)
        {
            if (cbCustomers.SelectedValue == null ||
                string.IsNullOrWhiteSpace(txtDeviceType.Text) ||
                string.IsNullOrWhiteSpace(txtBrand.Text) ||
                string.IsNullOrWhiteSpace(txtModel.Text))
            {
                MessageBox.Show("Будь ласка, оберіть клієнта та заповніть обов'язкові поля (Тип, Бренд, Модель)!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                var newDevice = new CustomerDevice
                {
                    CustomerId = (int)cbCustomers.SelectedValue,
                    DeviceType = txtDeviceType.Text.Trim(),
                    Brand = txtBrand.Text.Trim(),
                    Model = txtModel.Text.Trim(),
                    SerialNumber = string.IsNullOrWhiteSpace(txtSerialNumber.Text) ? null : txtSerialNumber.Text.Trim()
                };

                db.CustomerDevices.Add(newDevice);
                db.SaveChanges();
            }

            MessageBox.Show("Пристрій успішно зареєстровано!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }
    }
}