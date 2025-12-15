@echo off
chcp 1251 >nul
title Admission System - Rebuild
color 0E

echo.
echo ================================================
echo     Rebuilding Admission System Project
echo ================================================
echo.

cd /d "%~dp0"

echo Step 1: Stopping running instances...
echo.
taskkill /F /IM AdmissionSystem.exe >nul 2>&1
if %errorlevel% equ 0 (
    echo Application stopped successfully
) else (
    echo No running instances found
)
echo.

echo Step 2: Cleaning old build files...
echo.
if exist "AdmissionSystem\bin" (
    rd /s /q "AdmissionSystem\bin"
    echo Bin folder cleaned
)
if exist "AdmissionSystem\obj" (
    rd /s /q "AdmissionSystem\obj"
    echo Obj folder cleaned
)
echo.

echo Step 3: Building project...
echo.
call Build.bat

echo.
echo ================================================
echo            Rebuild Complete!
echo ================================================
echo.
echo You can now run the program using "Zapusk.bat"
echo.
pause
