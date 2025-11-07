using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace PreviewImageDownloader.Forms;

[DesignerCategory("")] // Отключаем дизайнер, так как UI создается программно
public partial class TitleSelectionForm : Form
{
    private ListBox lstTitles = null!;
    private Button btnSelect = null!;
    private Button btnCancel = null!;
    private Label lblInfo = null!;
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? SelectedTitle { get; private set; }

    public TitleSelectionForm(JToken doc)
    {
        InitializeComponent();
        LoadTitles(doc);
    }

    private void InitializeComponent()
    {
        this.Text = "Выберите название";
        this.Size = new Size(600, 500);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        lblInfo = new Label
        {
            Text = "Доступные названия (выберите одно для поиска обложки):",
            Location = new Point(15, 15),
            Size = new Size(560, 25),
            Font = new Font("Arial", 9, FontStyle.Bold)
        };

        lstTitles = new ListBox
        {
            Location = new Point(15, 45),
            Size = new Size(560, 350),
            Font = new Font("Arial", 9),
            SelectionMode = SelectionMode.One
        };
        lstTitles.SelectedIndexChanged += (s, e) => btnSelect.Enabled = lstTitles.SelectedIndex >= 0;
        lstTitles.DoubleClick += (s, e) => { if (lstTitles.SelectedIndex >= 0) SelectTitle(); };

        btnSelect = new Button
        {
            Text = "Выбрать",
            Location = new Point(400, 410),
            Size = new Size(85, 35),
            Enabled = false,
            DialogResult = DialogResult.OK
        };
        btnSelect.Click += (s, e) => SelectTitle();

        btnCancel = new Button
        {
            Text = "Отмена",
            Location = new Point(490, 410),
            Size = new Size(85, 35),
            DialogResult = DialogResult.Cancel
        };

        this.AcceptButton = btnSelect;
        this.CancelButton = btnCancel;

        this.Controls.Add(lblInfo);
        this.Controls.Add(lstTitles);
        this.Controls.Add(btnSelect);
        this.Controls.Add(btnCancel);
    }

    private void LoadTitles(JToken doc)
    {
        List<(string title, string description)> titles = new List<(string, string)>();

        // Основные поля
        string? name = doc["name"]?.ToString();
        string? alternativeName = doc["alternativeName"]?.ToString();
        string? enName = doc["enName"]?.ToString();

        if (!string.IsNullOrEmpty(name))
        {
            titles.Add((name, "Русское название (name)"));
        }

        if (!string.IsNullOrEmpty(alternativeName))
        {
            titles.Add((alternativeName, "Альтернативное название (alternativeName)"));
        }

        if (!string.IsNullOrEmpty(enName))
        {
            titles.Add((enName, "Английское название (enName)"));
        }

        // Массив names
        JArray? namesArray = doc["names"] as JArray;
        if (namesArray != null)
        {
            foreach (JToken nameItem in namesArray)
            {
                string? titleName = nameItem["name"]?.ToString();
                string? language = nameItem["language"]?.ToString();
                string? type = nameItem["type"]?.ToString();

                if (!string.IsNullOrEmpty(titleName))
                {
                    // Пропускаем дубликаты
                    if (titles.Any(t => t.title == titleName))
                        continue;

                    string description = "Из массива names";
                    if (!string.IsNullOrEmpty(language))
                        description += $" (язык: {language})";
                    if (!string.IsNullOrEmpty(type))
                        description += $" - {type}";
                    else if (string.IsNullOrEmpty(language))
                        description += " (оригинальное)";

                    titles.Add((titleName, description));
                }
            }
        }

        // Отображаем в ListBox
        foreach (var (title, description) in titles)
        {
            lstTitles.Items.Add($"{title} | {description}");
        }

        // Выбираем английское название по умолчанию, если есть
        if (titles.Count > 0)
        {
            // Ищем английское название
            int englishIndex = -1;
            
            // Сначала ищем в массиве names название с типом "Original title on kinopoisk"
            if (namesArray != null)
            {
                for (int i = 0; i < titles.Count; i++)
                {
                    var (title, desc) = titles[i];
                    if (desc.Contains("Original title on kinopoisk"))
                    {
                        // Проверяем, что это действительно английское название (латинские буквы, не японские символы)
                        if (title.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c)) &&
                            !title.Any(c => c >= 0x3040 && c <= 0x309F) && // Хирагана
                            !title.Any(c => c >= 0x30A0 && c <= 0x30FF))    // Катакана
                        {
                            englishIndex = i;
                            break;
                        }
                    }
                }
            }
            
            // Если не нашли, ищем enName
            if (englishIndex < 0 && !string.IsNullOrEmpty(enName))
            {
                for (int i = 0; i < titles.Count; i++)
                {
                    var (title, desc) = titles[i];
                    if (desc.Contains("enName"))
                    {
                        englishIndex = i;
                        break;
                    }
                }
            }
            
            // Если все еще не нашли, ищем любое название с латинскими буквами из массива names
            if (englishIndex < 0 && namesArray != null)
            {
                for (int i = 0; i < titles.Count; i++)
                {
                    var (title, desc) = titles[i];
                    if (desc.Contains("Из массива names") && 
                        title.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c)) &&
                        !title.Any(c => c >= 0x3040 && c <= 0x309F) && // Хирагана
                        !title.Any(c => c >= 0x30A0 && c <= 0x30FF))    // Катакана
                    {
                        englishIndex = i;
                        break;
                    }
                }
            }

            if (englishIndex >= 0)
            {
                lstTitles.SelectedIndex = englishIndex;
            }
            else
            {
                lstTitles.SelectedIndex = 0;
            }
        }
    }

    private void SelectTitle()
    {
        if (lstTitles.SelectedIndex >= 0)
        {
            string selected = lstTitles.SelectedItem?.ToString() ?? "";
            // Извлекаем название до символа |
            int pipeIndex = selected.IndexOf(" | ");
            if (pipeIndex > 0)
            {
                SelectedTitle = selected.Substring(0, pipeIndex);
            }
            else
            {
                SelectedTitle = selected;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

