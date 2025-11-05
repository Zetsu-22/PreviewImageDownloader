# Preview Image Downloader

Windows Forms приложение для автоматического скачивания превью изображений из различных API (Kitsu, OMDb, Open Library) в папку cache Obsidian.

## Возможности

- **Мульти-источники**: Поддержка аниме, фильмов, сериалов и книг
- **Аниме**: Поиск через Kitsu API
- **Фильмы и сериалы**: Поиск через OMDb API (требуется API ключ)
- **Книги**: Поиск через Open Library API
- Поиск по названию или прямому API URL
- Автоматическое извлечение URL превью из JSON ответов API
- Скачивание изображения в папку `cache` Obsidian
- Автоматическое именование файла: `{название}_preview.jpg`
- Предпросмотр скачанного изображения
- Сохранение настроек OMDb API ключа

## Использование

### Настройка OMDb API (только для фильмов и сериалов)

1. Получите бесплатный API ключ на http://www.omdbapi.com/apikey.aspx
2. В приложении нажмите кнопку "Настройки"
3. Введите ваш OMDb API ключ
4. Нажмите "Сохранить"

**Примечание**: Для аниме и книг API ключ не требуется.

### Поиск по названию

1. Выберите тип контента из выпадающего списка:
   - **Аниме** - поиск через Kitsu API
   - **Фильм** - поиск через OMDb API
   - **Сериал** - поиск через OMDb API
   - **Книга** - поиск через Open Library API
2. Выберите "Поиск по названию"
3. Введите название (рекомендуется на английском языке)
4. Нажмите "Скачать превью"

### Поиск по API URL

1. Выберите тип контента
2. Выберите "По API URL"
3. Вставьте полный URL запроса к соответствующему API
4. Нажмите "Скачать превью"

Приложение автоматически:
- Сформирует API URL (при поиске по названию)
- Получит JSON ответ
- Извлечет URL превью из ответа
- Извлечет название для имени файла
- Скачает изображение в `C:\Users\Doffy\Documents\локальное хранилище Obsidian\Obsidian\cache\{название}_preview.jpg`

## Поддерживаемые API

### Kitsu API (Аниме)
- **Endpoint**: `https://kitsu.io/api/edge/anime?filter[text]={название}`
- **Превью**: `data[0].attributes.posterImage.original`
- **Пример**: `noragami` → `noragami_preview.jpg`

### OMDb API (Фильмы и сериалы)
- **Endpoint**: `http://www.omdbapi.com/?apikey={ключ}&t={название}&type={movie|series}`
- **Превью**: `Poster`
- **Пример**: `Fight Club` (фильм) → `fight_club_preview.jpg`
- **Пример**: `Breaking Bad` (сериал) → `breaking_bad_preview.jpg`
- **Требуется**: Бесплатный API ключ (1000 запросов/день)

### Open Library API (Книги)
- **Endpoint**: `https://openlibrary.org/search.json?title={название}`
- **Превью**: `https://covers.openlibrary.org/b/id/{cover_i}-L.jpg`
- **Пример**: `The Witcher` → `the_witcher_preview.jpg`

## Примеры использования

### Аниме: Noragami
- Тип: **Аниме**
- Название: `noragami`
- Результат: `noragami_preview.jpg`

### Фильм: Fight Club
- Тип: **Фильм**
- Название: `Fight Club`
- Результат: `fight_club_preview.jpg`

### Сериал: Breaking Bad
- Тип: **Сериал**
- Название: `Breaking Bad`
- Результат: `breaking_bad_preview.jpg`

### Книга: The Witcher
- Тип: **Книга**
- Название: `The Witcher`
- Результат: `the_witcher_preview.jpg`

## Технические детали

- **Язык**: C# (.NET 9.0)
- **UI**: Windows Forms
- **JSON парсинг**: Newtonsoft.Json
- **HTTP клиент**: System.Net.Http.HttpClient

## Сборка

```bash
cd "C:\Users\Doffy\source\repos\preview image\PreviewImageDownloader"
dotnet build
dotnet run
```

## Запуск

После сборки исполняемый файл находится в:
```
bin\Debug\net9.0-windows\PreviewImageDownloader.exe
```

Можно создать ярлык на рабочем столе для быстрого доступа.




