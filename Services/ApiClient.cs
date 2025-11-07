using PreviewImageDownloader.Services;

namespace PreviewImageDownloader.Services;

/// <summary>
/// Класс для работы с различными API
/// </summary>
public class ApiClient
{
    private readonly SettingsManager settingsManager;

    public ApiClient(SettingsManager settingsManager)
    {
        this.settingsManager = settingsManager;
    }

    /// <summary>
    /// Получает JSON ответ от API
    /// </summary>
    public async Task<string> GetJsonAsync(string url, Dictionary<string, string>? headers = null)
    {
        using (HttpClient client = new HttpClient())
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }

    /// <summary>
    /// Получает JSON ответ от API с использованием HttpRequestMessage (для специальных заголовков)
    /// </summary>
    public async Task<string> GetJsonWithRequestAsync(string url, Dictionary<string, string>? headers = null)
    {
        using (HttpClient client = new HttpClient())
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
    }

    /// <summary>
    /// Скачивает изображение по URL
    /// </summary>
    public async Task DownloadImageAsync(string url, string filePath)
    {
        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(60);
            byte[] imageData = await client.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(filePath, imageData);
        }
    }

    /// <summary>
    /// Получает OMDb API ключ из настроек
    /// </summary>
    public string GetOmdbApiKey()
    {
        var settings = settingsManager.LoadSettings();
        return settings.OmdbApiKey;
    }

    /// <summary>
    /// Получает Kinopoisk API ключ из настроек
    /// </summary>
    public string GetKinopoiskApiKey()
    {
        var settings = settingsManager.LoadSettings();
        return settings.KinopoiskApiKey;
    }
}

