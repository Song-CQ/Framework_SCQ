
@echo on

echo HotFix_Dll_CompileAssembly 
echo\

cd %~dp0
@call Release\HotFix_Tool.exe %1
