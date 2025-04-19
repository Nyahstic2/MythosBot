#!/bin/bash 

# Salva a política de execução atual (disponível apenas para sistemas que usam um controle de política)
CURRENT_POLICY=$(umask)
echo "Politica de execucao atual: $CURRENT_POLICY"

# Altera temporariamente a política para permitir execução sem restrições
umask 000
echo "Politica de execucao alterada para Unrestricted."

# Mudando para a pasta MythosBot relativa ao local do script
PROJECT_PATH="$(dirname "$0")/MythosBot"

if [ ! -d "$PROJECT_PATH" ]; then
    echo "Pasta MythosBot nao encontrada. Verifique a localizacao do script."
    exit 1
fi

cd "$PROJECT_PATH"
echo "Entrou na pasta $PROJECT_PATH"

# Verifica se o .NET está instalado
if ! command -v dotnet &> /dev/null; then
    echo "Dotnet nao encontrado. Instalando o Dotnet 8 SDK..."
    
    if command -v apt &> /dev/null; then
        sudo apt update && sudo apt install -y dotnet-sdk-8.0
    elif command -v yum &> /dev/null; then
        sudo yum install -y dotnet-sdk-8.0
    else
        echo "Nao foi possivel instalar automaticamente. Instale manualmente via: https://dotnet.microsoft.com/download/dotnet/8.0"
        exit 1
    fi
else
    echo "Dotnet encontrado: $(dotnet --version)"
fi

# Restaura pacotes e compila o projeto
echo "Restaurando as dependencias..."
dotnet restore

echo "Compilando o projeto..."
dotnet build

# Define a pasta onde o binário foi gerado
BINARY_FOLDER="$PROJECT_PATH/bin/Debug/net8.0"

if [ ! -d "$BINARY_FOLDER" ]; then
    echo "Pasta do binario nao encontrada em $BINARY_FOLDER."
    exit 1
fi

cd "$BINARY_FOLDER"
echo "Entrou na pasta do binario: $BINARY_FOLDER"

# Procura o arquivo executável do bot (assumindo que seu nome contenha 'MythosBot')
BOT_EXE=$(find . -maxdepth 1 -type f -name "*.exe" | grep "MythosBot" | head -n 1)

if [ -n "$BOT_EXE" ]; then
    echo "Iniciando o bot em um novo processo..."
    nohup ./"$BOT_EXE" --ask-token &> /dev/null &
    echo "Bot iniciado com sucesso!"
else
    echo "Arquivo executavel do bot nao encontrado. Verifique se a compilacao ocorreu corretamente."
fi

# Restaura a política de execução original
umask "$CURRENT_POLICY"
echo "Politica de execucao restaurada para: $CURRENT_POLICY"

exit 0

