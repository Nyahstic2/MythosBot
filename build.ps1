# Armazena a política de execução atual (escopo CurrentUser)
$originalPolicy = Get-ExecutionPolicy -Scope CurrentUser
Write-Output "Politica de execucao atual: $originalPolicy"

# Altera para Unrestricted para que o script execute sem restrições
Set-ExecutionPolicy Unrestricted -Scope CurrentUser -Force
Write-Output "Politica de execucao alterada para Unrestricted."

try {
    # Muda para a pasta MythosBot relativa ao local do script
    $projectPath = Join-Path -Path (Get-Location) -ChildPath "MythosBot"
    if (-not (Test-Path $projectPath)) {
        Write-Output "Pasta MythosBot nao encontrada. Verifique a localizacao do script."
        exit 1
    }
    Set-Location $projectPath
    Write-Output "Entrou na pasta $projectPath"

    # Verifica se o comando dotnet está disponível
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        Write-Output "Dotnet nao encontrado. Iniciando a instalacao do Dotnet 8 SDK..."
        if (Get-Command winget -ErrorAction SilentlyContinue) {
            winget install --id Microsoft.DotNet.SDK.8 -e --source winget
        }
        else {
            Write-Output "Winget nao encontrado. Por favor, instale manualmente o Dotnet 8 SDK a partir de https://dotnet.microsoft.com/download/dotnet/8.0"
            exit 1
        }
    }
    else {
        Write-Output "Dotnet encontrado: $(dotnet --version)"
    }

    # Restaura os pacotes e compila o projeto
    Write-Output "Restaurando as dependencias..."
    dotnet restore

    Write-Output "Compilando o projeto..."
    dotnet build

    # Define a pasta onde o binário foi gerado
    $binaryFolder = Join-Path -Path (Get-Location) -ChildPath "bin\Debug\net8.0"
    if (-not (Test-Path $binaryFolder)) {
        Write-Output "Pasta do binario nao encontrada em $binaryFolder."
        exit 1
    }
    Set-Location $binaryFolder
    Write-Output "Entrou na pasta do binario: $binaryFolder"

    # Procura o arquivo executável do bot (assumindo que seu nome contenha 'MythosBot')
    $botExe = Get-ChildItem -Filter "*.exe" | Where-Object { $_.Name -match "MythosBot" } | Select-Object -First 1

    if ($botExe) {
        Write-Output "Iniciando o bot em um novo processo..."
        Start-Process -FilePath $botExe.FullName -ArgumentList "--ask-token"
		Set-Location "..\..\..\.."
    }
    else {
        Write-Output "Arquivo executavel do bot nao encontrado. Verifique se a compilacao ocorreu corretamente."
    }
}
finally {
    # Restaura a política de execução original
    Set-ExecutionPolicy $originalPolicy -Scope CurrentUser -Force
    Write-Output "Politica de execucao restaurada para: $originalPolicy"
}
