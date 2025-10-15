# Script para publicar la aplicación UI en ambiente Development
# Uso: .\publish-development.ps1

param(
    [string]$OutputPath = "bin\Publish\Development"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Publicando UI - Ambiente: Development" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Limpiar publicación anterior
if (Test-Path $OutputPath) {
    Write-Host "Limpiando publicación anterior..." -ForegroundColor Yellow
    Remove-Item -Path $OutputPath -Recurse -Force
}

# Publicar usando el perfil Development
Write-Host "Iniciando publicación..." -ForegroundColor Green
msbuild ui.csproj `
    /p:Configuration=Development `
    /p:DeployOnBuild=true `
    /p:PublishProfile=Development `
    /p:VisualStudioVersion=15.0 `
    /verbosity:minimal

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "Publicación completada exitosamente!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "Ubicación: $OutputPath" -ForegroundColor White
    Write-Host ""
    Write-Host "Configuración aplicada:" -ForegroundColor Cyan
    Write-Host "  - ApiBaseUrl: https://localhost:5001/api" -ForegroundColor White
} else {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "Error en la publicación" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    exit $LASTEXITCODE
}
