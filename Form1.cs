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
            Size = new Size(840, 350),
            AutoScroll = true,
            BorderStyle = BorderStyle.FixedSingle,
            Visible = false
        };

        flpGallery = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true
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
            0 => "Введите название аниме (например: noragami)",
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
            ContentType.Anime => "https://kitsu.io/api/edge/anime?filter[text]={название}",
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
                jsonResponse = await GetSearchJsonResponseAsync(searchName, contentType);
            }

            // Показываем галерею с результатами
            ShowGalleryAsync(jsonResponse, contentType);
            lblStatus.Text = "Выберите превью из результатов";
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
        return contentType switch
        {
            ContentType.Anime => await GetJsonFromUrlAsync($"https://kitsu.io/api/edge/anime?filter[text]={Uri.EscapeDataString(searchName.ToLower())}"),
            ContentType.Movie => await GetJsonFromUrlAsync($"http://www.omdbapi.com/?apikey={GetOmdbApiKey()}&t={Uri.EscapeDataString(searchName)}&type=movie"),
            ContentType.Series => await GetJsonFromUrlAsync($"http://www.omdbapi.com/?apikey={GetOmdbApiKey()}&t={Uri.EscapeDataString(searchName)}&type=series"),
            ContentType.Book => await GetJsonFromUrlAsync($"https://openlibrary.org/search.json?title={Uri.EscapeDataString(searchName)}"),
            _ => throw new Exception("Неизвестный тип контента")
        };
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

    private void ShowGalleryAsync(string jsonResponse, ContentType contentType)
    {
        // Очищаем галерею
        flpGallery.Controls.Clear();
        selectedPreviewUrl = null;
        selectedItemName = null;

        List<(string previewUrl, string itemName)> items = ExtractAllPreviews(jsonResponse, contentType);

        if (items.Count == 0)
        {
            MessageBox.Show("Не найдено результатов", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // Показываем галерею
        Label? lblGallery = this.Controls.OfType<Label>().FirstOrDefault(l => l.Text.Contains("Результаты поиска"));
        if (lblGallery != null) lblGallery.Visible = true;
        pnlGallery.Visible = true;
        btnDownloadSelected.Visible = true;

        // Загружаем превью для каждого элемента
        foreach (var (previewUrl, itemName) in items)
        {
            if (string.IsNullOrEmpty(previewUrl)) continue;

            // Создаем контейнер для превью
            Panel itemPanel = new Panel
            {
                Size = new Size(150, 200),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(5),
                Cursor = Cursors.Hand
            };

            // PictureBox для превью
            PictureBox picBox = new PictureBox
            {
                Size = new Size(148, 148),
                Location = new Point(1, 1),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.None
            };

            // Метка с названием
            Label nameLabel = new Label
            {
                Text = itemName.Length > 20 ? itemName.Substring(0, 17) + "..." : itemName,
                Location = new Point(1, 150),
                Size = new Size(148, 49),
                TextAlign = ContentAlignment.TopCenter,
                Font = new Font("Arial", 8)
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
                selectedItemName = itemName;

                // Показываем выбранное превью
                ShowSelectedPreview(previewUrl);
            };

            itemPanel.Click += clickHandler;
            picBox.Click += clickHandler;
            nameLabel.Click += clickHandler;

            itemPanel.Controls.Add(picBox);
            itemPanel.Controls.Add(nameLabel);
            flpGallery.Controls.Add(itemPanel);

            // Загружаем изображение асинхронно
            _ = LoadPreviewImageAsync(picBox, previewUrl);
        }
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

    private List<(string previewUrl, string itemName)> ExtractAllPreviews(string json, ContentType contentType)
    {
        List<(string, string)> results = new List<(string, string)>();

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
                        string? name = item["attributes"]?["canonicalTitle"]?.ToString()
                            ?? item["attributes"]?["titles"]?["en"]?.ToString()
                            ?? item["attributes"]?["slug"]?.ToString()
                            ?? "anime";
                        
                        if (!string.IsNullOrEmpty(previewUrl))
                        {
                            results.Add((previewUrl, name));
                        }
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
                        results.Add((previewUrl, name));
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
                            results.Add((previewUrl, name));
                        }
                    }
                }
            }
        }
        catch
        {
            // Игнорируем ошибки парсинга
        }

        return results;
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
        string encodedName = Uri.EscapeDataString(animeName.ToLower());
        string apiUrl = $"https://kitsu.io/api/edge/anime?filter[text]={encodedName}";
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

