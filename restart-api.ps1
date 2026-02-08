# Script para reiniciar a API OpaMenu
# Este script para a API em execução e inicia novamente

Write-Host "=== Reiniciando OpaMenu API ===" -ForegroundColor Cyan

# Parar processo anterior
Write-Host "`nParando processo OpaMenu.Web..." -ForegroundColor Yellow
$process = Get-Process -Name "OpaMenu.Web" -ErrorAction SilentlyContinue
if ($process) {
    Stop-Process -Name "OpaMenu.Web" -Force
    Write-Host "Processo parado com sucesso!" -ForegroundColor Green
    Start-Sleep -Seconds 2
} else {
    Write-Host "Nenhum processo OpaMenu.Web encontrado em execução" -ForegroundColor Gray
}

# Navegar para o diretório do projeto
$projectPath = "d:\dev\opamenu-ecosistema\opamenu-api\OpaMenu.Web"
Set-Location $projectPath
Write-Host "`nDiretório atual: $projectPath" -ForegroundColor Gray

# Iniciar a API
Write-Host "`nIniciando API..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$projectPath'; dotnet run"

Write-Host "`nAPI em inicialização! Aguarde alguns segundos..." -ForegroundColor Green
Write-Host "Uma nova janela do PowerShell foi aberta com a API rodando.`n" -ForegroundColor Cyan
