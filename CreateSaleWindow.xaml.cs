using Microsoft.EntityFrameworkCore;
using Service_Center.Data;
using Service_Store.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Service_Store
{
    public partial class CreateSaleWindow : Window
    {
        public CreateSaleWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                cbCustomers.ItemsSource = db.Customers.ToList();
                // Завантажуємо лише ті позиції зі складу, де є товар у наявності (> 0)
                cbProducts.ItemsSource = db.WarehouseStocks
                    .Include(w => w.Product)
                    .Where(w => w.Quantity > 0)
                    .ToList();
            }
        }

        private void CompleteSale_Click(object sender, RoutedEventArgs e)
        {
            if (cbProducts.SelectedValue == null || string.IsNullOrWhiteSpace(txtQuantity.Text))
            {
                MessageBox.Show("Оберіть товар та вкажіть кількість!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int qty) || qty <= 0)
            {
                MessageBox.Show("Введіть коректну кількість товару!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbCustomers.SelectedValue == null)
            {
                MessageBox.Show("Оберіть клієнта для оформлення продажу!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int customerId = Convert.ToInt32(cbCustomers.SelectedValue);

            var selectedStock = cbProducts.SelectedItem as WarehouseStock;
            if (selectedStock == null) return;

            if (selectedStock.Quantity < qty)
            {
                MessageBox.Show($"На складі недостатньо товару! Доступно: {selectedStock.Quantity}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string paymentMethod = (cbPaymentMethod.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "CASH";
            decimal unitPrice = selectedStock.Product.SalePrice;
            decimal totalAmount = unitPrice * qty;

            using (var db = new AppDbContext())
            {
                // Отримуємо першого доступного працівника з бази, щоб уникнути помилки FK
                var employee = db.Employees.FirstOrDefault();
                if (employee == null)
                {
                    MessageBox.Show("У базі даних немає жодного працівника! Додайте працівника перед оформленням продажу.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 1. Створюємо запис про продаж
                var sale = new Sale
                {
                    SaleNumber = "SALE-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    CustomerId = customerId,
                    EmployeeId = employee.EmployeeId, // Використовуємо реальний ID з бази
                    SaleDate = DateTime.Now,
                    TotalAmount = totalAmount,
                    BonusUsed = 0.00m
                };
                db.Sales.Add(sale);
                db.SaveChanges(); // Зберігаємо, щоб отримати SaleId

                // 2. Додаємо позицію до чеку
                var saleItem = new SaleItem
                {
                    SaleId = sale.SaleId,
                    ProductId = selectedStock.ProductId,
                    Quantity = qty,
                    UnitPrice = unitPrice,
                    Total = totalAmount
                };
                db.SaleItems.Add(saleItem);

                // 3. Записуємо оплату
                var payment = new Payment
                {
                    SaleId = sale.SaleId,
                    PaymentMethod = paymentMethod,
                    Amount = totalAmount,
                    CreatedAt = DateTime.Now
                };
                db.Payments.Add(payment);

                // 4. Зменшуємо залишок на складі
                var stockToUpdate = db.WarehouseStocks.FirstOrDefault(w => w.StockId == selectedStock.StockId);
                if (stockToUpdate != null)
                {
                    stockToUpdate.Quantity -= qty;
                }

                db.SaveChanges();
            }

            MessageBox.Show("Продаж успішно оформлено! Товар списано зі складу.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }
    }
}