@echo off
chcp 1251 >nul
title Admission System - Build
color 0B

echo.
echo ================================================
echo        Building Admission System Project
echo ================================================
echo.

cd /d "%~dp0"

echo Checking for .NET SDK...
where dotnet >nul 2>&1
if %errorlevel% neq 0 (
    echo.
    echo ERROR: .NET SDK not found!
    echo.
    echo Please install .NET 6.0 SDK or higher from:
    echo https://dotnet.microsoft.com/download
    echo.
    echo After installation, run this file again.
    echo.
    pause
    exit /b 1
)

echo .NET SDK found
echo.

echo Restoring NuGet packages...
dotnet restore AdmissionSystem\AdmissionSystem.csproj
if %errorlevel% neq 0 (
    echo.
    echo ERROR: Failed to restore packages!
    pause
    exit /b 1
)

echo.
echo Building project in Release mode...
dotnet build AdmissionSystem\AdmissionSystem.csproj -c Release
if %errorlevel% neq 0 (
    echo.
    echo ERROR: Build failed!
    pause
    exit /b 1
)

echo.
echo ================================================
echo        Build completed successfully!
echo ================================================
echo.
echo You can now run the program using "Run.bat"
echo.
pause
