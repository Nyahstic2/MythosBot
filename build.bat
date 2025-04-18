@echo off
REM *******************************************************
REM * Script para puxar a branch Nyahstic2/MythosBot,    *
REM * verificar e instalar .NET SDK 8.0 se necessário,      *
REM * entrar na pasta MythosBot\MythosBot e executar os     *
REM * comandos dotnet restore, build e run.               *
REM *******************************************************

REM -------------------------------------------
REM Clonar ou atualizar o repositório e branch
REM -------------------------------------------
if not exist "MythosBot" (
    echo Clonando o repositório...
    git clone https://github.com/Nyahstic2/MythosBot.git
    if errorlevel 1 (
        echo Erro ao clonar o repositório.
        pause
        exit /b 1
    )
) else (
    echo Repositório existente. Atualizando a branch Nyahstic2/MythosBot...
    pushd MythosBot
    git fetch
    git checkout Nyahstic2/MythosBot
    git pull origin Nyahstic2/MythosBot
    popd
)

REM -------------------------------------------
REM Verificar se o .NET SDK 8.0 está instalado
REM -------------------------------------------
dotnet --list-sdks | find "8.0" >nul 2>&1
if errorlevel 1 (
    echo .NET SDK 8.0 nao encontrado! Baixando e instalando .NET SDK 8.0...
    set "DOTNET_URL=https://download.visualstudio.microsoft.com/download/pr/67ed026513b4475d9c73a8a94d605ddd/dotnet-sdk-8.0.100-win-x64.exe"
    set "DOTNET_INSTALLER=%TEMP%\dotnet-sdk-8.0.exe"
    echo Baixando .NET SDK 8.0...
    curl -L -o "%DOTNET_INSTALLER%" "%DOTNET_URL%"
    if errorlevel 1 (
        echo Falha ao baixar o .NET SDK!
        pause
        exit /b 1
    )
    echo Instalando .NET SDK 8.0...
    start /wait "" "%DOTNET_INSTALLER%" /quiet /norestart
    if errorlevel 1 (
        echo Falha na instalacao do .NET SDK!
        pause
        exit /b 1
    )
    echo .NET SDK 8.0 instalado com sucesso.
) else (
    echo .NET SDK 8.0 ja esta instalado.
)

REM -------------------------------------------
REM Entrar na pasta do projeto e executar os comandos dotnet
REM -------------------------------------------
if not exist "\MythosBot" (
    echo Pasta "MythosBot\MythosBot" nao encontrada!
    pause
    exit /b 1
)

echo Entrando na pasta do projeto...
cd ./MythosBot

echo Restaurando pacotes...
dotnet restore
if errorlevel 1 (
    echo Erro no comando dotnet restore.
    pause
    exit /b 1
)

echo Compilando o projeto...
dotnet build --no-restore
if errorlevel 1 (
    echo Erro no comando dotnet build.
    pause
    exit /b 1
)

echo Executando o projeto...
dotnet run

echo Processo concluido.
pause
