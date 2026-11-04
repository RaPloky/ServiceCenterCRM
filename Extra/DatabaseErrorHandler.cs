using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace Service_Store.Helpers
{
    public static class DatabaseErrorHandler
    {
        public static void Show(Exception exception, string operation)
        {
            string message = exception is DbUpdateException
                ? "Зміни не вдалося зберегти до бази даних."
                : "Не вдалося виконати операцію з базою даних.";

            MessageBox.Show(
                $"{operation}\n\n{message}\n{exception.Message}",
                "Помилка бази даних",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
