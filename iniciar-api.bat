@echo off
title Ecos Colombia API - Host de Swagger
echo ===================================================
echo   Iniciando API Ecos Colombia con Swagger Host...
echo ===================================================
echo.
echo 1. Abriendo Swagger en el navegador: http://localhost:5154/swagger
start http://localhost:5154/swagger
echo 2. Iniciando servidor Kestrel...
cd /d "%~dp0Backend\Api"
dotnet run --launch-profile ApiPrueba
pause
