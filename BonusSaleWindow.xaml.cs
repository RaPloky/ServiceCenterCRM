using Service_Center.Data;
using Service_Store.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Service_Store
{
    public partial class BonusSaleWindow : Window
    {
        private int _employeeId;

        public BonusSaleWindow(int employeeId)
        {
            InitializeComponent();
            _employeeId = employeeId;
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                cbProducts.ItemsSource = db.Products.ToList();
                cbCustomers.ItemsSource = db.Customers.ToList();
            }
        }

        private void CbProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateTotals();
        }

        private void CbCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCustomers.SelectedItem is Customer selectedCustomer)
            {
                txtCustomerBonus.Text = $"{selectedCustomer.BonusBalance:N2} грн";
            }
            else
            {
                txtCustomerBonus.Text = "0.00 грн";
            }
            CalculateTotals();
        }

        private void TxtBonusToSpend_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateTotals();
        }

        private void CalculateTotals()
        {
            if (cbProducts.SelectedItem is Product product && cbCustomers.SelectedItem is Customer customer)
            {
                decimal price = product.SalePrice;
                txtProductPrice.Text = $"Ціна товару: {price:N2} грн";

                decimal bonusToSpend = 0;
                if (decimal.TryParse(txtBonusToSpend.Text, out decimal parsedBonus))
                {
                    bonusToSpend = Math.Min(parsedBonus, customer.BonusBalance);
                    bonusToSpend = Math.Min(bonusToSpend, price);
                }

                decimal finalAmount = price - bonusToSpend;
                decimal bonusesEarned = finalAmount * 0.01m; // 1% на залишок від суми покупки

                txtFinalAmount.Text = $"До сплати: {finalAmount:N2} грн";
                txtBonusesEarned.Text = $"Буде нараховано бонусів: {bonusesEarned:N2} грн";
            }
        }

        private void CompleteSale_Click(object sender, RoutedEventArgs e)
        {
            if (cbProducts.SelectedItem is not Product selectedProduct)
            {
                MessageBox.Show("Оберіть товар для продажу!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbCustomers.SelectedItem is not Customer selectedCustomer)
            {
                MessageBox.Show("Оберіть клієнта!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal bonusToSpend = 0;
            decimal.TryParse(txtBonusToSpend.Text, out bonusToSpend);
            bonusToSpend = Math.Min(bonusToSpend, selectedCustomer.BonusBalance);
            bonusToSpend = Math.Min(bonusToSpend, selectedProduct.SalePrice);

            decimal finalAmount = selectedProduct.SalePrice - bonusToSpend;
            decimal bonusesEarned = finalAmount * 0.01m;

            using (var db = new AppDbContext())
            {
                var dbCustomer = db.Customers.Find(selectedCustomer.CustomerId);
                if (dbCustomer == null) return;

                // 1. Створюємо запис про продаж
                var sale = new Sale
                {
                    EmployeeId = _employeeId,
                    CustomerId = dbCustomer.CustomerId,
                    SaleDate = DateTime.Now,
                    TotalAmount = finalAmount,
                    BonusUsed = bonusToSpend,
                    SaleNumber = "POS-" + DateTime.Now.ToString("yyyyMMddHHmmssfff")
                };
                db.Sales.Add(sale);
                db.SaveChanges(); // Зберігаємо, щоб отримати sale.SaleId

                // 2. Додаємо позицію чека
                var saleItem = new SaleItem
                {
                    SaleId = sale.SaleId,
                    ProductId = selectedProduct.ProductId,
                    Quantity = 1,
                    UnitPrice = selectedProduct.SalePrice
                };
                db.SaleItems.Add(saleItem);

                // 3. Якщо списуємо бонуси
                if (bonusToSpend > 0)
                {
                    dbCustomer.BonusBalance -= bonusToSpend;

                    var spendTransaction = new BonusTransaction
                    {
                        CustomerId = dbCustomer.CustomerId,
                        SaleId = sale.SaleId,
                        TransactionType = "spend",
                        Amount = bonusToSpend,
                        Description = $"Списання бонусів за покупку №{sale.SaleId}",
                        CreatedAt = DateTime.Now
                    };
                    db.BonusTransactions.Add(spendTransaction);
                }

                // 4. Нараховуємо 1% нових бонусів
                if (bonusesEarned > 0)
                {
                    dbCustomer.BonusBalance += bonusesEarned;

                    var earnTransaction = new BonusTransaction
                    {
                        CustomerId = dbCustomer.CustomerId,
                        SaleId = sale.SaleId,
                        TransactionType = "earn",
                        Amount = bonusesEarned,
                        Description = $"Нарахування 1% за покупку №{sale.SaleId}",
                        CreatedAt = DateTime.Now
                    };
                    db.BonusTransactions.Add(earnTransaction);
                }

                db.SaveChanges();

                // 5. Виклик вікна розподілу оплат (SplitPaymentWindow)
                SplitPaymentWindow splitWindow = new SplitPaymentWindow(sale.SaleId, sale.TotalAmount, dbCustomer.CustomerId);
                if (splitWindow.ShowDialog() == true)
                {
                    MessageBox.Show("Продаж успішно оформлено та оплачено!", "Каса", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    // Якщо касир закрив вікно оплати без підтвердження
                }
            }
        }
    }
}