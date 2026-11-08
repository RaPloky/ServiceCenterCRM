using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Windows;

namespace Service_Store.Helpers
{
    public static class DatabaseErrorHandler
    {
        public static void Show(Exception exception, string operation)
        {
            try
            {
                string logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
                Directory.CreateDirectory(logDirectory);
                string logPath = Path.Combine(logDirectory, "database-errors.log");
                File.AppendAllText(
                    logPath,
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {operation}{Environment.NewLine}{exception}{Environment.NewLine}{Environment.NewLine}");
            }
            catch
            {

            }

            string message = exception is DbUpdateException
                ? $"{operation}\nНе вдалося зберегти зміни в базі даних."
                : $"{operation}\nНе вдалося підключитися до бази даних або виконати запит.";

            MessageBox.Show(message, "Помилка бази даних", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
