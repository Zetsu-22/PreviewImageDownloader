using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace PreviewImageDownloader.Forms;

public partial class SettingsForm : Form
{
    private readonly string settingsPath;

    public SettingsForm(string settingsPath)
    {
        this.settingsPath = settingsPath;
        InitializeComponent();
        LoadSettings();
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        SaveSettings();
    }

    private void LblGetApiKey_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "http://www.omdbapi.com/apikey.aspx",
            UseShellExecute = true
        });
    }

    private void LblGetKinopoiskApiKey_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "https://kinopoisk.dev/documentation",
            UseShellExecute = true
        });
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
                txtKinopoiskApiKey.Text = settings["KinopoiskApiKey"]?.ToString() ?? "";
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
                ["OmdbApiKey"] = txtOmdbApiKey.Text,
                ["KinopoiskApiKey"] = txtKinopoiskApiKey.Text
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

    public string GetKinopoiskApiKey()
    {
        return txtKinopoiskApiKey.Text;
    }
}

