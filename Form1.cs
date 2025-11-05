using Newtonsoft.Json.Linq;
using System.Text;
using System.Linq;

namespace PreviewImageDownloader;

public enum ContentType
{
    Anime,
    Movie,
    Series,
    Book
}

public partial class Form1 : Form
{
    private readonly string cachePath;
    private readonly string settingsPath;
    private TextBox txtSearchName = null!;
    private Button btnDownload = null!;
    private Button btnSettings = null!;
    private Label lblStatus = null!;
    private ComboBox cmbContentType = null!;
    private RadioButton rbSearchByName = null!;
    private RadioButton rbApiUrl = null!;
    private TextBox txtApiUrl = null!;
    private PictureBox picPreview = null!;
    private Panel pnlGallery = null!;
    private FlowLayoutPanel flpGallery = null!;
    private Button btnDownloadSelected = null!;
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
    }

    private void InitializeComponent()
    {
        this.Text = "Preview Image Downloader";
        this.Size = new Size(900, 700);
        this.StartPosition = FormStartPosition.CenterScreen;

        // Выбор типа контента
        Label lblContentType = new Label
        {
            Text = "Тип контента:",
            Location = new Point(20, 20),
            Size = new Size(100, 25)
        };

        cmbContentType = new ComboBox
        {
            Location = new Point(130, 18),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbContentType.Items.AddRange(new[] { "Аниме", "Фильм", "Сериал", "Книга" });
        cmbContentType.SelectedIndex = 0;
        cmbContentType.SelectedIndexChanged += CmbContentType_SelectedIndexChanged;

        // Кнопка настроек
        btnSettings = new Button
        {
            Text = "Настройки",
            Location = new Point(350, 17),
            Size = new Size(100, 27)
        };
        btnSettings.Click += BtnSettings_Click;

        // RadioButton для выбора режима
        rbSearchByName = new RadioButton
        {
            Text = "Поиск по названию",
            Location = new Point(20, 60),
            Size = new Size(150, 25),
            Checked = true
        };

        rbApiUrl = new RadioButton
        {
            Text = "По API URL",
            Location = new Point(180, 60),
            Size = new Size(120, 25)
        };

        // Поле для поиска по названию
        Label lblSearchName = new Label
        {
            Text = "Название:",
            Location = new Point(20, 95),
            Size = new Size(100, 25)
        };

        txtSearchName = new TextBox
        {
            Location = new Point(20, 120),
            Size = new Size(640, 25),
            Enabled = true
        };

        // Поле для API URL
        Label lblApiUrl = new Label
        {
            Text = "API URL:",
            Location = new Point(20, 95),
            Size = new Size(100, 25),
            Visible = false
        };

        txtApiUrl = new TextBox
        {
            Location = new Point(20, 120),
            Size = new Size(640, 25),
            Enabled = false,
            Visible = false
        };

        // Кнопка скачивания
        btnDownload = new Button
        {
            Text = "Скачать превью",
            Location = new Point(20, 160),
            Size = new Size(200, 35)
        };
        btnDownload.Click += BtnDownload_Click;

        // Статус
        lblStatus = new Label
        {
            Text = "Готов к работе",
            Location = new Point(20, 205),
            Size = new Size(640, 25)
        };

        // Панель для галереи превью
        Label lblGallery = new Label
        {
            Text = "Результаты поиска (выберите превью):",
            Location = new Point(20, 240),
            Size = new Size(300, 25),
            Visible = false
        };

        pnlGallery = new Panel
        {
            Location = new Point(20, 265),
            Size = new Size(840, 380),
            AutoScroll = true,
            BorderStyle = BorderStyle.FixedSingle,
            Visible = false
        };

        flpGallery = new FlowLayoutPanel
        {
            Location = new Point(0, 0),
            AutoSize = true,
            AutoScroll = false,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        pnlGallery.Controls.Add(flpGallery);

        // Кнопка скачивания выбранного превью
        btnDownloadSelected = new Button
        {
            Text = "Скачать выбранное превью",
            Location = new Point(20, 625),
            Size = new Size(200, 35),
            Visible = false
        };
        btnDownloadSelected.Click += BtnDownloadSelected_Click;

        // PictureBox для превью (скрыт по умолчанию)
        Label lblPreview = new Label
        {
            Text = "Выбранное превью:",
            Location = new Point(240, 625),
            Size = new Size(150, 25),
            Visible = false
        };

        picPreview = new PictureBox
        {
            Location = new Point(240, 650),
            Size = new Size(150, 210),
            SizeMode = PictureBoxSizeMode.Zoom,
            BorderStyle = BorderStyle.FixedSingle,
            Visible = false
        };

        // Обработчики для RadioButton
        rbSearchByName.CheckedChanged += (s, e) =>
        {
            txtSearchName.Enabled = rbSearchByName.Checked;
            txtSearchName.Visible = rbSearchByName.Checked;
            txtApiUrl.Enabled = rbApiUrl.Checked;
            txtApiUrl.Visible = rbApiUrl.Checked;
            lblSearchName.Visible = rbSearchByName.Checked;
            lblApiUrl.Visible = rbApiUrl.Checked;
        };

        rbApiUrl.CheckedChanged += (s, e) =>
        {
            txtSearchName.Enabled = rbSearchByName.Checked;
            txtSearchName.Visible = rbSearchByName.Checked;
            txtApiUrl.Enabled = rbApiUrl.Checked;
            txtApiUrl.Visible = rbApiUrl.Checked;
            lblSearchName.Visible = rbSearchByName.Checked;
            lblApiUrl.Visible = rbApiUrl.Checked;
            
            // Устанавливаем шаблон URL при выборе режима API URL
            if (rbApiUrl.Checked)
            {
                UpdateApiUrlTemplate();
            }
        };

        // Добавляем элементы на форму
        this.Controls.Add(lblContentType);
        this.Controls.Add(cmbContentType);
        this.Controls.Add(btnSettings);
        this.Controls.Add(rbSearchByName);
        this.Controls.Add(rbApiUrl);
        this.Controls.Add(lblSearchName);
        this.Controls.Add(txtSearchName);
        this.Controls.Add(lblApiUrl);
        this.Controls.Add(txtApiUrl);
        this.Controls.Add(btnDownload);
        this.Controls.Add(lblStatus);
        this.Controls.Add(lblGallery);
        this.Controls.Add(pnlGallery);
        this.Controls.Add(btnDownloadSelected);
        this.Controls.Add(lblPreview);
        this.Controls.Add(picPreview);

        // Обработчик закрытия формы для сохранения настроек
        this.FormClosing += Form1_FormClosing;
    }

    private void CmbContentType_SelectedIndexChanged(object? sender, EventArgs e)
    {
        // Обновляем подсказки в зависимости от типа контента
        string placeholder = cmbContentType.SelectedIndex switch
        {
            0 => "Введите название аниме на русском или английском (например: моя геройская академия или my hero academia)",
            1 => "Введите название фильма (например: Fight Club)",
            2 => "Введите название сериала (например: Breaking Bad)",
            3 => "Введите название книги (например: The Witcher)",
            _ => "Введите название"
        };
        txtSearchName.PlaceholderText = placeholder;
        
        // Обновляем шаблон URL если активен режим API URL
        if (rbApiUrl.Checked)
        {
            UpdateApiUrlTemplate();
        }
    }

    private void UpdateApiUrlTemplate()
    {
        ContentType contentType = (ContentType)cmbContentType.SelectedIndex;
        string apiKey = GetOmdbApiKey();
        
        string template = contentType switch
        {
            ContentType.Anime => "https://kitsu.io/api/edge/anime?filter[text]={название_на_любом_языке}",
            ContentType.Movie => string.IsNullOrEmpty(apiKey) 
                ? "http://www.omdbapi.com/?apikey={ваш_ключ}&t={название_фильма}&type=movie"
                : $"http://www.omdbapi.com/?apikey={apiKey}&t={{название_фильма}}&type=movie",
            ContentType.Series => string.IsNullOrEmpty(apiKey)
                ? "http://www.omdbapi.com/?apikey={ваш_ключ}&t={название_сериала}&type=series"
                : $"http://www.omdbapi.com/?apikey={apiKey}&t={{название_сериала}}&type=series",
            ContentType.Book => "https://openlibrary.org/search.json?title={название_книги}",
            _ => ""
        };
        
        txtApiUrl.Text = template;
    }

    private void BtnSettings_Click(object? sender, EventArgs e)
    {
        using (SettingsForm settingsForm = new SettingsForm(settingsPath))
        {
            if (settingsForm.ShowDialog(this) == DialogResult.OK)
            {
                // Настройки сохранены в форме SettingsForm
                // Обновляем шаблон URL если активен режим API URL
                if (rbApiUrl.Checked)
                {
                    UpdateApiUrlTemplate();
                }
            }
        }
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

    private async void BtnDownload_Click(object? sender, EventArgs e)
    {
        try
        {
            btnDownload.Enabled = false;
            lblStatus.Text = "Обработка...";

            // Скрываем галерею
            HideGallery();

            ContentType contentType = (ContentType)cmbContentType.SelectedIndex;
            string jsonResponse;

            if (rbApiUrl.Checked)
            {
                string apiUrl = txtApiUrl.Text.Trim();
                if (string.IsNullOrEmpty(apiUrl))
                {
                    MessageBox.Show("Введите API URL", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                lblStatus.Text = "Загрузка данных из API...";
                lastSearchQuery = null; // Сбрасываем для API URL
                jsonResponse = await DownloadJsonAsync(apiUrl);
            }
            else
            {
                string searchName = txtSearchName.Text.Trim();
                if (string.IsNullOrEmpty(searchName))
                {
                    MessageBox.Show("Введите название", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
            }

            lblStatus.Text = "Загрузка данных из API...";
                lastSearchQuery = searchName; // Сохраняем запрос для ранжирования
                jsonResponse = await GetSearchJsonResponseAsync(searchName, contentType);
            }

            // Показываем галерею с результатами
            ShowGalleryAsync(jsonResponse, contentType);
            lblStatus.Text = "Результаты отсортированы по релевантности. Выберите превью из списка.";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblStatus.Text = $"Ошибка: {ex.Message}";
            HideGallery();
        }
        finally
        {
            btnDownload.Enabled = true;
        }
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
            ContentType.Movie => await GetJsonFromUrlAsync($"http://www.omdbapi.com/?apikey={GetOmdbApiKey()}&t={Uri.EscapeDataString(searchName)}&type=movie"),
            ContentType.Series => await GetJsonFromUrlAsync($"http://www.omdbapi.com/?apikey={GetOmdbApiKey()}&t={Uri.EscapeDataString(searchName)}&type=series"),
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

    private async Task<string> GetJsonFromUrlAsync(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }

    private string? lastSearchQuery = null;

    private void ShowGalleryAsync(string jsonResponse, ContentType contentType)
    {
        // Очищаем галерею
        flpGallery.Controls.Clear();
        selectedPreviewUrl = null;
        selectedItemName = null;

        // Используем улучшенную версию с ранжированием, если есть поисковый запрос
        List<(string previewUrl, string itemName, string displayName, string officialName, int relevanceScore)> itemsWithScore;
        
        if (!string.IsNullOrEmpty(lastSearchQuery))
        {
            itemsWithScore = ExtractAllPreviewsWithScore(jsonResponse, contentType, lastSearchQuery);
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
                // OMDb возвращает один результат, но проверим на ошибку
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
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
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

