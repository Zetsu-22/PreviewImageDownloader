using Newtonsoft.Json.Linq;

namespace PreviewImageDownloader;

public partial class SettingsForm : Form
{
    private readonly string settingsPath;
    private TextBox txtOmdbApiKey = null!;
    private Button btnSave = null!;
    private Button btnCancel = null!;
    private LinkLabel lblGetApiKey = null!;

    public SettingsForm(string settingsPath)
    {
        this.settingsPath = settingsPath;
        InitializeComponent();
        LoadSettings();
    }

    private void InitializeComponent()
    {
        this.Text = "Настройки";
        this.Size = new Size(450, 200);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.ShowInTaskbar = false;

        // Ссылка для получения API ключа
        lblGetApiKey = new LinkLabel
        {
            Text = "Получить бесплатный OMDb API ключ",
            Location = new Point(20, 20),
            Size = new Size(400, 20),
            AutoSize = true
        };
        lblGetApiKey.LinkClicked += (s, e) =>
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "http://www.omdbapi.com/apikey.aspx",
                UseShellExecute = true
            });
        };

        // Метка для поля ввода
        Label lblOmdbKey = new Label
        {
            Text = "OMDb API ключ:",
            Location = new Point(20, 55),
            Size = new Size(120, 25)
        };

        // Поле для ввода API ключа
        txtOmdbApiKey = new TextBox
        {
            Location = new Point(20, 80),
            Size = new Size(390, 25),
            PasswordChar = '*'
        };

        // Кнопка Сохранить
        btnSave = new Button
        {
            Text = "Сохранить",
            Location = new Point(235, 120),
            Size = new Size(85, 30),
            DialogResult = DialogResult.OK
        };
        btnSave.Click += BtnSave_Click;

        // Кнопка Отмена
        btnCancel = new Button
        {
            Text = "Отмена",
            Location = new Point(325, 120),
            Size = new Size(85, 30),
            DialogResult = DialogResult.Cancel
        };

        this.AcceptButton = btnSave;
        this.CancelButton = btnCancel;

        // Добавляем элементы на форму
        this.Controls.Add(lblGetApiKey);
        this.Controls.Add(lblOmdbKey);
        this.Controls.Add(txtOmdbApiKey);
        this.Controls.Add(btnSave);
        this.Controls.Add(btnCancel);
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        SaveSettings();
    }

    private void LoadSettings()
    {
        try
        {
            if (File.Exists(settingsPath))
            {
                string json = File.ReadAllText(settingsPath);
                JObject settings = JObject.Parse(json);
                txtOmdbApiKey.Text = settings["OmdbApiKey"]?.ToString() ?? "";
            }
        }
        catch
        {
            // Игнорируем ошибки загрузки настроек
        }
    }

    private void SaveSettings()
    {
        try
        {
            JObject settings = new JObject
            {
                ["OmdbApiKey"] = txtOmdbApiKey.Text
            };
            File.WriteAllText(settingsPath, settings.ToString());
            MessageBox.Show("Настройки сохранены", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка сохранения настроек: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public string GetOmdbApiKey()
    {
        return txtOmdbApiKey.Text;
    }
}

