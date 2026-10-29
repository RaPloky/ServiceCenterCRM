using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Service_Center.Data;
using Service_Center.Models;
using Service_Store.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace Service_Store
{
    public partial class EditRepairWindow : Window
    {
        private int _repairId;
        private Employee _currentEmployee;

        public EditRepairWindow(int repairId, Employee currentEmployee)
        {
            InitializeComponent();
            _repairId = repairId;
            _currentEmployee = currentEmployee;

            LoadStatuses();
            LoadEmployees();
            LoadRepairData();
            LoadRepairHistory(); // Завантажуємо історію при відкритті вікна
            LoadAttachments();
            LoadRepairPartsData();
        }

        private void LoadAttachments()
        {
            using (var db = new AppDbContext())
            {
                var attachments = db.RepairAttachments
                    .Where(a => a.RepairId == _repairId)
                    .ToList();

                lstAttachments.ItemsSource = attachments;
            }
        }

        private void AddAttachment_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Оберіть файл (фото або акт)",
                Filter = "Усі файли (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string sourceFilePath = openFileDialog.FileName;
                string uploadsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(sourceFilePath)}";
                string destinationFilePath = Path.Combine(uploadsFolder, fileName);

                File.Copy(sourceFilePath, destinationFilePath);

                using (var db = new AppDbContext())
                {
                    var attachment = new RepairAttachment
                    {
                        RepairId = _repairId,
                        FilePath = $"Uploads/{fileName}",
                        Description = Path.GetFileName(sourceFilePath),
                        UploadedBy = _currentEmployee != null ? _currentEmployee.EmployeeId : 1,
                        CreatedAt = DateTime.Now
                    };

                    db.RepairAttachments.Add(attachment);
                    db.SaveChanges();
                }

                LoadAttachments();
                MessageBox.Show("Файл успішно додано до ремонту!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteAttachment_Click(object sender, RoutedEventArgs e)
        {
            if (lstAttachments.SelectedItem is RepairAttachment selectedAttachment)
            {
                if (MessageBox.Show("Ви впевнені, що хочете видалити цей файл?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (var db = new AppDbContext())
                    {
                        var dbAttachment = db.RepairAttachments.Find(selectedAttachment.AttachmentId);
                        if (dbAttachment != null)
                        {
                            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbAttachment.FilePath);
                            if (File.Exists(fullPath))
                            {
                                File.Delete(fullPath);
                            }

                            db.RepairAttachments.Remove(dbAttachment);
                            db.SaveChanges();
                        }
                    }

                    LoadAttachments();
                }
            }
            else
            {
                MessageBox.Show("Оберіть файл зі списку для видалення!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LstAttachments_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lstAttachments.SelectedItem is RepairAttachment selectedAttachment)
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, selectedAttachment.FilePath);

                if (File.Exists(fullPath))
                {
                    try
                    {
                        // Запускаємо файл у системній програмі за замовчуванням
                        Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Не вдалося відкрити файл: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Файл не знайдено на дискy за вказаним шляхом!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void LoadStatuses()
        {
            using (var db = new AppDbContext())
            {
                cbStatuses.ItemsSource = db.RepairStatuses.ToList();
            }
        }

        private void LoadEmployees()
        {
            using (var db = new AppDbContext())
            {
                cbAssignedEmployee.ItemsSource = db.Employees.ToList();
            }
        }

        private void LoadRepairData()
        {
            using (var db = new AppDbContext())
            {
                var repair = db.Repairs.FirstOrDefault(r => r.RepairId == _repairId);
                if (repair != null)
                {
                    txtRepairNumber.Text = repair.RepairNumber;
                    cbStatuses.SelectedValue = repair.StatusId;
                    cbAssignedEmployee.SelectedValue = repair.AssignedEmployeeId;
                    txtCustomerDescription.Text = repair.CustomerDescription;
                    txtInternalNotes.Text = repair.InternalNotes;
                    txtEstimatedPrice.Text = repair.EstimatedPrice.ToString();
                    txtFinalPrice.Text = repair.FinalPrice?.ToString() ?? repair.EstimatedPrice.ToString();
                }
            }
        }

        private void LoadRepairHistory()
        {
            using (var db = new AppDbContext())
            {
                var historyList = db.RepairStatusHistories
                    .Include(h => h.Employee)
                    .Where(h => h.RepairId == _repairId)
                    .OrderByDescending(h => h.CreatedAt)
                    .Select(h => new
                    {
                        HistoryId = h.HistoryId,
                        OldStatusId = h.OldStatusId,
                        NewStatusId = h.NewStatusId,
                        EmployeeName = h.Employee != null ? h.Employee.FirstName + " " + h.Employee.LastName : "Система",
                        Comment = h.Comment,
                        CreatedAt = h.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .ToList();

                // Прив'язуємо отриманий список до нашої DataGrid у вікні
                dgRepairHistory.ItemsSource = historyList;
            }
        }

        private void SaveRepair_Click(object sender, RoutedEventArgs e)
        {
            // 1. Валідація вхідних даних
            if (cbStatuses.SelectedValue == null)
            {
                MessageBox.Show("Будь ласка, оберіть статус ремонту!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal estimatedPrice = 0;
            if (!string.IsNullOrWhiteSpace(txtEstimatedPrice.Text) && !decimal.TryParse(txtEstimatedPrice.Text, out estimatedPrice))
            {
                MessageBox.Show("Будь ласка, введіть коректну орієнтовну вартість!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal finalPrice = 0;
            if (!string.IsNullOrWhiteSpace(txtFinalPrice.Text) && !decimal.TryParse(txtFinalPrice.Text, out finalPrice))
            {
                MessageBox.Show("Будь ласка, введіть коректну фінальну вартість!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int newStatusId = Convert.ToInt32(cbStatuses.SelectedValue);

            using (var db = new AppDbContext())
            {
                // 2. Знаходимо ремонт у базі даних
                var repair = db.Repairs.FirstOrDefault(r => r.RepairId == _repairId);

                if (repair == null)
                {
                    MessageBox.Show("Ремонт не знайдено в базі даних!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int oldStatusId = repair.StatusId;

                // 3. Оновлюємо всі поля ремонту з форми
                repair.StatusId = newStatusId;
                repair.CustomerDescription = txtCustomerDescription.Text;
                repair.InternalNotes = txtInternalNotes.Text;
                repair.EstimatedPrice = estimatedPrice;
                repair.FinalPrice = finalPrice;

                if (cbAssignedEmployee.SelectedValue != null)
                {
                    repair.AssignedEmployeeId = Convert.ToInt32(cbAssignedEmployee.SelectedValue);
                }

                if (newStatusId == 9 && repair.CompletedAt == null)
                {
                    repair.CompletedAt = DateTime.Now;
                }
                else if (newStatusId == 10 && repair.IssuedAt == null)
                {
                    repair.IssuedAt = DateTime.Now;
                }

                // 4. Логування: якщо статус дійсно змінився, створюємо запис в історії
                if (oldStatusId != newStatusId)
                {
                    var historyRecord = new RepairStatusHistory
                    {
                        RepairId = repair.RepairId,
                        OldStatusId = oldStatusId,
                        NewStatusId = newStatusId,
                        EmployeeId = _currentEmployee != null ? _currentEmployee.EmployeeId : 1,
                        Comment = string.IsNullOrWhiteSpace(txtLogComment.Text)
                            ? $"Зміна статусу з ID {oldStatusId} на ID {newStatusId}"
                            : txtLogComment.Text,
                        CreatedAt = DateTime.Now
                    };

                    db.RepairStatusHistories.Add(historyRecord);
                }

                // 5. Зберігаємо всі зміни у базі даних
                db.SaveChanges();
            }

            // Оновлюємо таблицю історії одразу після збереження в цьому ж вікні
            LoadRepairHistory();

            MessageBox.Show("Дані про ремонт та історію статусів успішно збережено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);

            // Якщо ти хочеш, щоб вікно не закривалося одразу після збереження (щоб побачити оновлену таблицю), 
            // можна закоментувати рядок нижче, а закриття залишити на окрему кнопку:
            this.DialogResult = true;
            this.Close();
        }

        private void LoadRepairPartsData()
        {
            using (var db = new AppDbContext())
            {
                // Формуємо зручний список для випадаючого списку з урахуванням залишку
                cbWarehouseProducts.ItemsSource = db.WarehouseStocks
                    .Include(w => w.Product)
                    .Where(w => w.Product != null && w.Quantity > 0)
                    .Select(w => new
                    {
                        ProductId = w.ProductId,
                        DisplayText = $"{w.Product.Name} (Залишок: {w.Quantity} шт. — {w.Product.SalePrice} грн)"
                    })
                    .ToList();

                // Завантажуємо використані запчастини для поточного ремонту
                var parts = db.RepairParts
                    .Where(rp => rp.RepairId == _repairId)
                    .Select(rp => new
                    {
                        rp.RepairPartId,
                        rp.Name,
                        rp.Quantity,
                        rp.UnitPrice,
                        TotalPrice = rp.Quantity * rp.UnitPrice
                    })
                    .ToList();

                dgRepairParts.ItemsSource = parts;
            }
        }

        private void AddRepairPart_Click(object sender, RoutedEventArgs e)
        {
            if (cbWarehouseProducts.SelectedValue == null)
            {
                MessageBox.Show("Оберіть товар/запчастину зі складу!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtPartQuantity.Text, out int qty) || qty <= 0)
            {
                MessageBox.Show("Введіть коректну кількість!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int productId = Convert.ToInt32(cbWarehouseProducts.SelectedValue);

            using (var db = new AppDbContext())
            {
                // Перевіряємо залишок на складі
                var stockItem = db.WarehouseStocks
                    .Include(w => w.Product)
                    .FirstOrDefault(w => w.ProductId == productId);

                if (stockItem == null || stockItem.Quantity < qty)
                {
                    MessageBox.Show("На складі недостатньо цієї позиції!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var product = stockItem.Product;
                if (product == null) return;

                // Створюємо запис із назвою деталі
                var repairPart = new RepairPart
                {
                    RepairId = _repairId,
                    ProductId = productId,
                    Name = product.Name, // Записуємо назву у нове поле
                    Quantity = qty,
                    UnitPrice = product.SalePrice
                };

                db.RepairParts.Add(repairPart);

                // Зменшуємо залишок на складі
                stockItem.Quantity -= qty;

                db.SaveChanges();
            }

            LoadRepairPartsData();
            txtPartQuantity.Text = "1";
            MessageBox.Show("Запчастину успішно додано до ремонту, склад оновлено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteRepairPart_Click(object sender, RoutedEventArgs e)
        {
            if (dgRepairParts.SelectedItem != null)
            {
                var selectedItem = dgRepairParts.SelectedItem;
                int repairPartId = (int)selectedItem.GetType().GetProperty("RepairPartId").GetValue(selectedItem, null);

                if (MessageBox.Show("Ви дійсно хочете видалити цей запис? (Кількість товару повернеться на склад)", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (var db = new AppDbContext())
                    {
                        var repairPart = db.RepairParts.Find(repairPartId);
                        if (repairPart != null)
                        {
                            // Повертаємо кількість назад на склад
                            var stockItem = db.WarehouseStocks.FirstOrDefault(w => w.ProductId == repairPart.ProductId);
                            if (stockItem != null)
                            {
                                stockItem.Quantity += repairPart.Quantity;
                            }

                            db.RepairParts.Remove(repairPart);
                            db.SaveChanges();
                        }
                    }

                    LoadRepairPartsData();
                    MessageBox.Show("Запис видалено, склад оновлено.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Оберіть запчастину в таблиці для видалення!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}