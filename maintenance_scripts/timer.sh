#!/bin/bash

# Сделайте его исполняемым:
# chmod +x timer.sh
# Импортируйте функции в текущую сессию:
# source timer.sh
# или
# . timer.sh
# Используйте команды:
# s; npm run format; e

TIME_OFFSET_HOURS=4  # Смещение времени в часах (по умолчанию +4)
LINE_LENGTH=176      # Длина разделительных линий (по умолчанию 176 символов)

st="" # Переменная для хранения стартового времени (в секундах)

get_offset_time() { # Функция для получения текущего времени со смещением
    # Получаем текущее время в секундах
    local current_seconds=$(date +%s)
    # Добавляем смещение в секундах
    local offset_seconds=$((current_seconds + TIME_OFFSET_HOURS * 3600))
    # Конвертируем обратно в читаемый формат
    date -d "@$offset_seconds" "+%Y.%m.%d %H:%M:%S"
}

t() {
    now=$(get_offset_time)

    if [ -z "$st" ]; then
        # Сохраняем стартовое время со смещением в секундах
        st=$(( $(date +%s) + TIME_OFFSET_HOURS * 3600 ))
        echo -e "\033[32m$now\033[0m"
    else
        # Текущее время со смещением в секундах
        local now_seconds=$(date +%s)
        local now_offset_seconds=$((now_seconds + TIME_OFFSET_HOURS * 3600))

        # Вычисляем разницу
        local diff=$((now_offset_seconds - st))
        local mins=$((diff / 60))
        local secs=$((diff % 60))

        printf "\033[32m%s - затрачено %02d:%02d\033[0m\n" "$now" "$mins" "$secs"
    fi
}

s() {
    st=""
    # Создаём линию из # длиной LINE_LENGTH
    printf "\033[32m"
    for ((i=1; i<=LINE_LENGTH; i++)); do
        printf "#"
    done
    echo -e "\033[0m"
    t
}

e() {
    t
    # Создаём линию из ^ длиной LINE_LENGTH
    printf "\033[32m"
    for ((i=1; i<=LINE_LENGTH; i++)); do
        printf "^"
    done
    echo -e "\033[0m"
}
