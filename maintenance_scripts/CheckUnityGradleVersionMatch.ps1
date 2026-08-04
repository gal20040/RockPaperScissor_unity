param(
    [string]$ProjectRoot = ".",
    [string]$PluginPathSuffix = "android-secure-storage-plugin"
)

$projectVersionFile = Join-Path $ProjectRoot "ProjectSettings\ProjectVersion.txt"
$gradlePropsFile = Join-Path $ProjectRoot "$PluginPathSuffix\gradle.properties"

Write-Host "🔍 Проверка соответствия версий Unity..." -ForegroundColor Cyan

# 1. Читаем версию из ProjectVersion.txt
if (-not (Test-Path $projectVersionFile)) {
    Write-Host "❌ Файл не найден: $projectVersionFile" -ForegroundColor Red; exit 1
}
$content = Get-Content $projectVersionFile -Raw
if ($content -match 'm_EditorVersion:\s*([\w.]+)') {
    $unityVersionFromProject = $matches[1]
} else {
    Write-Host "❌ Не удалось распарсить m_EditorVersion в $projectVersionFile" -ForegroundColor Red; exit 1
}
Write-Host "✅ Версия из ProjectVersion.txt: $unityVersionFromProject" -ForegroundColor Green

# 2. Читаем путь из gradle.properties
if (-not (Test-Path $gradlePropsFile)) {
    Write-Host "❌ Файл не найден: $gradlePropsFile" -ForegroundColor Red; exit 1
}
$gradleContent = Get-Content $gradlePropsFile -Raw

# Ищем org.gradle.java.home (учитываем возможные кавычки и пробелы)
if ($gradleContent -match 'org\.gradle\.java\.home\s*=\s*(?:["''])?(.*?)(?:["''])?\s*(?=\r?\n|$)') {
    $javaHomePath = $matches[1].Trim()
} else {
    Write-Host "❌ Не найдено значение org.gradle.java.home в $gradlePropsFile" -ForegroundColor Red; exit 1
}
Write-Host "📂 Путь из gradle.properties: $javaHomePath" -ForegroundColor Gray

# 3. ИЗВЛЕЧЕНИЕ ВЕРСИИ ИЗ ПУТИ (ИСПРАВЛЕННАЯ ЛОГИКА)
# Путь имеет вид: ...\Hub\Editor\<VERSION>\Editor\Data\...
# Нам нужно вытащить <VERSION> между первым "Editor\" и следующим "\Editor"
# Нормализуем слэши для надежности (заменяем / на \)
$normalizedPath = $javaHomePath -replace '/', '\'

# Регулярка: ищем слово "Editor", затем обратный слэш, затем захватываем версию (цифры, точки, f/b/c и цифры), затем снова "\Editor"
if ($normalizedPath -match '(?i)Editor\\([\d.]+[fbc]?\d*)(\\Editor)') {
    $unityVersionFromGradle = $matches[1]
    Write-Host "✅ Версия из пути JDK: $unityVersionFromGradle" -ForegroundColor Green
} else {
    # Если основная регулярка не сработала, пробуем запасной вариант (на случай очень странных путей)
    Write-Host "❌ Не удалось извлечь версию Unity из пути к JDK." -ForegroundColor Red
    Write-Host "Путь: $normalizedPath" -ForegroundColor Gray
    
    # Показываем, где примерно мы искали
    Write-Host "💡 Ожидаемый формат: ...\Hub\Editor\<ВЕРСИЯ>\Editor\..." -ForegroundColor Yellow
    exit 1
}

# 4. Сравнение
Write-Host "----------------------------------------"
if ($unityVersionFromProject -eq $unityVersionFromGradle) {
    Write-Host "🎉 РЕЗУЛЬТАТ: ВЕРСИИ СОВПАДАЮТ!" -ForegroundColor Green
    exit 0
} else {
    Write-Host "💥 РЕЗУЛЬТАТ: НЕСООТВЕТСТВИЕ ВЕРСИЙ!" -ForegroundColor Red
    Write-Host "   В проекте:      $unityVersionFromProject" -ForegroundColor White
    Write-Host "   В пути JDK:     $unityVersionFromGradle" -ForegroundColor Red
    Write-Host ""
    Write-Host "⚠️ Это вызовет проблемы при сборке Android (несовместимость JDK и билда Unity)." -ForegroundColor Yellow
    Write-Host "💡 Решение: Исправьте 'org.gradle.java.home' в 'android-secure-storage-plugin\gradle.properties' под версию $unityVersionFromProject" -ForegroundColor Red
    exit 1
}
