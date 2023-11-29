@echo off
set unityPath=%1%
set projectPath=%2%
set appName=%3%

ren %projectPath% "%appName%_UClient"
call StartUnity_启动项目.bat unityPath projectPath
 
exit