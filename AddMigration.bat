@echo off
set /p MigrationName="Enter migration name: "
if "%MigrationName%"=="" (
    echo Migration name cannot be empty.
    pause
    exit /b
)

:: Установить переменную окружения для скрипта
set NODETREES_DB_CONNECTION_STRING=Host=localhost;Port=5432;Database=NodeTreesDB;Username=postgres;Password=postgres;Include Error Detail=true

:: Выполнить команду миграции
powershell -Command "dotnet ef migrations add %MigrationName% --project src/NodeTrees.DataAccess/NodeTrees.DataAccess.csproj --startup-project src/NodeTrees.DataAccess/NodeTrees.DataAccess.csproj --output-dir Migrations"

pause
