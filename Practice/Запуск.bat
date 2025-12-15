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
    echo Trying to build the project automatically...
    echo.
    call Build.bat
    if %errorlevel% equ 0 (
        echo.
        echo Build successful! Starting program...
        echo.
        if exist "AdmissionSystem\bin\Release\net6.0-windows\AdmissionSystem.exe" (
            start "" "AdmissionSystem\bin\Release\net6.0-windows\AdmissionSystem.exe"
            echo Program started!
            timeout /t 2 >nul
        )
    ) else (
        echo.
        echo Build failed! Please check the errors above.
        echo.
        pause
    )
)
