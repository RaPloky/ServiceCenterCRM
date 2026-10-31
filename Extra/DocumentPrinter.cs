using Service_Center.Data;
using Service_Store.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace Service_Store.Helpers
{
    public static class DocumentPrinter
    {
        /// <summary>
        /// Друк / збереження чека продажу
        /// </summary>
        public static void PrintSaleReceipt(int saleId)
        {
            using (var db = new AppDbContext())
            {
                var sale = db.Sales
                    .Include(s => s.Customer)
                    .Include(s => s.Employee)
                    .FirstOrDefault(s => s.SaleId == saleId);

                if (sale == null)
                {
                    MessageBox.Show("Продаж не знайдено!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var saleItems = db.SaleItems
                    .Where(si => si.SaleId == saleId)
                    .ToList();

                var payments = db.Payments.Where(p => p.SaleId == saleId).ToList();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("=========================================");
                sb.AppendLine("          СЕРВІСНИЙ ЦЕНТР & МАГАЗИН      ");
                sb.AppendLine("              ТОВ 'TECH STORE'           ");
                sb.AppendLine("=========================================");
                sb.AppendLine($"ЧЕК ПРОДАЖУ №: {sale.SaleNumber ?? sale.SaleId.ToString()}");
                sb.AppendLine($"Дата: {sale.SaleDate:dd.MM.yyyy HH:mm}");
                sb.AppendLine($"Касир: {sale.Employee?.FirstName} {sale.Employee?.LastName}");
                sb.AppendLine($"Клієнт: {sale.Customer?.FirstName} {sale.Customer?.LastName}");
                sb.AppendLine("-----------------------------------------");
                sb.AppendLine("Товари/Послуги:");

                foreach (var item in saleItems)
                {
                    var product = db.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
                    string productName = product?.Name ?? "Товар (ID: " + item.ProductId + ")";

                    sb.AppendLine($"{productName}");
                    sb.AppendLine($"  {item.Quantity} x {item.UnitPrice:N2} грн = {item.Quantity * item.UnitPrice:N2} грн");
                }

                sb.AppendLine("-----------------------------------------");
                sb.AppendLine($"Списано бонусів: {sale.BonusUsed:N2} грн");
                sb.AppendLine($"ЗАГАЛЬНА СУМА: {sale.TotalAmount:N2} грн");
                sb.AppendLine("-----------------------------------------");
                sb.AppendLine("Способи оплати:");

                if (payments.Any())
                {
                    foreach (var p in payments)
                    {
                        sb.AppendLine($"  - {p.PaymentMethod}: {p.Amount:N2} грн");
                    }
                }
                else
                {
                    sb.AppendLine($"  - CASH: {sale.TotalAmount:N2} грн");
                }

                sb.AppendLine("=========================================");
                sb.AppendLine("      Дякуємо за покупку! Гарного дня!   ");
                sb.AppendLine("=========================================");

                ProcessDocument("Чек_продажу_" + sale.SaleId, sb.ToString());
            }
        }

        /// <summary>
        /// Друк Акта виконаних робіт по ремонту
        /// </summary>
        public static void PrintRepairAct(int repairId)
        {
            using (var db = new AppDbContext())
            {
                var repair = db.Repairs
                    .Include(r => r.Customer)
                    .Include(r => r.AssignedEmployee)
                    .FirstOrDefault(r => r.RepairId == repairId);

                if (repair == null)
                {
                    MessageBox.Show("Ремонт не знайдено!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var device = db.CustomerDevices.FirstOrDefault(d => d.DeviceId == repair.DeviceId);
                var status = db.RepairStatuses.FirstOrDefault(s => s.StatusId == repair.StatusId);

                var parts = db.RepairParts
                    .Where(rp => rp.RepairId == repairId)
                    .ToList();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("==================================================");
                sb.AppendLine("           АКТ ВИКОНАНИХ РОБІТ ТА РЕМОНТУ         ");
                sb.AppendLine("                СЕРВІСНИЙ ЦЕНТР                   ");
                sb.AppendLine("==================================================");
                sb.AppendLine($"Заявка №: {repair.RepairNumber ?? repair.RepairId.ToString()}");
                sb.AppendLine($"Дата видачі/акта: {DateTime.Now:dd.MM.yyyy HH:mm}");
                sb.AppendLine($"Клієнт: {repair.Customer?.LastName} {repair.Customer?.FirstName}");
                sb.AppendLine($"Телефон: {repair.Customer?.Phone}");
                sb.AppendLine($"Пристрій: {device?.DeviceType} {device?.Brand} {device?.Model}");
                sb.AppendLine($"Серійний №: {device?.SerialNumber}");
                sb.AppendLine($"Майстер: {repair.AssignedEmployee?.FirstName} {repair.AssignedEmployee?.LastName}");
                sb.AppendLine("--------------------------------------------------");
                sb.AppendLine($"Опис проблеми: {repair.CustomerDescription}");
                sb.AppendLine($"Поточний статус: {status?.Name ?? "Не вказано"}");
                sb.AppendLine("--------------------------------------------------");
                sb.AppendLine("Використані запчастини та матеріали:");

                decimal partsTotal = 0;
                if (parts.Any())
                {
                    foreach (var p in parts)
                    {
                        decimal lineTotal = p.Quantity * p.UnitPrice;
                        partsTotal += lineTotal;
                        sb.AppendLine($"  * {p.Name} - {p.Quantity} шт. x {p.UnitPrice:N2} грн = {lineTotal:N2} грн");
                    }
                }
                else
                {
                    sb.AppendLine("  (Запчастини не використовувалися)");
                }

                decimal finalPrice = repair.FinalPrice ?? repair.EstimatedPrice;

                sb.AppendLine("--------------------------------------------------");
                sb.AppendLine($"Вартість запчастин: {partsTotal:N2} грн");
                sb.AppendLine($"ЗАГАЛЬНА ВАРТІСТЬ РЕМОНТУ: {finalPrice:N2} грн");
                sb.AppendLine("==================================================");
                sb.AppendLine("Роботи виконані в повному обсязі, претензій немає.");
                sb.AppendLine("\nПідпис клієнта: _______________");
                sb.AppendLine("Підпис майстра:  _______________");

                ProcessDocument("Акт_ремонту_" + repair.RepairId, sb.ToString());
            }
        }

        /// <summary>
        /// Допоміжний метод: пропонує зберегти у файл або роздрукувати
        /// </summary>
        private static void ProcessDocument(string defaultName, string textContent)
        {
            // Спитуємо користувача, що він хоче зробити
            MessageBoxResult answer = MessageBox.Show(
                "Бажаєте зберегти текстовий варіант документа на комп'ютер (наприклад, на Робочий стіл)?\n\n" +
                "(Натисніть 'Так' — для збереження у файл .txt, 'Ні' — щоб відкрити системне вікно друку/PDF)",
                "Вибір формату документа",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (answer == MessageBoxResult.Yes)
            {
                // Відкриваємо вікно збереження файлу
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text file (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.FileName = defaultName + ".txt";
                // За замовчуванням пропонуємо Робочий стіл
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, textContent, Encoding.UTF8);
                    MessageBox.Show("Документ успішно збережено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Одразу відкриваємо створений файл у блокноті, щоб переглянути
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = saveFileDialog.FileName,
                        UseShellExecute = true
                    });
                }
            }
            else if (answer == MessageBoxResult.No)
            {
                // Стандартний віконний друк (де можна вибрати "Microsoft Print to PDF")
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    FlowDocument doc = new FlowDocument(new Paragraph(new Run(textContent)));
                    doc.PagePadding = new Thickness(50);
                    doc.FontFamily = new FontFamily("Courier New");
                    doc.FontSize = 12;

                    IDocumentPaginatorSource idpSource = doc;
                    printDialog.PrintDocument(idpSource.DocumentPaginator, defaultName);
                }
            }
        }
    }
}