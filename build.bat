@echo off
REM *******************************************************
REM * Script para verificar/instalar Git e .NET SDK 8.0,  *
REM * clonar ou atualizar a branch Nyahstic2/MythosBot e    *
REM * executar os comandos dotnet restore, build e run.    *
REM *******************************************************

REM -------------------------------------------
REM Verificar se o Git está instalado
REM -------------------------------------------
where git >nul 2>nul
if errorlevel 1 (
    echo Git não encontrado! Baixando e instalando o Git...
    REM URL do instalador do Git. Atualize conforme necessário.
    set "GIT_URL=https://github.com/git-for-windows/git/releases/download/v2.39.1.windows.1/Git-2.39.1-64-bit.exe"
    set "GIT_INSTALLER=%TEMP%\GitInstaller.exe"
    echo Baixando Git...
    curl -L -o "%GIT_INSTALLER%" "%GIT_URL%"
    if errorlevel 1 (
        echo Falha ao baixar o Git!
        pause
        exit /b 1
    )
    echo Instalando o Git...
    start /wait "" "%GIT_INSTALLER%" /SILENT
    if errorlevel 1 (
        echo Falha na instalacao do Git!
        pause
        exit /b 1
    )
    echo Git instalado com sucesso.
) else (
    echo Git ja esta instalado.
)

REM -------------------------------------------
REM Clonar ou atualizar o repositório e branch
REM -------------------------------------------
REM Note: Atualize o URL a seguir para o do repositório correto.
if not exist "MythosBot" (
    echo Clonando o repositório...
    git clone https://github.com/Nyahstic2/MythosBot MythosBot
    if errorlevel 1 (
        echo Erro ao clonar o repositorio.
        pause
        exit /b 1
    )
) else (
    echo Repositorio existente. Atualizando a branch Nyahstic2/MythosBot...
    pushd MythosBot
    git fetch
    git checkout Nyahstic2/MythosBot
    git pull origin Nyahstic2/MythosBot
    popd
)

REM -------------------------------------------
REM Verificar se o .NET SDK 8.0 esta instalado
REM -------------------------------------------
dotnet --list-sdks | find "8.0" >nul 2>&1
if errorlevel 1 (
    echo .NET SDK 8.0 nao encontrado! Baixando e instalando .NET SDK 8.0...
    REM URL do instalador do .NET SDK 8.0. Atualize o link conforme necessario.
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
if not exist "MythosBot\MythosBot" (
    echo Pasta "MythosBot\MythosBot" nao encontrada!
    pause
    exit /b 1
)

pushd MythosBot\MythosBot

echo Restaurando pacotes...
dotnet restore
if errorlevel 1 (
    echo Erro no comando dotnet restore.
    popd
    pause
    exit /b 1
)

echo Compilando o projeto...
dotnet build --no-restore
if errorlevel 1 (
    echo Erro no comando dotnet build.
    popd
    pause
    exit /b 1
)

echo Executando o projeto...
dotnet run

popd
echo Processo concluido.
pause
