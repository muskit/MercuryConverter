@echo off

:: Set paths here first before running!!!
set video_path="\app\WindowsNoEditor\Mercury\Content\Movie"
set export_path=".\data\movies"

for %%i in (%video_path%\*.usm) do ffmpeg -n -f mpegvideo -i "%%i" "%export_path%\%%~ni.mp4"