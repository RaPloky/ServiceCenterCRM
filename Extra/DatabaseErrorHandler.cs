using Microsoft.EntityFrameworkCore;
using System;
using System.Windows;

namespace Service_Store.Helpers
{
    public static class DatabaseErrorHandler
    {
        public static void Show(Exception exception, string operation)
        {
            string message = exception is DbUpdateException
                ? $"{operation}\nНе вдалося зберегти зміни в базі даних."
                : $"{operation}\nНе вдалося підключитися до бази даних або виконати запит.";

            MessageBox.Show(message, "Помилка бази даних", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
