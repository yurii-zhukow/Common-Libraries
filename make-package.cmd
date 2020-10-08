pushd %~dp0\%1

dotnet pack --configuration Release
call xcopy ".\bin\Release\*.nupkg" "..\~Release\" /I /F /C /H /R /Y 
popd