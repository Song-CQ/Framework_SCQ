@echo on
set unityPath=%1%
set projectPath=%2%
set appName=%3%
set newprojectPath=%4%
set batPath=%~dp0
timeout /t 3 /nobreak

ren %projectPath% "%appName%_UClient"
cd /d %batPath%
call StartUnity_Æô¶¯ÏîÄ¿.bat %unityPath% %newprojectPath%

pause