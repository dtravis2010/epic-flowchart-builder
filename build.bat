@echo off
echo ========================================
echo Epic Flowchart Builder - Build Script
echo ========================================
echo.

echo Restoring NuGet packages...
dotnet restore
if errorlevel 1 goto error

echo.
echo Building project...
dotnet build --configuration Release
if errorlevel 1 goto error

echo.
echo ========================================
echo Build completed successfully!
echo ========================================
echo.
echo To run the application:
echo    cd bin\Release\net8.0-windows
echo    EpicFlowchartBuilder.exe
echo.
pause
exit /b 0

:error
echo.
echo ========================================
echo Build failed! Check errors above.
echo ========================================
pause
exit /b 1
