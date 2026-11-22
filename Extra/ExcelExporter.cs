using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Service_Store.Helpers
{
    public static class ExcelExporter
    {
        public static void ExportDataGridToExcel(DataGrid dataGrid, string defaultFileName)
        {
            if (dataGrid == null || dataGrid.Items.Count == 0)
            {
                MessageBox.Show("Немає даних для експорту!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = $"{defaultFileName}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (saveFileDialog.ShowDialog() != true) return;

            try
            {
                // Зчитуємо заголовки та рядки з DataGrid
                var headers = new List<string>();
                var columns = new List<DataGridTextColumn>();

                foreach (var col in dataGrid.Columns)
                {
                    if (col is DataGridTextColumn textCol && col.Header != null)
                    {
                        headers.Add(col.Header.ToString()!);
                        columns.Add(textCol);
                    }
                }

                var rowsData = new List<Dictionary<string, string>>();
                foreach (var item in dataGrid.Items)
                {
                    var rowDict = new Dictionary<string, string>();
                    for (int i = 0; i < columns.Count; i++)
                    {
                        var binding = columns[i].Binding as System.Windows.Data.Binding;
                        string? propName = binding?.Path?.Path;
                        string valStr = "";

                        if (!string.IsNullOrEmpty(propName))
                        {
                            var prop = item.GetType().GetProperty(propName);
                            if (prop != null)
                            {
                                var rawVal = prop.GetValue(item, null);
                                valStr = rawVal?.ToString() ?? "";
                            }
                        }
                        rowDict[headers[i]] = valStr;
                    }
                    rowsData.Add(rowDict);
                }

                using (var package = new ExcelPackage())
                {
                    var ws = package.Workbook.Worksheets.Add("Аналітичний звіт");

                    // 1. Запис основної таблиці (колонки 1 .. headers.Count)
                    for (int c = 0; c < headers.Count; c++)
                    {
                        var cell = ws.Cells[1, c + 1];
                        cell.Value = headers[c];
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(44, 62, 80));
                        cell.Style.Font.Color.SetColor(Color.White);
                        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    for (int r = 0; r < rowsData.Count; r++)
                    {
                        for (int c = 0; c < headers.Count; c++)
                        {
                            var cell = ws.Cells[r + 2, c + 1];
                            string raw = rowsData[r][headers[c]];

                            // Пробуємо зберегти числа як числа, щоб Excel розумів їх у формулах
                            if (decimal.TryParse(raw.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal numVal)
                                && !headers[c].Contains("Чек") && !headers[c].Contains("Заявка") && !headers[c].Contains("ID") && !headers[c].Contains("Артикул"))
                            {
                                cell.Value = numVal;
                            }
                            else
                            {
                                cell.Value = raw;
                            }
                            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.LightGray);
                        }
                    }

                    // 2. Побудова зведеної таблиці та графіка праворуч від основних даних
                    int summaryStartCol = headers.Count + 3; // Відступ 2 порожні колонки

                    if (headers.Contains("Сума (грн)") && headers.Contains("Дата"))
                    {
                        BuildSalesChart(ws, rowsData, summaryStartCol);
                    }
                    else if (headers.Contains("Інженер") && headers.Contains("Заявка №"))
                    {
                        BuildRepairsWorkloadChart(ws, rowsData, summaryStartCol);
                    }
                    else if (headers.Contains("Статус") && headers.Contains("Поточний залишок"))
                    {
                        BuildLowStockChart(ws, rowsData, summaryStartCol);
                    }
                    else if (headers.Contains("Категорія") && headers.Contains("Виручка (грн)"))
                    {
                        BuildCategorySalesChart(ws, rowsData, summaryStartCol);
                    }
                    else if (headers.Contains("Тип несправності") && headers.Contains("Кількість звернень"))
                    {
                        BuildRepairIssuesChart(ws, rowsData, summaryStartCol);
                    }
                    else if (headers.Contains("Показник") && headers.Contains("Сума (грн)"))
                    {
                        BuildPaymentBonusChart(ws, rowsData, summaryStartCol);
                    }
                    else if (headers.Contains("Клієнт") && headers.Contains("Загальна сума (грн)"))
                    {
                        BuildCustomerActivityChart(ws, rowsData, summaryStartCol);
                    }

                    ws.Cells[ws.Dimension.Address].AutoFitColumns();
                    package.SaveAs(new FileInfo(saveFileDialog.FileName));
                }

                MessageBox.Show("Звіт із графіком успішно експортовано у формат .xlsx!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = saveFileDialog.FileName,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при експорті: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ЗВІТ 1: Динаміка виручки за датами (Лінійний графік)
        private static void BuildSalesChart(ExcelWorksheet ws, List<Dictionary<string, string>> rows, int startCol)
        {
            var grouped = rows
                .Select(r =>
                {
                    DateTime.TryParse(r["Дата"], out DateTime dt);
                    decimal.TryParse(r["Сума (грн)"].Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal sum);
                    return new { DateStr = dt != DateTime.MinValue ? dt.ToString("yyyy-MM-dd") : r["Дата"], Sum = sum };
                })
                .GroupBy(x => x.DateStr)
                .OrderBy(g => g.Key)
                .Select(g => new { Date = g.Key, Total = g.Sum(x => x.Sum) })
                .ToList();

            WriteSummaryTable(ws, startCol, "Дата продажу", "Виручка (грн)", grouped.Select(x => (x.Date, x.Total)).ToList());

            var chart = ws.Drawings.AddChart("SalesChart", eChartType.LineMarkers) as ExcelLineChart;
            chart!.Title.Text = "Динаміка виручки від продажів по датах";
            chart.XAxis.Title.Text = "Дата";
            chart.YAxis.Title.Text = "Сума виручки (грн)";
            chart.Legend.Position = eLegendPosition.Bottom;

            int endRow = grouped.Count + 1;
            var series = chart.Series.Add(ws.Cells[2, startCol + 1, endRow, startCol + 1], ws.Cells[2, startCol, endRow, startCol]);
            series.Header = "Виручка (грн)";

            // Увімкнення підписів значень на точках графіка
            var lineSeries = series as ExcelLineChartSerie;
            if (lineSeries != null)
            {
                lineSeries.DataLabel.ShowValue = true;
                lineSeries.DataLabel.Position = eLabelPosition.Top;
            }

            // Розміщуємо графік строго під зведеною таблицею (щоб не перекривав дані)
            chart.SetPosition(endRow + 2, 0, startCol - 1, 0);
            chart.SetSize(650, 380);
        }

        // ЗВІТ 2: Завантаженість інженерів (Стовпчикова діаграма)
        private static void BuildRepairsWorkloadChart(ExcelWorksheet ws, List<Dictionary<string, string>> rows, int startCol)
        {
            var grouped = rows
                .GroupBy(r => string.IsNullOrWhiteSpace(r["Інженер"]) ? "Не призначено" : r["Інженер"])
                .Select(g => new { Engineer = g.Key, Count = (decimal)g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            WriteSummaryTable(ws, startCol, "Інженер / Статус розподілу", "Кількість заявок", grouped.Select(x => (x.Engineer, x.Count)).ToList());

            var chart = ws.Drawings.AddChart("RepairsChart", eChartType.ColumnClustered) as ExcelBarChart;
            chart!.Title.Text = "Розподіл заявок на ремонт між інженерами";
            chart.XAxis.Title.Text = "Відповідальний інженер";
            chart.YAxis.Title.Text = "Кількість ремонтів (шт)";
            chart.Legend.Position = eLegendPosition.Bottom;

            int endRow = grouped.Count + 1;
            var series = chart.Series.Add(ws.Cells[2, startCol + 1, endRow, startCol + 1], ws.Cells[2, startCol, endRow, startCol]);
            series.Header = "Кількість заявок";

            // Увімкнення підписів значень над стовпцями
            var barSeries = series as ExcelBarChartSerie;
            if (barSeries != null)
            {
                barSeries.DataLabel.ShowValue = true;
                barSeries.DataLabel.Position = eLabelPosition.OutEnd;
            }

            chart.SetPosition(endRow + 2, 0, startCol - 1, 0);
            chart.SetSize(650, 380);
        }

        // ЗВІТ 3: Структура критичних залишків на складі (Кругова діаграма)
        private static void BuildLowStockChart(ExcelWorksheet ws, List<Dictionary<string, string>> rows, int startCol)
        {
            var grouped = rows
                .GroupBy(r => r["Статус"])
                .Select(g => new { Status = g.Key, Count = (decimal)g.Count() })
                .ToList();

            WriteSummaryTable(ws, startCol, "Статус залишку", "Кількість позицій", grouped.Select(x => (x.Status, x.Count)).ToList());

            var chart = ws.Drawings.AddChart("StockChart", eChartType.Pie3D) as ExcelPieChart;
            chart!.Title.Text = "Структура дефіцитних товарів на складі";
            chart.Legend.Position = eLegendPosition.Right;

            int endRow = grouped.Count + 1;
            var series = chart.Series.Add(ws.Cells[2, startCol + 1, endRow, startCol + 1], ws.Cells[2, startCol, endRow, startCol]);
            series.Header = "Кількість позицій";

            // Підписи значень і відсотків на секторах діаграми
            var pieSeries = series as ExcelPieChartSerie;
            if (pieSeries != null)
            {
                pieSeries.DataLabel.ShowValue = true;
                pieSeries.DataLabel.ShowPercent = true;
            }

            chart.SetPosition(endRow + 2, 0, startCol - 1, 0);
            chart.SetSize(600, 380);
        }

        private static void BuildCategorySalesChart(ExcelWorksheet ws, List<Dictionary<string, string>> rows, int startCol)
        {
            var data = rows.Select(row => (Label: row["Категорія"], Value: ParseDecimal(row["Виручка (грн)"])))
                .OrderByDescending(item => item.Value).ToList();
            WriteSummaryTable(ws, startCol, "Категорія", "Виручка (грн)", data);
            var chart = ws.Drawings.AddChart("CategorySalesChart", eChartType.Doughnut) as ExcelDoughnutChart;
            chart!.Title.Text = "Частка виручки за категоріями";
            chart.Legend.Position = eLegendPosition.Right;
            var series = chart.Series.Add(ws.Cells[2, startCol + 1, data.Count + 1, startCol + 1], ws.Cells[2, startCol, data.Count + 1, startCol]);
            series.Header = "Виручка (грн)";
            chart.SetPosition(data.Count + 4, 0, startCol - 1, 0);
            chart.SetSize(650, 380);
        }

        private static void BuildRepairIssuesChart(ExcelWorksheet ws, List<Dictionary<string, string>> rows, int startCol)
        {
            var data = rows.Select(row => (Label: row["Тип несправності"], Value: ParseDecimal(row["Кількість звернень"])))
                .OrderByDescending(item => item.Value).ToList();
            WriteSummaryTable(ws, startCol, "Тип несправності", "Кількість звернень", data);
            var chart = ws.Drawings.AddChart("RepairIssuesChart", eChartType.BarClustered) as ExcelBarChart;
            chart!.Title.Text = "Найчастіші несправності";
            chart.XAxis.Title.Text = "Кількість звернень";
            chart.YAxis.Title.Text = "Тип несправності";
            var series = chart.Series.Add(ws.Cells[2, startCol + 1, data.Count + 1, startCol + 1], ws.Cells[2, startCol, data.Count + 1, startCol]);
            series.Header = "Кількість звернень";
            chart.SetPosition(data.Count + 4, 0, startCol - 1, 0);
            chart.SetSize(750, 420);
        }

        private static void BuildPaymentBonusChart(ExcelWorksheet ws, List<Dictionary<string, string>> rows, int startCol)
        {
            var data = rows.Select(row => (Label: row["Показник"], Value: Math.Abs(ParseDecimal(row["Сума (грн)"])))).ToList();
            WriteSummaryTable(ws, startCol, "Показник", "Сума (грн)", data);
            var chart = ws.Drawings.AddChart("PaymentBonusChart", eChartType.Pie3D) as ExcelPieChart;
            chart!.Title.Text = "Структура оплат та бонусних операцій";
            chart.Legend.Position = eLegendPosition.Right;
            var series = chart.Series.Add(ws.Cells[2, startCol + 1, data.Count + 1, startCol + 1], ws.Cells[2, startCol, data.Count + 1, startCol]);
            series.Header = "Сума (грн)";
            chart.SetPosition(data.Count + 4, 0, startCol - 1, 0);
            chart.SetSize(650, 380);
        }

        private static void BuildCustomerActivityChart(ExcelWorksheet ws, List<Dictionary<string, string>> rows, int startCol)
        {
            var data = rows.Select(row => (Label: row["Клієнт"], Value: ParseDecimal(row["Загальна сума (грн)"])))
                .OrderByDescending(item => item.Value).Take(10).ToList();
            WriteSummaryTable(ws, startCol, "Клієнт", "Загальна сума (грн)", data);
            var chart = ws.Drawings.AddChart("CustomerActivityChart", eChartType.ColumnClustered) as ExcelBarChart;
            chart!.Title.Text = "Топ-10 клієнтів за загальною сумою";
            chart.XAxis.Title.Text = "Клієнт";
            chart.YAxis.Title.Text = "Сума (грн)";
            var series = chart.Series.Add(ws.Cells[2, startCol + 1, data.Count + 1, startCol + 1], ws.Cells[2, startCol, data.Count + 1, startCol]);
            series.Header = "Загальна сума (грн)";
            chart.SetPosition(data.Count + 4, 0, startCol - 1, 0);
            chart.SetSize(750, 420);
        }

        private static decimal ParseDecimal(string value)
        {
            return decimal.TryParse(value.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result) ? result : 0;
        }

        // Допоміжний метод для запису акуратної зведеної таблиці
        private static void WriteSummaryTable(ExcelWorksheet ws, int startCol, string col1Header, string col2Header, List<(string Label, decimal Value)> data)
        {
            var h1 = ws.Cells[1, startCol];
            var h2 = ws.Cells[1, startCol + 1];
            h1.Value = col1Header;
            h2.Value = col2Header;

            foreach (var h in new[] { h1, h2 })
            {
                h.Style.Font.Bold = true;
                h.Style.Fill.PatternType = ExcelFillStyle.Solid;
                h.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(39, 174, 96));
                h.Style.Font.Color.SetColor(Color.White);
                h.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            for (int i = 0; i < data.Count; i++)
            {
                var c1 = ws.Cells[i + 2, startCol];
                var c2 = ws.Cells[i + 2, startCol + 1];
                c1.Value = data[i].Label;
                c2.Value = data[i].Value;

                c1.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.LightGray);
                c2.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.LightGray);
            }
        }
    }
}