#!/usr/bin/env bash
set -euo pipefail

# Usage:
# ```
# DIR="path/to"
# SRC="${DIR}/file.${{ matrix.exportMode }}.ext"
#
# chmod +x .github/scripts/helpers.sh && source .github/scripts/helpers.sh
#
# check_folder "$DIR"
# check_file "$SRC"
# cat_file "$SRC"
# ```

start_separator() {
  echo; printf '%.0s#' {1..140}; echo
}

final_separator() {
  echo; printf '%.0s^' {1..140}; echo
}

compress() {
  local path_to_compress="$1"
  local        arch_name="$2"
  local        arch_type="$3"
  local             rate="$4"
  #
  start_separator
  #
  missing_args=""
  [ -z "$path_to_compress" ] && missing_args="$missing_args path_to_compress"
  [ -z "$arch_name" ]        && missing_args="$missing_args arch_name"
  [ -z "$arch_type" ]        && missing_args="$missing_args arch_type"
  [ -z "$rate" ]             && missing_args="$missing_args rate"
  if [ ! -z "$missing_args" ]; then
    echo "❌ Missing args: $missing_args"
    final_separator
    return
  fi
  #
  if [[ "$arch_type" != "7z" && "$arch_type" != "zstd" ]]; then
    echo "❌ Incorrect arch_type: $arch_type"
    final_separator
    return
  fi
  #
  if [ -f "$path_to_compress" ]; then
    echo "🗜️  Compression:"
    echo "      - file:      '$path_to_compress'"
    check_file   $path_to_compress
  elif [ -d "$path_to_compress" ]; then
    echo "🗜️  Compression:"
    echo "      - folder:    '$path_to_compress'"
    check_folder $path_to_compress
  else
    echo "❌  🗜️  No '$path_to_compress' to compress -> no compression."
    final_separator
    return
  fi
  echo "      - arch:      '$arch_name'"
  echo "      - arch type: '$arch_type'"
  echo "      - rate:      '$rate'"
  if [ -f "$path_to_compress" ]; then
    check_file   $path_to_compress
  else
    check_folder $path_to_compress
  fi
  #
  if [ -f "$arch_name" ]; then
    message="🗜️  Test archive '$arch_name' already exists - removing it before compression of '$path_to_compress'..."
    #echo "::warning:: $message"
    echo    "Warning: $message" >&2
    check_file   $arch_name
    rm           $arch_name
    message="🗜️  Removed '$arch_name'."
    #echo "::warning:: $message"
    echo    "Warning: $message" >&2
  fi
  #
  echo "🗜️  Compressing..."
  if   [ "$arch_type" == "7z" ]; then
    time 7z a -ms=on -mx="$rate" -mmt=on $arch_name $path_to_compress
  else
    time tar -I "zstd -$rate" -cf        $arch_name $path_to_compress
  fi
  echo "🗜️  Compression finished. Checking it..."
  check_file     $arch_name
  if [ -f "$arch_name" ]; then
    if   [ "$arch_type" == "7z" ]; then
      echo  "🗜️  Testing archive '$arch_name'..."
      time 7z t  $arch_name
    fi
  else
    message="🗜️  Test archive '$arch_name' doesn't exist after compression of '$path_to_compress' - some error."
    #echo "::warning:: $message"
    echo    "Warning: $message" >&2
  fi
  echo "🗜️  Compression completed."
  final_separator
}

compress_7z() {
  local path_to_compress="$1"
  local        arch_name="$2"
  local             rate="$3"
  #
  compress $path_to_compress $arch_name "7z" $rate
}

compress_zstd() {
  local path_to_compress="$1"
  local        arch_name="$2"
  local             rate="$3"
  #
  compress $path_to_compress $arch_name "zstd" $rate
}

check_file() {
  local file="$1"
  start_separator
  if [ -f "$file" ]; then
    local full_path=$(readlink -f "$file" 2>/dev/null || echo "$file")
    local size=$(du -sh "$file" 2>/dev/null | cut -f1 || echo "N/A")
    echo "🗃️ check '$file' ($full_path) (Size: $size)."
  else
    echo "❌ 🗃️ No '$file'."
  fi
  final_separator
}

check_folder() {
  local folder="$1"
  start_separator
  if [ -d "$folder" ]; then
    local full_path=$(readlink -f "$folder" 2>/dev/null || echo "$folder")
    local size=$(du -sh "$folder" 2>/dev/null | cut -f1 || echo "N/A")
    echo "📁 '$folder' ($full_path) (Size: $size):"
    ls -alF --group-directories-first "$folder"
  else
    echo "❌ 📁 No '$folder'."
  fi
  final_separator
}

cat_file() {
  local file="$1"
  start_separator
  if [ -f "$file" ]; then
    local full_path=$(readlink -f "$file" 2>/dev/null || echo "$file")
    local size=$(du -sh "$file" 2>/dev/null | cut -f1 || echo "N/A")
    echo "📁 cat '$file' ($full_path) (Size: $size):"
    if command -v bat &> /dev/null; then
      bat --theme=ansi "$file"
    else
      # Фоллбэк на обычный cat
      cat "$file"
    fi
  else
    echo "❌ 📁 No '$file'."
  fi
  final_separator
}
