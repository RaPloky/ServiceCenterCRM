using System.Text.Json;
using System.IO;

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
                if (document.RootElement.TryGetProperty("ConnectionStrings", out JsonElement connectionStrings) &&
                    connectionStrings.TryGetProperty("Default", out JsonElement defaultConnectionString) &&
                    !string.IsNullOrWhiteSpace(defaultConnectionString.GetString()))
                {
                    return defaultConnectionString.GetString()!;
                }

                throw new InvalidOperationException("Рядок підключення до бази даних не налаштовано.");
            }
        }
    }
}
