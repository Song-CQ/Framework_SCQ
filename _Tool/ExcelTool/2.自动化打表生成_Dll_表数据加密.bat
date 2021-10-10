@echo off

echo 配置表生成Dll类 
echo Excel数据加密
echo\

cd %~dp0
@call .\Release\ExcelTool.exe Dll True %1

pause