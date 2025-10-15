# Script para publicar la aplicación UI en ambiente QA
# Uso: .\publish-qa.ps1

param(
    [string]$OutputPath = "bin\Publish\QA"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Publicando UI - Ambiente: QA" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Limpiar publicación anterior
if (Test-Path $OutputPath) {
    Write-Host "Limpiando publicación anterior..." -ForegroundColor Yellow
    Remove-Item -Path $OutputPath -Recurse -Force
}

# Publicar usando el perfil QA
Write-Host "Iniciando publicación..." -ForegroundColor Green
msbuild ui.csproj `
    /p:Configuration=QA `
    /p:DeployOnBuild=true `
    /p:PublishProfile=QA `
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
    Write-Host "  - ApiBaseUrl: https://localhost:5002/api" -ForegroundColor White
    Write-Host ""
    Write-Host "NOTA: Actualiza las URLs en Web.QA.config con los valores reales" -ForegroundColor Yellow
} else {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "Error en la publicación" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    exit $LASTEXITCODE
}
