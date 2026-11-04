using Service_Center.Data;
using Service_Store.Helpers;
using Service_Store.Models;
using System;
using System.Linq;
using System.Windows;

namespace Service_Store
{
    public partial class SplitPaymentWindow : Window
    {
        private int _saleId;
        private decimal _totalAmount;
        private int _customerId;

        /// <summary>
        /// Ініціалізує вікно розподілу платежу.
        /// </summary>
        public SplitPaymentWindow(int saleId, decimal totalAmount, int customerId)
        {
            InitializeComponent();
            _saleId = saleId;
            _totalAmount = totalAmount;
            _customerId = customerId;

            txtTotalAmount.Text = $"{_totalAmount:N2} грн";
            // За замовчуванням усю суму ставимо на готівку для зручності
            txtCashAmount.Text = _totalAmount.ToString("0.##");
        }

        /// <summary>
        /// Перевіряє та зберігає розподіл платежу за способами оплати.
        /// </summary>
        private void ConfirmPayment_Click(object sender, RoutedEventArgs e)
        {
            // Парсимо введені суми
            if (!decimal.TryParse(txtCashAmount.Text, out decimal cash) || cash < 0 ||
                !decimal.TryParse(txtCardAmount.Text, out decimal card) || card < 0 ||
                !decimal.TryParse(txtBonusAmount.Text, out decimal bonus) || bonus < 0)
            {
                MessageBox.Show("Введіть коректні числові значення для оплат!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal sumEntered = cash + card + bonus;

            // Перевіряємо, чи збігається сума платежів із сумою чека (з невеликою похибкою на округлення)
            if (Math.Abs(sumEntered - _totalAmount) > 0.01m)
            {
                MessageBox.Show($"Сума рознесених платежів ({sumEntered:N2} грн) не дорівнює загальній сумі чека ({_totalAmount:N2} грн)!",
                    "Невідповідність суми", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    if (bonus > 0)
                    {
                        var customer = db.Customers.Find(_customerId);
                        if (customer == null || customer.BonusBalance < bonus)
                        {
                            MessageBox.Show("У клієнта недостатньо бонусних балів для списання!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        customer.BonusBalance -= bonus;

                        var bonusTransaction = new BonusTransaction
                        {
                            CustomerId = _customerId,
                            SaleId = _saleId,
                            TransactionType = "use",
                            Amount = -bonus,
                            Description = $"Списання бонусів при оплаті чека №{_saleId}",
                            CreatedAt = DateTime.Now
                        };
                        db.BonusTransactions.Add(bonusTransaction);

                        var saleRecord = db.Sales.Find(_saleId);
                        if (saleRecord != null)
                        {
                            saleRecord.BonusUsed = bonus;
                        }
                    }

                    if (cash > 0)
                    {
                        db.Payments.Add(new Payment { SaleId = _saleId, PaymentMethod = "CASH", Amount = cash, CreatedAt = DateTime.Now });
                    }
                    if (card > 0)
                    {
                        db.Payments.Add(new Payment { SaleId = _saleId, PaymentMethod = "CARD", Amount = card, CreatedAt = DateTime.Now });
                    }
                    if (bonus > 0)
                    {
                        db.Payments.Add(new Payment { SaleId = _saleId, PaymentMethod = "BONUS", Amount = bonus, CreatedAt = DateTime.Now });
                    }

                    var saleItem = db.SaleItems.FirstOrDefault(item => item.SaleId == _saleId);
                    if (saleItem == null)
                    {
                        MessageBox.Show("Для продажу не знайдено товарну позицію!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var stock = db.WarehouseStocks.FirstOrDefault(item => item.ProductId == saleItem.ProductId);
                    if (stock == null || stock.Quantity - stock.ReservedQuantity < saleItem.Quantity)
                    {
                        MessageBox.Show("Недостатньо товару на складі для завершення продажу!", "Помилка складу", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    stock.Quantity -= saleItem.Quantity;

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseErrorHandler.Show(ex, "Не вдалося завершити оплату.");
                return;
            }

            MessageBox.Show("Оплату успішно проведено та розподілено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// Скасовує розподіл платежу та закриває вікно.
        /// </summary>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}