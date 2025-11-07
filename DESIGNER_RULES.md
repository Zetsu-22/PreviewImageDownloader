# Правила для InitializeComponent в Windows Forms

## ✅ ЧТО МОЖНО делать в InitializeComponent:

1. **Создание UI элементов**
   ```csharp
   txtSearchTitle = new TextBox();
   btnSearchTitle = new Button();
   ```

2. **Базовая настройка свойств элементов**
   ```csharp
   txtSearchTitle.Location = new Point(15, 50);
   txtSearchTitle.Size = new Size(700, 25);
   btnSearchTitle.Text = "Найти";
   ```

3. **Добавление элементов в контейнеры**
   ```csharp
   this.Controls.Add(txtSearchTitle);
   panel.Controls.Add(button);
   ```

4. **Подписка на события (но без установки значений, вызывающих события)**
   ```csharp
   btnSearchTitle.Click += BtnSearchTitle_Click;
   ```

## ❌ ЧТО НЕЛЬЗЯ делать в InitializeComponent:

1. **Вызовы методов, обращающихся к файлам**
   ```csharp
   // ❌ НЕПРАВИЛЬНО
   File.ReadAllText("settings.json");
   Directory.Exists(path);
   ```

2. **Вызовы методов, обращающихся к API или сети**
   ```csharp
   // ❌ НЕПРАВИЛЬНО
   await HttpClient.GetAsync(url);
   ```

3. **Установка значений, которые вызывают события**
   ```csharp
   // ❌ НЕПРАВИЛЬНО (вызовет SelectedIndexChanged)
   cmbContentType.SelectedIndex = 0;
   
   // ❌ НЕПРАВИЛЬНО (может вызвать TextChanged)
   txtSearchTitle.Text = "Значение";
   ```

4. **Вызовы методов, которые читают настройки**
   ```csharp
   // ❌ НЕПРАВИЛЬНО
   GetKinopoiskApiKey();
   LoadSettings();
   ```

5. **Инициализация коллекций, которая может вызвать события**
   ```csharp
   // ❌ НЕПРАВИЛЬНО (может вызвать события)
   cmbContentType.Items.AddRange(new[] { "Аниме", "Фильм" });
   ```

6. **Любые вызовы, которые могут выбросить исключения**
   ```csharp
   // ❌ НЕПРАВИЛЬНО
   JObject.Parse(json);
   ```

## ✅ ПРАВИЛЬНАЯ СТРУКТУРА:

### InitializeComponent (только UI):
```csharp
private void InitializeComponent()
{
    // 1. Создание элементов
    txtSearchTitle = new TextBox();
    btnSearchTitle = new Button();
    
    // 2. Настройка свойств
    txtSearchTitle.Location = new Point(15, 50);
    txtSearchTitle.Size = new Size(700, 25);
    
    // 3. Подписка на события
    btnSearchTitle.Click += BtnSearchTitle_Click;
    
    // 4. Добавление в контейнеры
    this.Controls.Add(txtSearchTitle);
    this.Controls.Add(btnSearchTitle);
}
```

### Конструктор (инициализация данных):
```csharp
public Form1()
{
    InitializeComponent(); // Сначала создаем UI
    
    // Затем инициализируем данные
    cachePath = Path.Combine(...);
    LoadSettings();
    this.Load += Form1_Load;
}
```

### Form_Load (динамическая инициализация):
```csharp
private void Form1_Load(object? sender, EventArgs e)
{
    // Здесь можно безопасно:
    // - Заполнять ComboBox.Items
    // - Устанавливать SelectedIndex
    // - Вызывать методы, которые читают настройки
    cmbContentType.Items.AddRange(new[] { "Аниме", "Фильм" });
    cmbContentType.SelectedIndex = 0;
    UpdateCoverApiOptions();
}
```

## 🔧 РЕШЕНИЕ ПРОБЛЕМ:

Если дизайнер все равно выдает ошибки:

1. **Отключить дизайнер** (если UI создается программно):
   ```csharp
   [DesignerCategory("")]
   public partial class Form1 : Form
   ```

2. **Использовать проверку DesignMode**:
   ```csharp
   if (!DesignMode)
   {
       // Код, который не должен выполняться в дизайнере
   }
   ```

3. **Переместить проблемный код** в конструктор или Form_Load

