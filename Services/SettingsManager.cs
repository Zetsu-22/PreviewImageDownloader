using Newtonsoft.Json.Linq;
using PreviewImageDownloader.Models;

namespace PreviewImageDownloader.Services;

/// <summary>
/// Класс для управления настройками приложения
/// </summary>
public class SettingsManager
{
    private readonly string settingsPath;

    public SettingsManager(string settingsPath)
    {
        this.settingsPath = settingsPath;
    }

    /// <summary>
    /// Загружает настройки из файла
    /// </summary>
    public AppSettings LoadSettings()
    {
        try
        {
            if (File.Exists(settingsPath))
            {
                string json = File.ReadAllText(settingsPath);
                JObject settings = JObject.Parse(json);
                return new AppSettings
                {
                    OmdbApiKey = settings["OmdbApiKey"]?.ToString() ?? "",
                    KinopoiskApiKey = settings["KinopoiskApiKey"]?.ToString() ?? ""
                };
            }
        }
        catch
        {
            // Игнорируем ошибки загрузки настроек
        }
        return new AppSettings();
    }

    /// <summary>
    /// Сохраняет настройки в файл
    /// </summary>
    public void SaveSettings(AppSettings settings)
    {
        try
        {
            JObject json = new JObject
            {
                ["OmdbApiKey"] = settings.OmdbApiKey,
                ["KinopoiskApiKey"] = settings.KinopoiskApiKey
            };
            File.WriteAllText(settingsPath, json.ToString());
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка сохранения настроек: {ex.Message}", ex);
        }
    }
}

