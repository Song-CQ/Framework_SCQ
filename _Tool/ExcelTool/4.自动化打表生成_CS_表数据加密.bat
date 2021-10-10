@echo off

echo 配置表生成Cs类 
echo Excel数据加密
echo\

cd %~dp0
@call .\Release\ExcelTool.exe CS True %1

pause