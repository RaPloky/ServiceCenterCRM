using System;
using System.Windows;

namespace Service_Store
{
    public partial class SplitPaymentWindow : Window
    {
        private decimal _totalAmount;

        public decimal CashAmount { get; private set; }
        public decimal CardAmount { get; private set; }
        public decimal BonusAmount { get; private set; }

        /// <summary>
        /// Ініціалізує вікно розподілу платежу.
        /// </summary>
        public SplitPaymentWindow(decimal totalAmount, decimal initialBonusAmount = 0)
        {
            InitializeComponent();
            _totalAmount = totalAmount;

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
                !decimal.TryParse(txtCardAmount.Text, out decimal card) || card < 0)
            {
                MessageBox.Show("Введіть коректні числові значення для оплат!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal sumEntered = cash + card;

            // Перевіряємо, чи збігається сума платежів із сумою чека (з невеликою похибкою на округлення)
            if (Math.Abs(sumEntered - _totalAmount) > 0.01m)
            {
                MessageBox.Show($"Сума рознесених платежів ({sumEntered:N2} грн) не дорівнює загальній сумі чека ({_totalAmount:N2} грн)!",
                    "Невідповідність суми", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CashAmount = cash;
            CardAmount = card;

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