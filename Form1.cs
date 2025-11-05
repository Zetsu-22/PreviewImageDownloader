using Newtonsoft.Json.Linq;
using System.Text;

namespace PreviewImageDownloader;

public partial class Form1 : Form
{
    private readonly string cachePath;
    private TextBox txtApiUrl;
    private TextBox txtAnimeName;
    private Button btnDownload;
    private Label lblStatus;
    private RadioButton rbApiUrl;
    private RadioButton rbAnimeName;
    private PictureBox picPreview;

    public Form1()
    {
        InitializeComponent();
        
        // Определяем путь к папке cache в Obsidian
        string obsidianPath = @"C:\Users\Doffy\Documents\локальное хранилище Obsidian\Obsidian";
        cachePath = Path.Combine(obsidianPath, "cache");
        
        // Создаем папку если её нет
        if (!Directory.Exists(cachePath))
        {
            Directory.CreateDirectory(cachePath);
        }
    }

    private void InitializeComponent()
    {
        this.Text = "Preview Image Downloader";
        this.Size = new Size(600, 500);
        this.StartPosition = FormStartPosition.CenterScreen;

        // RadioButton для выбора режима
        rbApiUrl = new RadioButton
        {
            Text = "API URL",
            Location = new Point(20, 20),
            Size = new Size(100, 25),
            Checked = true
        };

        rbAnimeName = new RadioButton
        {
            Text = "Название аниме",
            Location = new Point(130, 20),
            Size = new Size(150, 25)
        };

        // Поле для API URL
        Label lblApiUrl = new Label
        {
            Text = "API URL:",
            Location = new Point(20, 60),
            Size = new Size(100, 25)
        };

        txtApiUrl = new TextBox
        {
            Location = new Point(20, 85),
            Size = new Size(540, 25),
            Text = "https://kitsu.io/api/edge/anime?filter[text]="
        };

        // Поле для названия аниме
        Label lblAnimeName = new Label
        {
            Text = "Название аниме:",
            Location = new Point(20, 120),
            Size = new Size(150, 25)
        };

        txtAnimeName = new TextBox
        {
            Location = new Point(20, 145),
            Size = new Size(540, 25),
            Enabled = false
        };

        // Кнопка скачивания
        btnDownload = new Button
        {
            Text = "Скачать превью",
            Location = new Point(20, 180),
            Size = new Size(200, 35)
        };
        btnDownload.Click += BtnDownload_Click;

        // Статус
        lblStatus = new Label
        {
            Text = "Готов к работе",
            Location = new Point(20, 225),
            Size = new Size(540, 25)
        };

        // PictureBox для превью
        Label lblPreview = new Label
        {
            Text = "Превью:",
            Location = new Point(20, 260),
            Size = new Size(100, 25)
        };

        picPreview = new PictureBox
        {
            Location = new Point(20, 285),
            Size = new Size(200, 280),
            SizeMode = PictureBoxSizeMode.Zoom,
            BorderStyle = BorderStyle.FixedSingle
        };

        // Обработчики для RadioButton
        rbApiUrl.CheckedChanged += (s, e) =>
        {
            txtApiUrl.Enabled = rbApiUrl.Checked;
            txtAnimeName.Enabled = rbAnimeName.Checked;
        };

        rbAnimeName.CheckedChanged += (s, e) =>
        {
            txtApiUrl.Enabled = rbApiUrl.Checked;
            txtAnimeName.Enabled = rbAnimeName.Checked;
        };

        // Добавляем элементы на форму
        this.Controls.Add(rbApiUrl);
        this.Controls.Add(rbAnimeName);
        this.Controls.Add(lblApiUrl);
        this.Controls.Add(txtApiUrl);
        this.Controls.Add(lblAnimeName);
        this.Controls.Add(txtAnimeName);
        this.Controls.Add(btnDownload);
        this.Controls.Add(lblStatus);
        this.Controls.Add(lblPreview);
        this.Controls.Add(picPreview);
    }

    private async void BtnDownload_Click(object? sender, EventArgs e)
    {
        try
        {
            btnDownload.Enabled = false;
            lblStatus.Text = "Обработка...";

            string apiUrl;
            string animeName;

            if (rbApiUrl.Checked)
            {
                apiUrl = txtApiUrl.Text.Trim();
                if (string.IsNullOrEmpty(apiUrl))
                {
                    MessageBox.Show("Введите API URL", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                animeName = txtAnimeName.Text.Trim();
                if (string.IsNullOrEmpty(animeName))
                {
                    MessageBox.Show("Введите название аниме", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                // Формируем URL из названия
                string encodedName = Uri.EscapeDataString(animeName.ToLower());
                apiUrl = $"https://kitsu.io/api/edge/anime?filter[text]={encodedName}";
            }

            // Получаем JSON
            lblStatus.Text = "Загрузка данных из API...";
            string jsonResponse = await DownloadJsonAsync(apiUrl);

            // Парсим JSON и извлекаем URL превью
            string? previewUrl = ExtractPreviewUrl(jsonResponse);
            if (string.IsNullOrEmpty(previewUrl))
            {
                MessageBox.Show("Не удалось найти превью в ответе API", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Ошибка: превью не найдено";
                return;
            }

            // Извлекаем название аниме из JSON для имени файла
            animeName = ExtractAnimeName(jsonResponse);
            if (string.IsNullOrEmpty(animeName))
            {
                animeName = "anime";
            }

            // Скачиваем изображение
            lblStatus.Text = "Скачивание изображения...";
            string fileName = $"{animeName.ToLower().Replace(" ", "_")}_preview.jpg";
            string filePath = Path.Combine(cachePath, fileName);

            await DownloadImageAsync(previewUrl, filePath);

            // Показываем превью
            picPreview.Image?.Dispose();
            picPreview.Image = new Bitmap(filePath);

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
            btnDownload.Enabled = true;
        }
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

    private string? ExtractPreviewUrl(string json)
    {
        try
        {
            JObject jsonObj = JObject.Parse(json);
            JToken? data = jsonObj["data"]?[0];
            if (data == null) return null;

            string? previewUrl = data["attributes"]?["posterImage"]?["original"]?.ToString();
            return previewUrl;
        }
        catch
        {
            return null;
        }
    }

    private string ExtractAnimeName(string json)
    {
        try
        {
            JObject jsonObj = JObject.Parse(json);
            JToken? data = jsonObj["data"]?[0];
            if (data == null) return "anime";

            // Пробуем разные варианты названий
            string? name = data["attributes"]?["canonicalTitle"]?.ToString();
            if (string.IsNullOrEmpty(name))
            {
                name = data["attributes"]?["titles"]?["en"]?.ToString();
            }
            if (string.IsNullOrEmpty(name))
            {
                name = data["attributes"]?["slug"]?.ToString();
            }

            // Очищаем название от спецсимволов
            if (!string.IsNullOrEmpty(name))
            {
                name = name.Replace(" ", "_").Replace("/", "_").Replace("\\", "_");
                name = string.Join("", name.Split(Path.GetInvalidFileNameChars()));
            }

            return name ?? "anime";
        }
        catch
        {
            return "anime";
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
