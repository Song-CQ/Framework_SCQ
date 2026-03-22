@echo off
chcp 65001 > nul
setlocal enabledelayedexpansion

cd /d %~dp0

REM 检查是否有参数文件
if "%1"=="" (
    .\Release\ExcelTool.exe
    goto :end
)

set "PARAM_FILE=%~1"

if not exist "%PARAM_FILE%" (
    .\Release\ExcelTool.exe
    goto :end
)

REM 读取参数文件
for /f "tokens=1* delims=:" %%a in ('findstr /n "^" "%PARAM_FILE%"') do (
    if %%a==1 set "Version=%%b"
    if %%a==2 set "Type=%%b"
    if %%a==3 set "isOutMultipleDatas=%%b"
    if %%a==4 set "isEnciphermentData=%%b"
    if %%a==5 set "AES_IVector=%%b"
    if %%a==6 set "AES_Key=%%b"
)

REM 调用 ExcelTool 传递参数
.\Release\ExcelTool.exe %Version% %Type% %isOutMultipleDatas% %isEnciphermentData% "%AES_IVector%" "%AES_Key%"

if errorlevel 1 (
    echo Error: ExcelTool failed with exit code %errorlevel%
)

:end