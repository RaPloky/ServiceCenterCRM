using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
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
                Filter = "Excel Workbook (*.xlsx)|*.xls|CSV File (*.csv)|*.csv",
                FileName = defaultFileName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xls",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();

                    // Додаємо заголовки колонок
                    List<string> headers = new List<string>();
                    foreach (var column in dataGrid.Columns)
                    {
                        if (column.Header != null)
                            headers.Add($"\"{column.Header}\"");
                    }
                    sb.AppendLine(string.Join(";", headers));

                    // Додаємо рядки з даними
                    foreach (var item in dataGrid.Items)
                    {
                        List<string> cells = new List<string>();
                        foreach (var column in dataGrid.Columns)
                        {
                            if (column is DataGridTextColumn textCol)
                            {
                                var binding = textCol.Binding as System.Windows.Data.Binding;
                                string propertyName = binding?.Path?.Path;
                                string cellValue = "";

                                if (!string.IsNullOrEmpty(propertyName))
                                {
                                    var prop = item.GetType().GetProperty(propertyName);
                                    if (prop != null)
                                    {
                                        var val = prop.GetValue(item, null);
                                        cellValue = val != null ? val.ToString() : "";
                                    }
                                }
                                cells.Add($"\"{cellValue.Replace("\"", "\"\"")}\"");
                            }
                        }
                        sb.AppendLine(string.Join(";", cells));
                    }

                    // Зберігаємо файл у форматі CSV/Excel
                    File.WriteAllText(saveFileDialog.FileName, sb.ToString(), Encoding.UTF8);

                    MessageBox.Show("Звіт успішно експортовано у файл!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Одразу відкриваємо збережений файл
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
        }
    }
}