@echo off
echo ========================================
echo Epic Flowchart Builder - Verification
echo ========================================
echo.

echo Checking file structure...
echo.

set ERROR=0

if exist "EpicFlowchartBuilder.csproj" (echo [OK] Project file found) else (echo [MISSING] EpicFlowchartBuilder.csproj & set ERROR=1)
if exist "Models\QuestionnaireModels.cs" (echo [OK] Models found) else (echo [MISSING] Models & set ERROR=1)
if exist "ViewModels\MainViewModel.cs" (echo [OK] ViewModels found) else (echo [MISSING] ViewModels & set ERROR=1)
if exist "Views\MainWindow.xaml" (echo [OK] Views found) else (echo [MISSING] Views & set ERROR=1)
if exist "Services\FlowchartService.cs" (echo [OK] Services found) else (echo [MISSING] Services & set ERROR=1)
if exist "Services\AiAssistantService.cs" (echo [OK] AI Service found) else (echo [MISSING] AI Service & set ERROR=1)
if exist "Services\DrawioXmlExporter.cs" (echo [OK] Exporter found) else (echo [MISSING] Exporter & set ERROR=1)
if exist "Examples\MRI-Screening-Example.json" (echo [OK] Examples found) else (echo [MISSING] Examples & set ERROR=1)
if exist "README.md" (echo [OK] Documentation found) else (echo [MISSING] Documentation & set ERROR=1)

echo.
if %ERROR%==0 (
    echo ========================================
    echo All files present! Ready to build.
    echo ========================================
    echo.
    echo Next steps:
    echo   1. Run build.bat to compile
    echo   2. Read START-HERE.md
    echo   3. Follow QUICKSTART.md
) else (
    echo ========================================
    echo WARNING: Some files are missing!
    echo ========================================
)
echo.
pause
