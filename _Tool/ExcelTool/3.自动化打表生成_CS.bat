@echo off

echo 配置表生成Cs类 
echo\

cd %~dp0
@call .\Release\ExcelTool.exe CS False %1

pause