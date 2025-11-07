using Newtonsoft.Json.Linq;
using System.Text;
using System.Linq;
using PreviewImageDownloader.Models;

namespace PreviewImageDownloader.Forms;

public partial class Form1 : Form
{
    private readonly string cachePath;
    private readonly string settingsPath;
    
    private string? selectedTitle = null;
    private string? selectedTitleOfficial = null;
    private string? selectedPreviewUrl = null;
    private string? selectedItemName = null;

    public Form1()
    {
        InitializeComponent();
        
        // Определяем путь к папке cache в Obsidian
        string obsidianPath = @"C:\Users\Doffy\Documents\локальное хранилище Obsidian\Obsidian";
        cachePath = Path.Combine(obsidianPath, "cache");
        settingsPath = Path.Combine(Application.StartupPath, "settings.json");
        
        // Создаем папку если её нет
        if (!Directory.Exists(cachePath))
        {
            Directory.CreateDirectory(cachePath);
        }

        // Загружаем настройки
        LoadSettings();
        
        // Form1_Load уже подписан в InitializeComponent
    }

    private void Form1_Load(object? sender, EventArgs e)
    {
        // Инициализируем Items для ComboBox (не в дизайнере)
        if (cmbContentType != null && cmbContentType.Items.Count == 0)
        {
            cmbContentType.Items.AddRange(new[] { "Аниме", "Фильм", "Сериал", "Книга" });
            cmbContentType.SelectedIndex = 0;
        }
        // Инициализируем список API после полной загрузки формы
        UpdateCoverApiOptions();
    }

    private void UpdateCoverApiOptions()
    {
        // Не выполняем в режиме дизайнера
        if (DesignMode || cmbCoverApi == null || cmbContentType == null) return;
        
        cmbCoverApi.Items.Clear();
        ContentType contentType = (ContentType)cmbContentType.SelectedIndex;
        
        switch (contentType)
        {
            case ContentType.Anime:
                cmbCoverApi.Items.AddRange(new[] { "Kitsu API", "Jikan API" });
                break;
            case ContentType.Movie:
            case ContentType.Series:
                List<string> movieApis = new List<string> { "OMDb API" };
                // Проверяем наличие ключа только во время выполнения, не в дизайнере
                if (!DesignMode && !string.IsNullOrEmpty(GetKinopoiskApiKey()))
                {
                    movieApis.Insert(0, "Kinopoisk API");
                }
                cmbCoverApi.Items.AddRange(movieApis.ToArray());
                break;
            case ContentType.Book:
                cmbCoverApi.Items.AddRange(new[] { "Open Library API" });
                break;
        }
        if (cmbCoverApi.Items.Count > 0)
        {
            cmbCoverApi.SelectedIndex = 0;
        }
    }

    private void CmbContentType_SelectedIndexChanged(object? sender, EventArgs e)
    {
        // Обновляем список доступных API для обложки (только во время выполнения)
        if (!DesignMode)
        {
            UpdateCoverApiOptions();
        }
    }

    private void BtnSettings_Click(object? sender, EventArgs e)
    {
        using (SettingsForm settingsForm = new SettingsForm(settingsPath))
        {
            if (settingsForm.ShowDialog(this) == DialogResult.OK)
            {
                // Настройки сохранены, обновляем список API
                UpdateCoverApiOptions();
            }
        }
    }

