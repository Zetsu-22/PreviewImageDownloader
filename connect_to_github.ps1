# Скрипт для подключения локального репозитория к GitHub
# Перед запуском создайте репозиторий на https://github.com/new

param(
    [Parameter(Mandatory=$true)]
    [string]$GitHubUsername,
    
    [Parameter(Mandatory=$false)]
    [string]$RepositoryName = "PreviewImageDownloader"
)

Write-Host "Подключение к GitHub репозиторию..." -ForegroundColor Green
Write-Host ""

# Проверяем, что мы в правильной директории
if (-not (Test-Path ".git")) {
    Write-Host "Ошибка: не найден git репозиторий. Запустите скрипт из папки проекта." -ForegroundColor Red
    exit 1
}

# Проверяем, есть ли уже remote
$existingRemote = git remote -v 2>$null
if ($existingRemote) {
    Write-Host "Внимание: уже есть настроенный remote:" -ForegroundColor Yellow
    Write-Host $existingRemote
    $response = Read-Host "Перезаписать? (y/n)"
    if ($response -ne "y") {
        Write-Host "Отменено." -ForegroundColor Yellow
        exit 0
    }
    git remote remove origin
}

# Добавляем remote
$remoteUrl = "https://github.com/$GitHubUsername/$RepositoryName.git"
Write-Host "Добавление remote: $remoteUrl" -ForegroundColor Cyan
git remote add origin $remoteUrl

# Переименовываем ветку в main (если нужно)
$currentBranch = git branch --show-current
if ($currentBranch -eq "master") {
    Write-Host "Переименование ветки master в main..." -ForegroundColor Cyan
    git branch -M main
}

# Проверяем статус
Write-Host ""
Write-Host "Текущий статус:" -ForegroundColor Green
git status

Write-Host ""
Write-Host "Следующие шаги:" -ForegroundColor Yellow
Write-Host "1. Убедитесь, что репозиторий создан на GitHub: https://github.com/new" -ForegroundColor White
Write-Host "2. Выполните команду:" -ForegroundColor White
Write-Host "   git push -u origin main" -ForegroundColor Cyan
Write-Host ""

$push = Read-Host "Отправить код на GitHub сейчас? (y/n)"
if ($push -eq "y") {
    Write-Host "Отправка кода на GitHub..." -ForegroundColor Cyan
    git push -u origin main
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "Успешно! Репозиторий доступен по адресу:" -ForegroundColor Green
        Write-Host "https://github.com/$GitHubUsername/$RepositoryName" -ForegroundColor Cyan
    } else {
        Write-Host ""
        Write-Host "Ошибка при отправке. Проверьте:" -ForegroundColor Red
        Write-Host "- Репозиторий создан на GitHub" -ForegroundColor White
        Write-Host "- У вас есть права на запись" -ForegroundColor White
        Write-Host "- Правильно указан username" -ForegroundColor White
    }
}


