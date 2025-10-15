# Script para publicar la aplicación UI en ambiente Production
# Uso: .\publish-production.ps1

param(
    [string]$OutputPath = "bin\Publish\Production",
    [switch]$SkipConfirmation
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Publicando UI - Ambiente: PRODUCTION" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Confirmación para producción
if (-not $SkipConfirmation) {
    Write-Host "ADVERTENCIA: Estás a punto de publicar en PRODUCCIÓN" -ForegroundColor Red
    $confirmation = Read-Host "¿Estás seguro? (escribe 'SI' para continuar)"
    if ($confirmation -ne "SI") {
        Write-Host "Publicación cancelada" -ForegroundColor Yellow
        exit 0
    }
}

# Limpiar publicación anterior
if (Test-Path $OutputPath) {
    Write-Host "Limpiando publicación anterior..." -ForegroundColor Yellow
    Remove-Item -Path $OutputPath -Recurse -Force
}

# Publicar usando el perfil Production
Write-Host "Iniciando publicación..." -ForegroundColor Green
msbuild ui.csproj `
    /p:Configuration=Production `
    /p:DeployOnBuild=true `
    /p:PublishProfile=Production `
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
    Write-Host "  - ApiBaseUrl: https://localhost:5004/api" -ForegroundColor White
    Write-Host ""
    Write-Host "NOTA: Actualiza las URLs en Web.Production.config con los valores reales" -ForegroundColor Yellow
} else {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "Error en la publicación" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    exit $LASTEXITCODE
}
