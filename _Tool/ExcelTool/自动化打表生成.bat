@echo off

echo ���ñ����� 
echo\

cd %~dp0
@call .\Release\ExcelTool.exe  %1 %2 %3

pause