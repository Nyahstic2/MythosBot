#!/bin/bash

# Salva a política de execução atual
CURRENT_POLICY=$(umask)
echo "Politica de execucao atual: $CURRENT_POLICY"

# Altera temporariamente a política de execução
umask 000
echo "Politica de execucao alterada para Unrestricted."

# Definição do diretório do projeto
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
dotnet build --no-restore

umask "$CURRENT_POLICY"
echo "Politica de execucao restaurada para: $CURRENT_POLICY"

exit 0
