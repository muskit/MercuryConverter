#!/bin/sh

# Set paths here first before running!!!
video_path="App/WindowsNoEditor/Mercury/Content/Movie/"
output_path="./data/movies/"

for file in "$video_path"/*.usm; do
    output_file="$output_path/$(basename "$file" .usm).mp4"
    echo $file
    echo $output_file
    ffmpeg -n -f mpegvideo -i "$file" "$output_file"
done