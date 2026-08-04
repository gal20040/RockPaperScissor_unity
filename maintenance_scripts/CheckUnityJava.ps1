# CheckUnityJava.ps1 — проверка версий Java/Javac во всех установленных версиях Unity
# Запускать в PowerShell 7 (или 5.1) из любой папки.

$unityBasePath = "C:\Program Files\Unity\Hub\Editor"

if (-not (Test-Path $unityBasePath)) {
    Write-Host "❌ Папка '$unityBasePath' не найдена." -ForegroundColor Red
    Write-Host "Проверьте, где установлен Unity Hub, и поправьте путь в скрипте." -ForegroundColor Yellow
    exit 1
}

# Получаем только папки, имя которых похоже на версию Unity (начинается с цифры)
# Это отсечёт .git, .vscode, Assets и прочие папки проекта, если случайно запустить не там.
$versions = Get-ChildItem -Path $unityBasePath -Directory |
    Where-Object { $_.Name -match '^\d' } |
    Sort-Object Name

if ($versions.Count -eq 0) {
    Write-Host "⚠️ Версии Unity не найдены в $unityBasePath" -ForegroundColor Yellow
    Write-Host "Убедитесь, что путь верный и Unity действительно установлен через Hub." -ForegroundColor Gray
    exit 0
}

Write-Host "✅ Найдено версий Unity: $($versions.Count). Начинаю проверку..." -ForegroundColor Cyan
Write-Host ("{0,-12} {1,-10} {2,-10} {3}" -f "Версия", "Java", "Javac", "Статус")
Write-Host ("-" * 60)

foreach ($versionDir in $versions) {
    $versionName = $versionDir.Name
    $javaBinPath = Join-Path $versionDir.FullName "Editor\Data\PlaybackEngines\AndroidPlayer\OpenJDK\bin"
    
    $javaExe = Join-Path $javaBinPath "java.exe"
    $javacExe = Join-Path $javaBinPath "javac.exe"

    if (-not (Test-Path $javaExe) -or -not (Test-Path $javacExe)) {
        Write-Host ("{0,-12} {1,-10} {2,-10} {3}" -f $versionName, "-", "-", "❌ Нет JDK") -ForegroundColor Yellow
        continue
    }

    # Запускаем команды и захватываем вывод
    $javaVersionOutput = & $javaExe --version 2>&1
    $javacVersionOutput = & $javacExe --version 2>&1

    # Парсим мажорную версию Javac (например, из "javac 17.0.18" берём 17)
    $javacMajorVersion = 0
    if ($javacVersionOutput -match '^javac\s+(\d+)') {
        $javacMajorVersion = [int]$matches[1]
    }

    # Формируем строку вывода
    $currentJavacMajorVersion = 17
    $statusIcon = if ($javacMajorVersion -eq $currentJavacMajorVersion) { "✅ OK" } else { "⚠️ WARN" }
    $statusColor = if ($javacMajorVersion -eq $currentJavacMajorVersion) { "Green" } else { "Red" }

    $javaVerShort = ($javaVersionOutput | Select-Object -First 1) -replace '^openjdk\s+', '' -replace '\s.*$', ''
    $javacVerShort = ($javacVersionOutput | Select-Object -First 1) -replace '^javac\s+', ''

    Write-Host ("{0,-12} {1,-10} {2,-10} {3}" -f $versionName, $javaVerShort, $javacVerShort, $statusIcon) -ForegroundColor $statusColor
}

if (-not ($statusColor -eq "Green"))
{
    Write-Host ("-" * 60)
    Write-Host ("[$statusIcon] Скорее всего надо обновить версию 'java' и 'jvm...' в следующих файлах:") -ForegroundColor $statusColor
    Write-Host ("[$statusIcon]   - android-secure-storage-plugin\build.gradle.kts") -ForegroundColor $statusColor
    Write-Host ("[$statusIcon]   - android-secure-storage-plugin\readme.md") -ForegroundColor $statusColor
}
Write-Host ("-" * 60)
Write-Host "Проверка завершена." -ForegroundColor Cyan
