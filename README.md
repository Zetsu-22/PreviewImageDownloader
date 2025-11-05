# Preview Image Downloader

Windows Forms приложение для автоматического скачивания превью изображений из Kitsu API в папку cache Obsidian.

## Возможности

- Поиск превью по API URL или названию аниме
- Автоматическое извлечение URL превью из JSON ответа Kitsu API
- Скачивание изображения в папку `cache` Obsidian
- Автоматическое именование файла: `{название_аниме}_preview.jpg`
- Предпросмотр скачанного изображения

## Использование

### Способ 1: По API URL

1. Выберите "API URL"
2. Вставьте полный URL запроса к Kitsu API:
   ```
   https://kitsu.io/api/edge/anime?filter[text]=noragami
   ```
3. Нажмите "Скачать превью"

### Способ 2: По названию аниме

1. Выберите "Название аниме"
2. Введите название аниме (на английском или транслитом)
3. Нажмите "Скачать превью"

Приложение автоматически:
- Сформирует API URL
- Получит JSON ответ
- Извлечет URL превью: `data[0].attributes.posterImage.original`
- Извлечет название аниме для имени файла
- Скачает изображение в `C:\Users\Doffy\Documents\локальное хранилище Obsidian\Obsidian\cache\{название}_preview.jpg`

## Структура JSON ответа

Приложение ожидает JSON ответ от Kitsu API в формате:
```json
{
  "data": [{
    "attributes": {
      "posterImage": {
        "original": "https://media.kitsu.app/anime/poster_images/7881/original.jpg"
      },
      "canonicalTitle": "Noragami"
    }
  }]
}
```

## Примеры

### Пример 1: Noragami
- API URL: `https://kitsu.io/api/edge/anime?filter[text]=noragami`
- Результат: `noragami_preview.jpg` в папке cache

### Пример 2: One Piece
- Название: `one piece`
- Результат: `one_piece_preview.jpg` в папке cache

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



