@echo off
chcp 1251 >nul
title Admission System - Run
color 0A

echo.
echo ================================================
echo      Admission System Starting...
echo ================================================
echo.

cd /d "%~dp0"

if exist "AdmissionSystem\bin\Release\net6.0-windows\AdmissionSystem.exe" (
    echo Found compiled file (Release)
    start "" "AdmissionSystem\bin\Release\net6.0-windows\AdmissionSystem.exe"
    echo.
    echo Program started!
    timeout /t 2 >nul
) else if exist "AdmissionSystem\bin\Debug\net6.0-windows\AdmissionSystem.exe" (
    echo Found compiled file (Debug)
    start "" "AdmissionSystem\bin\Debug\net6.0-windows\AdmissionSystem.exe"
    echo.
    echo Program started!
    timeout /t 2 >nul
) else (
    echo ERROR: Executable file not found!
    echo.
    echo Please build the project first:
    echo 1. Run "Build.bat"
    echo 2. Or open project in Visual Studio and Build
    echo.
    pause
)
