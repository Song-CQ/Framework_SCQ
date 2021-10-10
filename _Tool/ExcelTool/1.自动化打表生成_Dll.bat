@echo off

echo 配置表生成Dll类 
echo\

cd %~dp0
@call .\Release\ExcelTool.exe Dll False %1

pause