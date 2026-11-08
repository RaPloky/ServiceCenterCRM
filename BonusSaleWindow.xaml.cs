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
    public sealed class CartItem
    {
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Total => Product.SalePrice * Quantity;
    }

    public partial class BonusSaleWindow : Window
    {
        private int _employeeId;
        private List<Product> _products = new();
        private List<Customer> _customers = new();
        private Customer? _selectedCustomer;
        private readonly List<CartItem> _cart = new();

        /// <summary>
        /// Ініціалізує вікно продажу з використанням бонусів.
        /// </summary>
        public BonusSaleWindow(int employeeId)
        {
            InitializeComponent();
            _employeeId = employeeId;
            LoadData();
        }

        /// <summary>
        /// Завантажує товари та клієнтів до списків вибору.
        /// </summary>
        private void LoadData()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    _products = db.Products.OrderBy(product => product.Name).ToList();
                    _customers = db.Customers.OrderBy(customer => customer.LastName).ThenBy(customer => customer.FirstName).ToList();
                    ApplyProductFilter();
                    ApplyCustomerFilter();
                }
            }
            catch (Exception ex)
            {
                DatabaseErrorHandler.Show(ex, "Не вдалося завантажити товари та клієнтів.");
            }
        }

        private void ApplyProductFilter()
        {
            string search = txtProductSearch?.Text.Trim() ?? string.Empty;
            dgProducts.ItemsSource = _products.Where(product =>
                string.IsNullOrWhiteSpace(search) ||
                product.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                product.ProductId.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private void ProductSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyProductFilter();

        private void ApplyCustomerFilter()
        {
            string search = txtCustomerSearch?.Text.Trim() ?? string.Empty;
            dgCustomers.ItemsSource = _customers.Where(customer =>
                string.IsNullOrWhiteSpace(search) ||
                customer.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                customer.LastName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private void CustomerSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyCustomerFilter();

        private void Products_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgProducts.SelectedItem is Product product)
            {
                AddToCart(product);
                CalculateTotals();
            }
        }

        private void AddToCart(Product product)
        {
            try
            {
                using var db = new AppDbContext();
                int currentQuantity = _cart.FirstOrDefault(item => item.Product.ProductId == product.ProductId)?.Quantity ?? 0;
                var stock = db.WarehouseStocks.FirstOrDefault(item => item.ProductId == product.ProductId);
                int availableQuantity = stock?.Quantity - stock?.ReservedQuantity ?? 0;

                if (stock == null || currentQuantity >= availableQuantity)
                {
                    MessageBox.Show("Недостатньо обраного товару на складі!", "Немає товару", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var cartItem = _cart.FirstOrDefault(item => item.Product.ProductId == product.ProductId);
                if (cartItem == null)
                {
                    _cart.Add(new CartItem { Product = product, Quantity = 1 });
                }
                else
                {
                    cartItem.Quantity++;
                }

                dgCart.ItemsSource = null;
                dgCart.ItemsSource = _cart;
            }
            catch (Exception ex)
            {
                DatabaseErrorHandler.Show(ex, "Не вдалося перевірити залишок товару.");
            }
        }

        private void RemoveCartItem_Click(object sender, RoutedEventArgs e)
        {
            if (dgCart.SelectedItem is not CartItem cartItem)
            {
                MessageBox.Show("Оберіть товар у кошику для видалення.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Видалити «{cartItem.Product.Name}» з кошика?",
                "Підтвердження видалення",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _cart.Remove(cartItem);
                dgCart.ItemsSource = null;
                dgCart.ItemsSource = _cart;
                CalculateTotals();
            }
        }

        private void Customers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgCustomers.SelectedItem is Customer customer)
            {
                _selectedCustomer = customer;
                txtCustomerBonus.Text = $"{customer.BonusBalance:N2} грн";
                CalculateTotals();
            }
        }

        /// <summary>
        /// Перераховує підсумок після зміни використаних бонусів.
        /// </summary>
        private void TxtBonusToSpend_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateTotals();
        }

        /// <summary>
        /// Обчислює суму до сплати та нараховані бонуси.
        /// </summary>
        private void CalculateTotals()
        {
            if (_cart.Count > 0 && _selectedCustomer is Customer customer)
            {
                decimal price = _cart.Sum(item => item.Total);
                txtProductPrice.Text = $"Сума товарів: {price:N2} грн";

                decimal bonusToSpend = 0;
                if (decimal.TryParse(txtBonusToSpend.Text, out decimal parsedBonus))
                {
                    bonusToSpend = Math.Min(parsedBonus, customer.BonusBalance);
                    bonusToSpend = Math.Min(bonusToSpend, price);
                }

                decimal finalAmount = price - bonusToSpend;
                decimal bonusesEarned = finalAmount * 0.01m;

                txtFinalAmount.Text = $"До сплати: {finalAmount:N2} грн";
                txtBonusesEarned.Text = $"Буде нараховано бонусів: {bonusesEarned:N2} грн";
            }
        }

        /// <summary>
        /// Перевіряє дані та зберігає продаж із бонусами.
        /// </summary>
        private void CompleteSale_Click(object sender, RoutedEventArgs e)
        {
            if (_cart.Count == 0)
            {
                MessageBox.Show("Додайте хоча б один товар до кошика!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_selectedCustomer is not Customer selectedCustomer)
            {
                MessageBox.Show("Оберіть клієнта!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal bonusToSpend = 0;
            decimal.TryParse(txtBonusToSpend.Text, out bonusToSpend);
            bonusToSpend = Math.Min(bonusToSpend, selectedCustomer.BonusBalance);
            decimal cartTotal = _cart.Sum(item => item.Total);
            bonusToSpend = Math.Min(bonusToSpend, cartTotal);

            decimal finalAmount = cartTotal - bonusToSpend;
            decimal bonusesEarned = finalAmount * 0.01m;

            try
            {
                using (var db = new AppDbContext())
                {
                    var dbCustomer = db.Customers.Find(selectedCustomer.CustomerId);
                    if (dbCustomer == null) return;

                    var stockByProduct = db.WarehouseStocks
                        .Where(item => _cart.Select(cartItem => cartItem.Product.ProductId).Contains(item.ProductId))
                        .ToDictionary(item => item.ProductId);
                    if (_cart.Any(cartItem => !stockByProduct.TryGetValue(cartItem.Product.ProductId, out var stock) ||
                        stock.Quantity - stock.ReservedQuantity < cartItem.Quantity))
                    {
                        MessageBox.Show("Недостатньо товару на складі для одного або кількох товарів!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

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

                foreach (var cartItem in _cart)
                {
                    db.SaleItems.Add(new SaleItem
                    {
                        SaleId = sale.SaleId,
                        ProductId = cartItem.Product.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Product.SalePrice,
                        Total = cartItem.Total
                    });
                }

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
                }
            }
            catch (Exception ex)
            {
                DatabaseErrorHandler.Show(ex, "Не вдалося оформити продаж.");
            }
        }
    }
}