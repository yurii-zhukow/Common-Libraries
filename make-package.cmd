pushd %~dp0
call Powershell.exe -File version-bump.ps1 %1
popd
pushd %~dp0\%1



call del /F ".\bin\Release\*.nupkg" 
dotnet pack --configuration Release
call xcopy ".\bin\Release\*.symbols.nupkg" "..\~Release\" /I /F /C /H /R /Y 
popd