    // ========== ЭТАП 1: ПОИСК НАЗВАНИЯ ЧЕРЕЗ КИНОПОИСК ==========
    private async void BtnSearchTitle_Click(object? sender, EventArgs e)
    {
        try
        {
            btnSearchTitle.Enabled = false;
            lblStatus.Text = "Поиск названия через Кинопоиск...";
            
            string searchQuery = txtSearchTitle.Text.Trim();
            if (string.IsNullOrEmpty(searchQuery))
            {
                MessageBox.Show("Введите название для поиска", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Очищаем предыдущие результаты
            flpTitleResults.Controls.Clear();
            selectedTitle = null;
            selectedTitleOfficial = null;
            lblSelectedTitle.Text = "Выбранное название: не выбрано";
            lblSelectedTitle.ForeColor = Color.Gray;

            // Поиск через Кинопоиск API
            string kinopoiskApiKey = GetKinopoiskApiKey();
            if (string.IsNullOrEmpty(kinopoiskApiKey))
            {
                MessageBox.Show("Kinopoisk API ключ не настроен. Нажмите 'Настройки' для ввода ключа.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string url = $"https://api.kinopoisk.dev/v1.4/movie/search?query={Uri.EscapeDataString(searchQuery)}&limit=20";
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                ["X-API-KEY"] = kinopoiskApiKey
            };
            
            string jsonResponse = await GetJsonFromUrlAsync(url, headers);
            
            // Логируем ответ от Кинопоиска для отладки
            string logsDir = Path.Combine(Application.StartupPath, "logs");
            if (!Directory.Exists(logsDir))
            {
                Directory.CreateDirectory(logsDir);
            }
            
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string logFileName = $"kinopoisk_response_{timestamp}_{Uri.EscapeDataString(searchQuery)}.json";
            string logPath = Path.Combine(logsDir, logFileName);
            
            try
            {
                // Сохраняем полный ответ
                File.WriteAllText(logPath, jsonResponse, Encoding.UTF8);
                
                // Также сохраняем в общий файл логов с временной меткой
                string generalLogPath = Path.Combine(logsDir, "kinopoisk_responses.log");
                string separator = new string('=', 80);
                string logEntry = $"\n{separator}\n" +
                                 $"Время: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                                 $"Запрос: {searchQuery}\n" +
                                 $"URL: {url}\n" +
                                 $"Размер ответа: {jsonResponse.Length} символов\n" +
                                 $"Файл: {logFileName}\n" +
                                 $"{separator}\n" +
                                 $"{jsonResponse}\n";
                File.AppendAllText(generalLogPath, logEntry, Encoding.UTF8);
                
                // Показываем информацию пользователю
                lblStatus.Text = $"Ответ сохранен в: {logPath}";
                System.Diagnostics.Debug.WriteLine($"Ответ от Кинопоиска сохранен в: {logPath}");
                
                // Опционально: открываем файл в блокноте
                // System.Diagnostics.Process.Start("notepad.exe", logPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Не удалось сохранить ответ: {ex.Message}");
                lblStatus.Text = $"Ошибка сохранения лога: {ex.Message}";
            }
            
            JObject jsonObj = JObject.Parse(jsonResponse);
            JArray? docsArray = jsonObj["docs"] as JArray;

            if (docsArray == null || docsArray.Count == 0)
            {
                MessageBox.Show("Не найдено результатов", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblStatus.Text = "Результаты не найдены";
                return;
            }

            // Отображаем результаты
            foreach (JToken doc in docsArray)
            {
                string? name = doc["name"]?.ToString();
                string? alternativeName = doc["alternativeName"]?.ToString();
                string? enName = doc["enName"]?.ToString();
                
                if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(alternativeName) && string.IsNullOrEmpty(enName))
                    continue;

                // Создаем кнопку для выбора названия
                Button titleButton = new Button
                {
                    Text = $"{name ?? alternativeName ?? enName}\n{(string.IsNullOrEmpty(enName) ? alternativeName : enName)}",
                    Size = new Size(200, 60),
                    Margin = new Padding(5),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Arial", 9)
                };

                string displayName = name ?? alternativeName ?? enName ?? "Unknown";

                // Сохраняем весь объект doc в Tag кнопки для доступа ко всем названиям
                titleButton.Tag = doc;

                titleButton.Click += (s, args) =>
                {
                    // Выделяем выбранную кнопку
                    foreach (Control ctrl in flpTitleResults.Controls)
                    {
                        if (ctrl is Button btn)
                        {
                            btn.BackColor = SystemColors.Control;
                            btn.ForeColor = SystemColors.ControlText;
                        }
                    }
                    titleButton.BackColor = Color.LightBlue;
                    titleButton.ForeColor = Color.DarkBlue;

                    // Показываем диалог выбора названия
                    if (titleButton.Tag is JToken docToken)
                    {
                        using (TitleSelectionForm selectionForm = new TitleSelectionForm(docToken))
                        {
                            if (selectionForm.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(selectionForm.SelectedTitle))
                            {
                                selectedTitle = displayName; // Для отображения используем основное название
                                selectedTitleOfficial = selectionForm.SelectedTitle; // Для поиска используем выбранное название
                                lblSelectedTitle.Text = $"Выбранное название: {displayName} → {selectionForm.SelectedTitle}";
                                lblSelectedTitle.ForeColor = Color.Black;

                                // Активируем этап 2
                                GroupBox? grpStep2 = this.Controls.OfType<GroupBox>().FirstOrDefault(g => g.Text.Contains("Шаг 2"));
                                if (grpStep2 != null)
                                {
                                    grpStep2.Enabled = true;
                                    btnSearchCover.Enabled = true;
                                    lblStatus.Text = $"Готово к поиску обложки для: {selectionForm.SelectedTitle}";
                                    lblStatus.ForeColor = Color.Black;
                                }
                            }
                        }
                    }
                };

                flpTitleResults.Controls.Add(titleButton);
            }

            lblStatus.Text = $"Найдено результатов: {docsArray.Count}. Выберите точное название.";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при поиске: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblStatus.Text = $"Ошибка: {ex.Message}";
        }
        finally
        {
            btnSearchTitle.Enabled = true;
        }
    }

    // ========== ЭТАП 2: ПОИСК ОБЛОЖКИ ПО ВЫБРАННОМУ НАЗВАНИЮ ==========
    private async void BtnSearchCover_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(selectedTitle) && string.IsNullOrEmpty(selectedTitleOfficial))
        {
            MessageBox.Show("Сначала выберите название в шаге 1", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            btnSearchCover.Enabled = false;
            lblStatus.Text = "Поиск обложки...";
            HideGallery();

            ContentType contentType = (ContentType)cmbContentType.SelectedIndex;
            string selectedApi = cmbCoverApi.SelectedItem?.ToString() ?? "";

            // Используем официальное название для поиска
            string searchName = selectedTitleOfficial ?? selectedTitle ?? "";

            string jsonResponse = await GetCoverSearchResponseAsync(searchName, contentType, selectedApi);
            
            // Показываем галерею с результатами
            ShowGalleryAsync(jsonResponse, contentType);
            lblStatus.Text = "Результаты отсортированы по релевантности. Выберите обложку из списка.";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblStatus.Text = $"Ошибка: {ex.Message}";
            HideGallery();
        }
        finally
        {
            btnSearchCover.Enabled = true;
        }
    }

    private async Task<string> GetCoverSearchResponseAsync(string searchName, ContentType contentType, string selectedApi)
    {
        switch (contentType)
        {
            case ContentType.Anime:
                if (selectedApi == "Kitsu API")
                {
                    return await GetJsonFromUrlAsync($"https://kitsu.io/api/edge/anime?filter[text]={Uri.EscapeDataString(searchName)}&page[limit]=20");
                }
                else if (selectedApi == "Jikan API")
                {
                    string jikanResponse = await GetJsonFromUrlAsync($"https://api.jikan.moe/v4/anime?q={Uri.EscapeDataString(searchName)}&limit=20");
                    return ConvertJikanToKitsuFormat(jikanResponse);
                }
                break;

            case ContentType.Movie:
            case ContentType.Series:
                if (selectedApi == "Kinopoisk API")
                {
                    string kinopoiskApiKey = GetKinopoiskApiKey();
                    if (!string.IsNullOrEmpty(kinopoiskApiKey))
                    {
                        string url = $"https://api.kinopoisk.dev/v1.4/movie/search?query={Uri.EscapeDataString(searchName)}&limit=20";
                        Dictionary<string, string> headers = new Dictionary<string, string>
                        {
                            ["X-API-KEY"] = kinopoiskApiKey
                        };
                        return await GetJsonFromUrlAsync(url, headers);
                    }
                }
                else if (selectedApi == "OMDb API")
                {
                    string type = contentType == ContentType.Movie ? "movie" : "series";
                    return await GetJsonFromUrlAsync($"http://www.omdbapi.com/?apikey={GetOmdbApiKey()}&t={Uri.EscapeDataString(searchName)}&type={type}");
                }
                break;

            case ContentType.Book:
                return await GetJsonFromUrlAsync($"https://openlibrary.org/search.json?title={Uri.EscapeDataString(searchName)}");
        }

        throw new Exception("Неизвестный тип контента или API");
    }

    private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
    {
        // Настройки уже сохраняются в SettingsForm при нажатии "Сохранить"
        // Здесь можно добавить дополнительную логику сохранения при необходимости
    }

    private void LoadSettings()
    {
        // Загружаем настройки при запуске (если нужно для отображения)
        // Основная загрузка теперь в SettingsForm
    }


    private async Task<string> GetSearchJsonResponseAsync(string searchName, ContentType contentType)
    {
        if (contentType == ContentType.Anime)
        {
            // Комбинированный поиск: пробуем оба API и объединяем результаты
            List<JToken> allResults = new List<JToken>();
            
            // 1. Сначала через Kitsu API
            try
            {
                string kitsuUrl = $"https://kitsu.io/api/edge/anime?filter[text]={Uri.EscapeDataString(searchName)}&page[limit]=20";
                string kitsuResponse = await GetJsonFromUrlAsync(kitsuUrl);
                JObject kitsuJson = JObject.Parse(kitsuResponse);
                JArray? kitsuData = kitsuJson["data"] as JArray;
                
                if (kitsuData != null && kitsuData.Count > 0)
                {
                    foreach (JToken item in kitsuData)
                    {
                        allResults.Add(item);
                    }
                }
            }
            catch
            {
                // Игнорируем ошибки Kitsu
            }
            
            // 2. Если Kitsu не дал результатов или дал мало, пробуем Jikan только с английским вариантом
            if (allResults.Count < 5)
            {
                try
                {
                    // Определяем английский вариант для популярных запросов
                    string englishVariant = searchName;
                    if (searchName.ToLower().Contains("моя геройская") || searchName.ToLower().Contains("геройская академия") || searchName.ToLower().Contains("герой"))
                    {
                        englishVariant = "my hero academia";
                    }
                    else if (searchName.ToLower().Contains("атака титанов") || searchName.ToLower().Contains("титаны") || searchName.ToLower().Contains("титан"))
                    {
                        englishVariant = "attack on titan";
                    }
                    
                    // Пробуем только один запрос к Jikan с английским вариантом, если он отличается от оригинала
                    if (englishVariant != searchName)
                    {
                        string jikanUrl = $"https://api.jikan.moe/v4/anime?q={Uri.EscapeDataString(englishVariant)}&limit=20";
                        string jikanResponse = await GetJsonFromUrlAsync(jikanUrl);
                        string convertedJikan = ConvertJikanToKitsuFormat(jikanResponse);
                        JObject jikanJson = JObject.Parse(convertedJikan);
                        JArray? jikanData = jikanJson["data"] as JArray;
                        
                        if (jikanData != null && jikanData.Count > 0)
                        {
                            // Добавляем только те результаты, которых нет в Kitsu
                            foreach (JToken jikanItem in jikanData)
                            {
                                string? jikanTitle = jikanItem["attributes"]?["canonicalTitle"]?.ToString();
                                bool exists = false;
                                
                                foreach (JToken kitsuItem in allResults)
                                {
                                    string? kitsuTitle = kitsuItem["attributes"]?["canonicalTitle"]?.ToString();
                                    if (kitsuTitle != null && jikanTitle != null && 
                                        kitsuTitle.Equals(jikanTitle, StringComparison.OrdinalIgnoreCase))
                                    {
                                        exists = true;
                                        break;
                                    }
                                }
                                
                                if (!exists)
                                {
                                    allResults.Add(jikanItem);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // Игнорируем ошибки Jikan
                }
            }
            
            // 3. Если есть результаты, возвращаем их
            if (allResults.Count > 0)
            {
                JObject result = new JObject
                {
                    ["data"] = new JArray(allResults.ToArray())
                };
                return result.ToString();
            }
            
            // 4. Если результатов нет, пробуем еще раз только через Jikan
            try
            {
                string jikanUrl = $"https://api.jikan.moe/v4/anime?q={Uri.EscapeDataString(searchName)}&limit=20";
                string jikanResponse = await GetJsonFromUrlAsync(jikanUrl);
                return ConvertJikanToKitsuFormat(jikanResponse);
            }
            catch (Exception ex)
            {
                throw new Exception($"Не удалось найти аниме. Попробуйте:\n1. Использовать английское название\n2. Использовать API URL напрямую\n3. Проверить правильность написания\n\nОшибка: {ex.Message}");
            }
        }
        
        return contentType switch
        {
            ContentType.Movie => await GetMovieSearchResponseAsync(searchName),
            ContentType.Series => await GetSeriesSearchResponseAsync(searchName),
            ContentType.Book => await GetJsonFromUrlAsync($"https://openlibrary.org/search.json?title={Uri.EscapeDataString(searchName)}"),
            _ => throw new Exception("Неизвестный тип контента")
        };
    }

    private string ConvertJikanToKitsuFormat(string jikanJson)
    {
        try
        {
            JObject jikanObj = JObject.Parse(jikanJson);
            JArray? jikanData = jikanObj["data"] as JArray;
            
            if (jikanData == null || jikanData.Count == 0)
            {
                return "{\"data\":[]}";
            }

            JArray kitsuData = new JArray();
            
            foreach (JToken item in jikanData)
            {
                JObject kitsuItem = new JObject
                {
                    ["id"] = item["mal_id"]?.ToString() ?? "",
                    ["type"] = "anime",
                    ["attributes"] = new JObject
                    {
                        ["canonicalTitle"] = item["title"]?.ToString() ?? item["title_english"]?.ToString() ?? "",
                        ["titles"] = new JObject
                        {
                            ["en"] = item["title_english"]?.ToString() ?? item["title"]?.ToString(),
                            ["en_jp"] = item["title"]?.ToString(),
                            ["ja_jp"] = item["title_japanese"]?.ToString()
                        },
                        ["posterImage"] = new JObject
                        {
                            ["original"] = item["images"]?["jpg"]?["image_url"]?.ToString() 
                                ?? item["images"]?["webp"]?["image_url"]?.ToString() 
                                ?? ""
                        }
                    }
                };
                kitsuData.Add(kitsuItem);
            }

            JObject result = new JObject
            {
                ["data"] = kitsuData
            };

            return result.ToString();
        }
        catch
        {
            return "{\"data\":[]}";
        }
    }

    private async Task<string> GetJsonFromUrlAsync(string url, Dictionary<string, string>? headers = null)
    {
        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            
            // Используем HttpRequestMessage для более точного контроля заголовков
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                // Добавляем заголовки, если они указаны
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

    private void ShowGalleryAsync(string jsonResponse, ContentType contentType)
    {
        // Очищаем галерею
        flpGallery.Controls.Clear();
        selectedPreviewUrl = null;
        selectedItemName = null;

        // Используем выбранное название для ранжирования
        string searchQuery = selectedTitleOfficial ?? selectedTitle ?? "";
        List<(string previewUrl, string itemName, string displayName, string officialName, int relevanceScore)> itemsWithScore;
        
        if (!string.IsNullOrEmpty(searchQuery))
        {
            itemsWithScore = ExtractAllPreviewsWithScore(jsonResponse, contentType, searchQuery);
            }
            else
            {
            // Если нет поискового запроса, используем старый метод
            var itemsWithoutScore = ExtractAllPreviews(jsonResponse, contentType);
            itemsWithScore = itemsWithoutScore.Select(i => (i.Item1, i.Item2, i.Item3, i.Item4, 0)).ToList();
        }
        
        // Преобразуем в формат для отображения
        var items = itemsWithScore.Select(i => (i.Item1, i.Item2, i.Item3, i.Item4)).ToList();

        if (items.Count == 0)
        {
            MessageBox.Show("Не найдено результатов", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

        // Показываем галерею
        Label? lblGallery = this.Controls.OfType<Label>().FirstOrDefault(l => l.Text.Contains("Результаты поиска"));
        if (lblGallery != null)
        {
            lblGallery.Visible = true;
            lblGallery.Text = $"Результаты поиска (выберите превью): Найдено {items.Count}";
        }
        pnlGallery.Visible = true;
        btnDownloadSelected.Visible = true;

        // Загружаем превью для каждого элемента
        int itemIndex = 0;
        foreach (var (previewUrl, itemName, displayName, officialName) in items)
        {
            if (string.IsNullOrEmpty(previewUrl)) continue;
            itemIndex++;

            // Создаем контейнер для превью (увеличиваем высоту для отображения дополнительной информации)
            Panel itemPanel = new Panel
            {
                Size = new Size(180, 240),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(5),
                Cursor = Cursors.Hand
            };

            // PictureBox для превью (загружаем изображение только при выборе)
            PictureBox picBox = new PictureBox
            {
                Size = new Size(178, 130),
                Location = new Point(1, 1),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.None,
                BackColor = Color.LightGray
            };
            
            // Показываем плейсхолдер вместо загрузки изображения
            Label placeholderLabel = new Label
            {
                Text = "Нажмите для\nзагрузки\nпревью",
                Location = new Point(1, 1),
                Size = new Size(178, 130),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 8),
                ForeColor = Color.Gray,
                BackColor = Color.LightGray
            };
            picBox.Controls.Add(placeholderLabel);

            // Метка с отображаемым названием (может быть русским)
            Label displayLabel = new Label
            {
                Text = displayName.Length > 25 ? displayName.Substring(0, 22) + "..." : displayName,
                Location = new Point(1, 132),
                Size = new Size(178, 35),
                TextAlign = ContentAlignment.TopCenter,
                Font = new Font("Arial", 8, FontStyle.Bold),
                ForeColor = Color.Black
            };

            // Метка с официальным названием (английское/японское)
            Label officialLabel = new Label
            {
                Text = "Офф: " + (officialName.Length > 22 ? officialName.Substring(0, 19) + "..." : officialName),
                Location = new Point(1, 167),
                Size = new Size(178, 35),
                TextAlign = ContentAlignment.TopCenter,
                Font = new Font("Arial", 7),
                ForeColor = Color.DarkBlue
            };

            // Метка с информацией для API
            Label apiInfoLabel = new Label
            {
                Text = "→ API: " + officialName,
                Location = new Point(1, 202),
                Size = new Size(178, 37),
                TextAlign = ContentAlignment.TopCenter,
                Font = new Font("Courier New", 6),
                ForeColor = Color.Gray
            };

            // Обработчик клика
            EventHandler clickHandler = (s, e) =>
            {
                // Убираем выделение с других элементов
                foreach (Control ctrl in flpGallery.Controls)
                {
                    if (ctrl is Panel panel)
                    {
                        panel.BackColor = Color.White;
                    }
                }

                // Выделяем выбранный элемент
                itemPanel.BackColor = Color.LightBlue;
                selectedPreviewUrl = previewUrl;
                selectedItemName = itemName; // Используем официальное название для имени файла

                // Загружаем превью только при выборе
                if (picBox.Image == null && picBox.Tag != null)
                {
                    // Убираем плейсхолдер
                    foreach (Control ctrl in picBox.Controls)
                    {
                        if (ctrl is Label)
                        {
                            picBox.Controls.Remove(ctrl);
                            ctrl.Dispose();
                            break;
                        }
                    }
                    // Загружаем изображение
                    _ = LoadPreviewImageAsync(picBox, previewUrl);
                }

                // Показываем выбранное превью
                ShowSelectedPreview(previewUrl);
            };

            itemPanel.Click += clickHandler;
            picBox.Click += clickHandler;
            displayLabel.Click += clickHandler;
            officialLabel.Click += clickHandler;
            apiInfoLabel.Click += clickHandler;

            itemPanel.Controls.Add(picBox);
            itemPanel.Controls.Add(displayLabel);
            itemPanel.Controls.Add(officialLabel);
            itemPanel.Controls.Add(apiInfoLabel);
            
            // Добавляем элемент в галерею
            flpGallery.Controls.Add(itemPanel);

            // НЕ загружаем изображение сразу - только при клике или выборе
            // Сохраняем URL для последующей загрузки
            picBox.Tag = previewUrl;
        }
        
        // Принудительно обновляем галерею для отображения всех элементов
        flpGallery.PerformLayout();
        
        // Обновляем размер FlowLayoutPanel вручную для правильного отображения
        int rows = (int)Math.Ceiling((double)items.Count / 4.0); // Примерно 4 элемента в ряд
        int height = rows * 250; // 240px высота + 10px отступы
        flpGallery.Height = Math.Max(height, pnlGallery.Height);
        flpGallery.Width = pnlGallery.Width - 20; // Учитываем скроллбар
        
        pnlGallery.Invalidate();
        
        lblStatus.Text = $"Найдено результатов: {items.Count}. Выберите превью из списка.";
    }

    private async Task LoadPreviewImageAsync(PictureBox picBox, string url)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                byte[] imageData = await client.GetByteArrayAsync(url);
                using (MemoryStream ms = new MemoryStream(imageData))
                {
                    picBox.Image = new Bitmap(ms);
                }
            }
        }
        catch
        {
            // Игнорируем ошибки загрузки изображения
            picBox.BackColor = Color.LightGray;
        }
    }

    private void ShowSelectedPreview(string previewUrl)
    {
        Label? lblPreview = this.Controls.OfType<Label>().FirstOrDefault(l => l.Text.Contains("Выбранное превью"));
        if (lblPreview != null) lblPreview.Visible = true;
        picPreview.Visible = true;

        _ = LoadPreviewImageAsync(picPreview, previewUrl);
    }

    private void HideGallery()
    {
        Label? lblGallery = this.Controls.OfType<Label>().FirstOrDefault(l => l.Text.Contains("Результаты поиска"));
        if (lblGallery != null) lblGallery.Visible = false;
        pnlGallery.Visible = false;
        btnDownloadSelected.Visible = false;
        
        Label? lblPreview = this.Controls.OfType<Label>().FirstOrDefault(l => l.Text.Contains("Выбранное превью"));
        if (lblPreview != null) lblPreview.Visible = false;
        picPreview.Visible = false;
            picPreview.Image?.Dispose();
        picPreview.Image = null;
    }

    private List<(string previewUrl, string itemName, string displayName, string officialName, int relevanceScore)> ExtractAllPreviewsWithScore(string json, ContentType contentType, string searchQuery)
    {
        List<(string, string, string, string, int)> results = new List<(string, string, string, string, int)>();

        try
        {
            JObject jsonObj = JObject.Parse(json);

            if (contentType == ContentType.Anime)
            {
                JArray? dataArray = jsonObj["data"] as JArray;
                if (dataArray != null)
                {
                    foreach (JToken item in dataArray)
                    {
                        string? previewUrl = item["attributes"]?["posterImage"]?["original"]?.ToString();
                        if (string.IsNullOrEmpty(previewUrl)) continue;

                        // Извлекаем все варианты названий
                        JToken? titles = item["attributes"]?["titles"];
                        string? enTitle = titles?["en"]?.ToString();
                        string? enJpTitle = titles?["en_jp"]?.ToString();
                        string? jaJpTitle = titles?["ja_jp"]?.ToString();
                        string? canonicalTitle = item["attributes"]?["canonicalTitle"]?.ToString();
                        string? slug = item["attributes"]?["slug"]?.ToString();
                        
                        // Приоритет для официального названия: en_jp > en > canonicalTitle
                        string officialName = enJpTitle ?? enTitle ?? canonicalTitle ?? "Unknown";
                        
                        // Для отображения используем canonicalTitle или первый доступный
                        string displayName = canonicalTitle ?? enTitle ?? enJpTitle ?? jaJpTitle ?? "Unknown";
                        
                        // Для имени файла используем officialName (английское или японское)
                        string fileName = SanitizeFileName(officialName);
                        
                        // Вычисляем релевантность (score)
                        int relevanceScore = CalculateRelevanceScore(searchQuery, displayName, officialName, enTitle, enJpTitle, canonicalTitle, slug);
                        
                        results.Add((previewUrl, fileName, displayName, officialName, relevanceScore));
                    }
                }
            }
            else if (contentType == ContentType.Movie || contentType == ContentType.Series)
            {
                // Проверяем, это Kinopoisk API (массив docs) или OMDb API (один объект)
                JArray? docsArray = jsonObj["docs"] as JArray;
                if (docsArray != null)
                {
                    // Kinopoisk API формат
                    foreach (JToken doc in docsArray)
                    {
                        JToken? poster = doc["poster"];
                        string? previewUrl = poster?["url"]?.ToString();
                        if (string.IsNullOrEmpty(previewUrl)) continue;

                        string? name = doc["name"]?.ToString();
                        string? alternativeName = doc["alternativeName"]?.ToString();
                        string? enName = doc["enName"]?.ToString();
                        
                        // Для отображения используем русское название
                        string displayName = name ?? alternativeName ?? enName ?? "Unknown";
                        // Для официального названия используем английское или альтернативное
                        string officialName = enName ?? alternativeName ?? name ?? "Unknown";
                        
                        string fileName = SanitizeFileName(officialName);
                        int score = CalculateRelevanceScore(searchQuery, displayName, officialName, enName, null, alternativeName, null);
                        results.Add((previewUrl, fileName, displayName, officialName, score));
                    }
                }
                else
                {
                    // OMDb API формат - возвращает один результат
                    if (jsonObj["Response"]?.ToString() != "False")
                    {
                        string? previewUrl = jsonObj["Poster"]?.ToString();
                        if (previewUrl != "N/A" && !string.IsNullOrEmpty(previewUrl))
                        {
                            string? name = jsonObj["Title"]?.ToString() ?? "movie";
                            string fileName = SanitizeFileName(name);
                            int score = CalculateRelevanceScore(searchQuery, name, name, name, null, name, null);
                            results.Add((previewUrl, fileName, name, name, score));
                        }
                    }
                }
            }
            else if (contentType == ContentType.Book)
            {
                JArray? docsArray = jsonObj["docs"] as JArray;
                if (docsArray != null)
                {
                    foreach (JToken doc in docsArray)
                    {
                        int? coverId = doc["cover_i"]?.ToObject<int?>();
                        string? name = doc["title"]?.ToString() ?? "book";
                        
                        if (coverId.HasValue)
                        {
                            string previewUrl = $"https://covers.openlibrary.org/b/id/{coverId}-L.jpg";
                            string fileName = SanitizeFileName(name);
                            int score = CalculateRelevanceScore(searchQuery, name, name, name, null, name, null);
                            results.Add((previewUrl, fileName, name, name, score));
                        }
                    }
                }
            }
        }
        catch
        {
            // Игнорируем ошибки парсинга
        }

        // Сортируем по релевантности (больший score = выше)
        results.Sort((a, b) => b.Item5.CompareTo(a.Item5));

        return results;
    }

    private int CalculateRelevanceScore(string searchQuery, string? displayName, string? officialName, string? enTitle, string? enJpTitle, string? canonicalTitle, string? slug)
    {
        if (string.IsNullOrEmpty(searchQuery)) return 0;
        
        int score = 0;
        string searchLower = searchQuery.ToLowerInvariant().Trim();
        
        // Проверяем точное совпадение (максимальный балл)
        if (displayName != null && displayName.Equals(searchQuery, StringComparison.OrdinalIgnoreCase))
            score += 10000;
        if (officialName != null && officialName.Equals(searchQuery, StringComparison.OrdinalIgnoreCase))
            score += 10000;
        
        // Проверяем начало названия (очень высокий приоритет)
        if (displayName != null && displayName.StartsWith(searchQuery, StringComparison.OrdinalIgnoreCase))
            score += 5000;
        if (officialName != null && officialName.StartsWith(searchQuery, StringComparison.OrdinalIgnoreCase))
            score += 5000;
        
        // Проверяем, содержит ли название все слова из запроса
        string[] searchWords = searchLower.Split(new[] { ' ', '-', '_', ',', '.', '!' }, StringSplitOptions.RemoveEmptyEntries);
        int allWordsMatch = 0;
        int partialWordsMatch = 0;
        
        foreach (string word in searchWords)
        {
            if (word.Length < 2) continue; // Пропускаем слишком короткие слова
            
            bool wordFound = false;
            
            if (displayName != null && displayName.ToLowerInvariant().Contains(word))
            {
                wordFound = true;
                allWordsMatch++;
            }
            if (officialName != null && officialName.ToLowerInvariant().Contains(word))
            {
                wordFound = true;
                allWordsMatch++;
            }
            if (enTitle != null && enTitle.ToLowerInvariant().Contains(word))
            {
                wordFound = true;
                allWordsMatch++;
            }
            if (enJpTitle != null && enJpTitle.ToLowerInvariant().Contains(word))
            {
                wordFound = true;
                allWordsMatch++;
            }
            if (canonicalTitle != null && canonicalTitle.ToLowerInvariant().Contains(word))
            {
                wordFound = true;
                allWordsMatch++;
            }
            if (slug != null && slug.ToLowerInvariant().Contains(word))
            {
                wordFound = true;
                allWordsMatch++;
            }
            
            if (!wordFound)
            {
                // Частичное совпадение (например, "hero" в "heroic")
                if (displayName != null && displayName.ToLowerInvariant().IndexOf(word) >= 0)
                    partialWordsMatch++;
                if (officialName != null && officialName.ToLowerInvariant().IndexOf(word) >= 0)
                    partialWordsMatch++;
            }
        }
        
        // Большой бонус, если все слова найдены
        if (allWordsMatch >= searchWords.Length * 2) // Умножаем на 2, т.к. проверяем несколько полей
            score += 3000;
        else if (allWordsMatch > 0)
            score += allWordsMatch * 200;
        
        // Меньший бонус за частичные совпадения
        score += partialWordsMatch * 50;
        
        // Специальная проверка для популярных аниме (My Hero Academia, Attack on Titan и т.д.)
        // Проверяем ключевые слова из русского запроса
        if (searchLower.Contains("герой") || searchLower.Contains("hero"))
        {
            if (officialName != null && (officialName.ToLowerInvariant().Contains("hero") && officialName.ToLowerInvariant().Contains("academia")))
                score += 3000;
            if (displayName != null && (displayName.ToLowerInvariant().Contains("hero") && displayName.ToLowerInvariant().Contains("academia")))
                score += 3000;
        }
        
        if (searchLower.Contains("академия") || searchLower.Contains("academia"))
        {
            if (officialName != null && officialName.ToLowerInvariant().Contains("academia"))
                score += 1500;
            if (displayName != null && displayName.ToLowerInvariant().Contains("academia"))
                score += 1500;
        }
        
        // Проверка для Attack on Titan
        if (searchLower.Contains("титан") || searchLower.Contains("titan"))
        {
            if (officialName != null && officialName.ToLowerInvariant().Contains("titan"))
                score += 2000;
            if (displayName != null && displayName.ToLowerInvariant().Contains("titan"))
                score += 2000;
        }
        
        return score;
    }

    private List<(string previewUrl, string itemName, string displayName, string officialName)> ExtractAllPreviews(string json, ContentType contentType)
    {
        // Для обратной совместимости, вызываем новую версию без релевантности
        return ExtractAllPreviewsWithScore(json, contentType, "").Select(r => (r.Item1, r.Item2, r.Item3, r.Item4)).ToList();
    }

    private async void BtnDownloadSelected_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(selectedPreviewUrl) || string.IsNullOrEmpty(selectedItemName))
        {
            MessageBox.Show("Выберите превью из галереи", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            btnDownloadSelected.Enabled = false;
            lblStatus.Text = "Скачивание изображения...";

            string fileName = SanitizeFileName($"{selectedItemName}_preview.jpg");
            string filePath = Path.Combine(cachePath, fileName);

            await DownloadImageAsync(selectedPreviewUrl, filePath);

            lblStatus.Text = $"Готово! Сохранено: {fileName}";
            MessageBox.Show($"Превью успешно скачано!\nПуть: {filePath}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblStatus.Text = $"Ошибка: {ex.Message}";
        }
        finally
        {
            btnDownloadSelected.Enabled = true;
        }
    }

    private async Task<(string? previewUrl, string itemName)> SearchAndExtractDataAsync(string searchName, ContentType contentType)
    {
        return contentType switch
        {
            ContentType.Anime => await SearchAnimeAsync(searchName),
            ContentType.Movie => await SearchMovieAsync(searchName),
            ContentType.Series => await SearchSeriesAsync(searchName),
            ContentType.Book => await SearchBookAsync(searchName),
            _ => (null, "item")
        };
    }

    private (string? previewUrl, string itemName) ExtractDataFromApiUrl(string jsonResponse, ContentType contentType)
    {
        return contentType switch
        {
            ContentType.Anime => ExtractAnimeData(jsonResponse),
            ContentType.Movie => ExtractOmdbData(jsonResponse),
            ContentType.Series => ExtractOmdbData(jsonResponse),
            ContentType.Book => ExtractBookData(jsonResponse),
            _ => (null, "item")
        };
    }

    // Методы для аниме (Kitsu API)
    private async Task<(string? previewUrl, string itemName)> SearchAnimeAsync(string animeName)
    {
        // Не используем ToLower() - Kitsu API поддерживает поиск на русском и других языках
        // Увеличиваем лимит для получения большего количества результатов
        string encodedName = Uri.EscapeDataString(animeName);
        string apiUrl = $"https://kitsu.io/api/edge/anime?filter[text]={encodedName}&page[limit]=20";
        string jsonResponse = await DownloadJsonAsync(apiUrl);
        return ExtractAnimeData(jsonResponse);
    }

    private (string? previewUrl, string itemName) ExtractAnimeData(string json)
    {
        try
        {
            JObject jsonObj = JObject.Parse(json);
            JToken? data = jsonObj["data"]?[0];
            if (data == null) return (null, "anime");

            string? previewUrl = data["attributes"]?["posterImage"]?["original"]?.ToString();
            string? name = data["attributes"]?["canonicalTitle"]?.ToString() 
                ?? data["attributes"]?["titles"]?["en"]?.ToString()
                ?? data["attributes"]?["slug"]?.ToString()
                ?? "anime";

            return (previewUrl, name);
        }
        catch
        {
            return (null, "anime");
        }
    }

    // Методы для фильмов и сериалов (OMDb API)
    private async Task<(string? previewUrl, string itemName)> SearchMovieAsync(string movieName)
    {
        string apiKey = GetOmdbApiKey();
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("OMDb API ключ не настроен. Нажмите 'Настройки' для ввода ключа.");
        }

        string encodedName = Uri.EscapeDataString(movieName);
        string apiUrl = $"http://www.omdbapi.com/?apikey={apiKey}&t={encodedName}&type=movie";
        string jsonResponse = await DownloadJsonAsync(apiUrl);
        return ExtractOmdbData(jsonResponse);
    }

    private async Task<(string? previewUrl, string itemName)> SearchSeriesAsync(string seriesName)
    {
        string apiKey = GetOmdbApiKey();
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("OMDb API ключ не настроен. Нажмите 'Настройки' для ввода ключа.");
        }

        string encodedName = Uri.EscapeDataString(seriesName);
        string apiUrl = $"http://www.omdbapi.com/?apikey={apiKey}&t={encodedName}&type=series";
        string jsonResponse = await DownloadJsonAsync(apiUrl);
        return ExtractOmdbData(jsonResponse);
    }

    private (string? previewUrl, string itemName) ExtractOmdbData(string json)
    {
        try
        {
            JObject jsonObj = JObject.Parse(json);
            
            // Проверяем на ошибку
            if (jsonObj["Response"]?.ToString() == "False")
            {
                string error = jsonObj["Error"]?.ToString() ?? "Неизвестная ошибка";
                throw new Exception($"OMDb API ошибка: {error}");
            }

            string? previewUrl = jsonObj["Poster"]?.ToString();
            if (previewUrl == "N/A" || string.IsNullOrEmpty(previewUrl))
            {
                return (null, "movie");
            }

            string? name = jsonObj["Title"]?.ToString() ?? "movie";
            return (previewUrl, name);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("OMDb API"))
                throw;
            return (null, "movie");
        }
    }

    // Методы для книг (Open Library API)
    private async Task<(string? previewUrl, string itemName)> SearchBookAsync(string bookName)
    {
        string encodedName = Uri.EscapeDataString(bookName);
        string apiUrl = $"https://openlibrary.org/search.json?title={encodedName}";
        string jsonResponse = await DownloadJsonAsync(apiUrl);
        return ExtractBookData(jsonResponse);
    }

    private (string? previewUrl, string itemName) ExtractBookData(string json)
    {
        try
        {
            JObject jsonObj = JObject.Parse(json);
            JToken? firstDoc = jsonObj["docs"]?[0];
            if (firstDoc == null) return (null, "book");

            // Получаем cover_i
            int? coverId = firstDoc["cover_i"]?.ToObject<int?>();
            string? name = firstDoc["title"]?.ToString() ?? "book";

            if (!coverId.HasValue)
            {
                return (null, name);
            }

            // Формируем URL превью (используем размер L - большой)
            string previewUrl = $"https://covers.openlibrary.org/b/id/{coverId}-L.jpg";
            return (previewUrl, name);
        }
        catch
        {
            return (null, "book");
        }
    }

    private string GetOmdbApiKey()
    {
        try
        {
            if (File.Exists(settingsPath))
            {
                string json = File.ReadAllText(settingsPath);
                JObject settings = JObject.Parse(json);
                return settings["OmdbApiKey"]?.ToString() ?? "";
            }
        }
        catch
        {
            // Игнорируем ошибки
        }
        return "";
    }

    private string GetKinopoiskApiKey()
    {
        try
        {
            if (File.Exists(settingsPath))
            {
                string json = File.ReadAllText(settingsPath);
                JObject settings = JObject.Parse(json);
                return settings["KinopoiskApiKey"]?.ToString() ?? "";
            }
        }
        catch
        {
            // Игнорируем ошибки
        }
        return "";
    }

    private string SanitizeFileName(string fileName)
    {
        string sanitized = fileName.ToLower().Replace(" ", "_");
        sanitized = string.Join("", sanitized.Split(Path.GetInvalidFileNameChars()));
        return sanitized;
    }

    private async Task<string> DownloadJsonAsync(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            
            // Авторизация для Kinopoisk API:
            // Токен НЕ передается в URL, а добавляется в HTTP-заголовок X-API-KEY
            // Это происходит автоматически для каждого запроса к api.kinopoisk.dev
            // Отдельный запрос для авторизации НЕ требуется
            if (url.Contains("api.kinopoisk.dev"))
            {
                string apiKey = GetKinopoiskApiKey();
                if (!string.IsNullOrEmpty(apiKey))
                {
                    // Используем HttpRequestMessage для добавления заголовка X-API-KEY
                    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
                    {
                        // Токен встраивается в заголовок запроса, а не в URL
                        request.Headers.Add("X-API-KEY", apiKey);
                        HttpResponseMessage kinopoiskResponse = await client.SendAsync(request);
                        kinopoiskResponse.EnsureSuccessStatusCode();
                        return await kinopoiskResponse.Content.ReadAsStringAsync();
                    }
                }
            }
            
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }


    private async Task<string> GetMovieSearchResponseAsync(string searchName)
    {
        string kinopoiskApiKey = GetKinopoiskApiKey();
        
        // Пробуем сначала Kinopoisk API, если есть ключ
        if (!string.IsNullOrEmpty(kinopoiskApiKey))
        {
            try
            {
                string url = $"https://api.kinopoisk.dev/v1.4/movie/search?query={Uri.EscapeDataString(searchName)}&limit=20";
                Dictionary<string, string> headers = new Dictionary<string, string>
                {
                    ["X-API-KEY"] = kinopoiskApiKey
                };
                return await GetJsonFromUrlAsync(url, headers);
            }
            catch
            {
                // Если Kinopoisk не сработал, пробуем OMDb
            }
        }
        
        // Fallback на OMDb API
        return await GetJsonFromUrlAsync($"http://www.omdbapi.com/?apikey={GetOmdbApiKey()}&t={Uri.EscapeDataString(searchName)}&type=movie");
    }

    private async Task<string> GetSeriesSearchResponseAsync(string searchName)
    {
        string kinopoiskApiKey = GetKinopoiskApiKey();
        
        // Пробуем сначала Kinopoisk API, если есть ключ
        if (!string.IsNullOrEmpty(kinopoiskApiKey))
        {
            try
            {
                string url = $"https://api.kinopoisk.dev/v1.4/movie/search?query={Uri.EscapeDataString(searchName)}&limit=20";
                Dictionary<string, string> headers = new Dictionary<string, string>
                {
                    ["X-API-KEY"] = kinopoiskApiKey
                };
                return await GetJsonFromUrlAsync(url, headers);
            }
            catch
            {
                // Если Kinopoisk не сработал, пробуем OMDb
            }
        }
        
        // Fallback на OMDb API
        return await GetJsonFromUrlAsync($"http://www.omdbapi.com/?apikey={GetOmdbApiKey()}&t={Uri.EscapeDataString(searchName)}&type=series");
    }

    private async Task DownloadImageAsync(string url, string filePath)
    {
        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(60);
            byte[] imageData = await client.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(filePath, imageData);
        }
    }
}

