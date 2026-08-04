Get-ChildItem -Recurse -Include "*" -File | Where-Object { $_.DirectoryName -notmatch "bin|obj|node_modules|StrykerOutput|android|.godot" } | ForEach-Object {
    $filePath = $_.FullName


    try {
        # Читаем первые 3 байта файла
        $bytes = [System.IO.File]::ReadAllBytes($filePath) | Select-Object -First 3

        # Форматируем байты в шестнадцатеричном виде
        $hex = ($bytes | ForEach-Object { '{0:X2}' -f $_ }) -join ' '


        # Проверяем, что массив не пустой и содержит хотя бы 3 элемента
        if ($bytes -eq $null) {
            Write-Host "ОШИБКА: Файл пуст или не удалось прочитать байты: $filePath" -ForegroundColor Yellow
            return
        }

        if ($bytes.Count -lt 3) {
            Write-Host "ВНИМАНИЕ: Файл слишком мал для проверки BOM (<3 байт): $filePath" -ForegroundColor Cyan
            #Write-Host "  Первые байты: $hex" -ForegroundColor Gray
            return
        }

        # Проверка на BOM (EF BB BF)
        if ($bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
            Write-Host "BOM НАЙДЕН: $filePath" -ForegroundColor Red
            Write-Host "  Сигнатура BOM: $hex" -ForegroundColor DarkRed
        } else {
            Write-Host "БЕЗ BOM: $filePath" -ForegroundColor Green
            #Write-Host "  Первые байты: $hex" -ForegroundColor Gray
        }
    } catch {
        # Полный вывод ошибки с указанием файла
        Write-Host "ИСКЛЮЧЕНИЕ ПРИ ЧТЕНИИ ФАЙЛА:" -ForegroundColor Magenta
        Write-Host "  Путь: $filePath" -ForegroundColor White
        Write-Host "  Тип ошибки: $($_.Exception.GetType().Name)" -ForegroundColor White
        Write-Host "  Сообщение: $($_.Exception.Message)" -ForegroundColor White

        # Дополнительно можно вывести стектрейс (опционально)
        # Write-Host "  Стек вызовов: $($_.ScriptStackTrace)" -ForegroundColor Gray
    }
}
