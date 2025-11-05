# Инструкция по созданию репозитория на GitHub

## Шаг 1: Создание репозитория на GitHub

1. Откройте https://github.com/new
2. Заполните форму:
   - **Repository name**: `PreviewImageDownloader` (или другое название)
   - **Description**: `Windows Forms приложение для скачивания превью аниме из Kitsu API`
   - **Visibility**: выберите Public или Private
   - ⚠️ **НЕ** ставьте галочки на:
     - ❌ "Add a README file" (у нас уже есть)
     - ❌ "Add .gitignore" (у нас уже есть)
     - ❌ "Choose a license" (можно добавить позже)
3. Нажмите "Create repository"

## Шаг 2: Подключение локального репозитория к GitHub

После создания репозитория GitHub покажет вам команды. Выполните их в PowerShell:

```powershell
cd "C:\Users\Doffy\source\repos\preview image\PreviewImageDownloader"

# Замените YOUR_USERNAME на ваш GitHub username
git remote add origin https://github.com/YOUR_USERNAME/PreviewImageDownloader.git

# Переименуйте ветку в main (если нужно)
git branch -M main

# Отправьте код на GitHub
git push -u origin main
```

## Альтернативный вариант: через SSH

Если у вас настроен SSH ключ:

```powershell
git remote add origin git@github.com:YOUR_USERNAME/PreviewImageDownloader.git
git branch -M main
git push -u origin main
```

## Проверка

После выполнения команд проверьте:
- Откройте ваш репозиторий на GitHub
- Убедитесь, что все файлы загружены
- Проверьте, что README.md отображается правильно

## Полезные команды для работы с репозиторием

```powershell
# Проверить статус
git status

# Добавить изменения
git add .

# Сделать коммит
git commit -m "Описание изменений"

# Отправить на GitHub
git push

# Получить изменения с GitHub
git pull

# Проверить удаленные репозитории
git remote -v
```


