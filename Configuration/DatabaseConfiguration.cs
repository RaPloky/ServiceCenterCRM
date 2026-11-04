using System;
using System.IO;
using System.Text.Json;

namespace Service_Center.Configuration
{
    public static class DatabaseConfiguration
    {
        public static string ConnectionString
        {
            get
            {
                string path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException("Файл конфігурації бази даних не знайдено.", path);
                }

                using JsonDocument document = JsonDocument.Parse(File.ReadAllText(path));
                if (!document.RootElement.TryGetProperty("ConnectionStrings", out JsonElement connectionStrings) ||
                    !connectionStrings.TryGetProperty("Default", out JsonElement defaultConnection) ||
                    string.IsNullOrWhiteSpace(defaultConnection.GetString()))
                {
                    throw new InvalidOperationException("У файлі конфігурації відсутній рядок підключення до бази даних.");
                }

                return defaultConnection.GetString()!;
            }
        }
    }
}
