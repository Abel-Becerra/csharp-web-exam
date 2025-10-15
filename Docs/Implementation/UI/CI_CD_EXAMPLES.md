# Ejemplos de Integración CI/CD

Este documento proporciona ejemplos de configuración para integrar los perfiles de publicación en diferentes plataformas de CI/CD.

## Azure DevOps Pipelines

### Pipeline YAML - Multi-Stage

```yaml
trigger:
  branches:
    include:
    - main
    - develop
    - release/*

variables:
  solution: 'csharp-web-exam/csharp-web-exam.sln'
  uiProject: 'csharp-web-exam/ui/ui.csproj'
  buildPlatform: 'Any CPU'

stages:
- stage: Build
  displayName: 'Build Stage'
  jobs:
  - job: Build
    displayName: 'Build Job'
    pool:
      vmImage: 'windows-latest'
    steps:
    - task: NuGetToolInstaller@1
      displayName: 'Install NuGet'
    
    - task: NuGetCommand@2
      displayName: 'Restore NuGet Packages'
      inputs:
        restoreSolution: '$(solution)'
    
    - task: VSBuild@1
      displayName: 'Build Solution'
      inputs:
        solution: '$(solution)'
        platform: '$(buildPlatform)'
        configuration: 'Release'

- stage: DeployQA
  displayName: 'Deploy to QA'
  dependsOn: Build
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/develop'))
  jobs:
  - deployment: DeployQA
    displayName: 'Deploy to QA Environment'
    environment: 'QA'
    pool:
      vmImage: 'windows-latest'
    strategy:
      runOnce:
        deploy:
          steps:
          - checkout: self
          
          - task: NuGetCommand@2
            displayName: 'Restore NuGet Packages'
            inputs:
              restoreSolution: '$(solution)'
          
          - task: MSBuild@1
            displayName: 'Publish UI to QA'
            inputs:
              solution: '$(uiProject)'
              msbuildArguments: '/p:Configuration=QA /p:DeployOnBuild=true /p:PublishProfile=QA'
              platform: '$(buildPlatform)'
          
          - task: PublishBuildArtifacts@1
            displayName: 'Publish Artifacts'
            inputs:
              PathtoPublish: 'csharp-web-exam/ui/bin/Publish/QA'
              ArtifactName: 'ui-qa'

- stage: DeployProduction
  displayName: 'Deploy to Production'
  dependsOn: DeployQA
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  jobs:
  - deployment: DeployProduction
    displayName: 'Deploy to Production Environment'
    environment: 'Production'
    pool:
      vmImage: 'windows-latest'
    strategy:
      runOnce:
        deploy:
          steps:
          - checkout: self
          
          - task: NuGetCommand@2
            displayName: 'Restore NuGet Packages'
            inputs:
              restoreSolution: '$(solution)'
          
          - task: MSBuild@1
            displayName: 'Publish UI to Production'
            inputs:
              solution: '$(uiProject)'
              msbuildArguments: '/p:Configuration=Production /p:DeployOnBuild=true /p:PublishProfile=Production'
              platform: '$(buildPlatform)'
          
          - task: PublishBuildArtifacts@1
            displayName: 'Publish Artifacts'
            inputs:
              PathtoPublish: 'csharp-web-exam/ui/bin/Publish/Production'
              ArtifactName: 'ui-production'
```

### Pipeline YAML - Deploy to IIS

```yaml
- stage: DeployToIIS
  displayName: 'Deploy to IIS'
  jobs:
  - deployment: DeployIIS
    displayName: 'Deploy to IIS Server'
    environment: 'Production'
    pool:
      vmImage: 'windows-latest'
    variables:
      deployPassword: $(IIS_DEPLOY_PASSWORD) # Variable secreta en Azure DevOps
    strategy:
      runOnce:
        deploy:
          steps:
          - checkout: self
          
          - task: NuGetCommand@2
            inputs:
              restoreSolution: '$(solution)'
          
          - task: MSBuild@1
            displayName: 'Deploy to IIS'
            inputs:
              solution: '$(uiProject)'
              msbuildArguments: '/p:Configuration=Production /p:DeployOnBuild=true /p:PublishProfile=Production-IIS /p:Password=$(deployPassword) /p:AllowUntrustedCertificate=true'
              platform: '$(buildPlatform)'
```

## GitHub Actions

### Workflow - Multi-Environment

```yaml
name: Build and Deploy UI

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

env:
  SOLUTION_PATH: csharp-web-exam/csharp-web-exam.sln
  UI_PROJECT_PATH: csharp-web-exam/ui/ui.csproj

jobs:
  build:
    runs-on: windows-latest
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup MSBuild
      uses: microsoft/setup-msbuild@v1
    
    - name: Setup NuGet
      uses: NuGet/setup-nuget@v1
    
    - name: Restore NuGet packages
      run: nuget restore ${{ env.SOLUTION_PATH }}
    
    - name: Build Solution
      run: msbuild ${{ env.SOLUTION_PATH }} /p:Configuration=Release /p:Platform="Any CPU"
    
    - name: Upload build artifacts
      uses: actions/upload-artifact@v3
      with:
        name: build-artifacts
        path: csharp-web-exam/ui/bin/

  deploy-qa:
    needs: build
    if: github.ref == 'refs/heads/develop'
    runs-on: windows-latest
    environment: QA
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup MSBuild
      uses: microsoft/setup-msbuild@v1
    
    - name: Setup NuGet
      uses: NuGet/setup-nuget@v1
    
    - name: Restore NuGet packages
      run: nuget restore ${{ env.SOLUTION_PATH }}
    
    - name: Publish to QA
      run: msbuild ${{ env.UI_PROJECT_PATH }} /p:Configuration=QA /p:DeployOnBuild=true /p:PublishProfile=QA
    
    - name: Upload QA artifacts
      uses: actions/upload-artifact@v3
      with:
        name: ui-qa
        path: csharp-web-exam/ui/bin/Publish/QA/

  deploy-production:
    needs: deploy-qa
    if: github.ref == 'refs/heads/main'
    runs-on: windows-latest
    environment: Production
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup MSBuild
      uses: microsoft/setup-msbuild@v1
    
    - name: Setup NuGet
      uses: NuGet/setup-nuget@v1
    
    - name: Restore NuGet packages
      run: nuget restore ${{ env.SOLUTION_PATH }}
    
    - name: Publish to Production
      run: msbuild ${{ env.UI_PROJECT_PATH }} /p:Configuration=Production /p:DeployOnBuild=true /p:PublishProfile=Production
    
    - name: Upload Production artifacts
      uses: actions/upload-artifact@v3
      with:
        name: ui-production
        path: csharp-web-exam/ui/bin/Publish/Production/
```

### Workflow - Deploy to IIS

```yaml
name: Deploy to IIS

on:
  workflow_dispatch:
    inputs:
      environment:
        description: 'Environment to deploy'
        required: true
        type: choice
        options:
          - QA
          - Production

jobs:
  deploy-to-iis:
    runs-on: windows-latest
    environment: ${{ github.event.inputs.environment }}
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup MSBuild
      uses: microsoft/setup-msbuild@v1
    
    - name: Setup NuGet
      uses: NuGet/setup-nuget@v1
    
    - name: Restore NuGet packages
      run: nuget restore csharp-web-exam/csharp-web-exam.sln
    
    - name: Deploy to IIS
      env:
        DEPLOY_PASSWORD: ${{ secrets.IIS_DEPLOY_PASSWORD }}
      run: |
        msbuild csharp-web-exam/ui/ui.csproj `
          /p:Configuration=${{ github.event.inputs.environment }} `
          /p:DeployOnBuild=true `
          /p:PublishProfile=${{ github.event.inputs.environment }}-IIS `
          /p:Password=$env:DEPLOY_PASSWORD `
          /p:AllowUntrustedCertificate=true
```

## GitLab CI/CD

### .gitlab-ci.yml

```yaml
stages:
  - build
  - deploy-qa
  - deploy-production

variables:
  SOLUTION_PATH: "csharp-web-exam/csharp-web-exam.sln"
  UI_PROJECT_PATH: "csharp-web-exam/ui/ui.csproj"

build:
  stage: build
  tags:
    - windows
  script:
    - nuget restore $SOLUTION_PATH
    - msbuild $SOLUTION_PATH /p:Configuration=Release /p:Platform="Any CPU"
  artifacts:
    paths:
      - csharp-web-exam/ui/bin/
    expire_in: 1 day

deploy-qa:
  stage: deploy-qa
  tags:
    - windows
  only:
    - develop
  environment:
    name: QA
  script:
    - nuget restore $SOLUTION_PATH
    - msbuild $UI_PROJECT_PATH /p:Configuration=QA /p:DeployOnBuild=true /p:PublishProfile=QA
  artifacts:
    paths:
      - csharp-web-exam/ui/bin/Publish/QA/
    expire_in: 7 days

deploy-production:
  stage: deploy-production
  tags:
    - windows
  only:
    - main
  when: manual
  environment:
    name: Production
  script:
    - nuget restore $SOLUTION_PATH
    - msbuild $UI_PROJECT_PATH /p:Configuration=Production /p:DeployOnBuild=true /p:PublishProfile=Production
  artifacts:
    paths:
      - csharp-web-exam/ui/bin/Publish/Production/
    expire_in: 30 days

deploy-to-iis:
  stage: deploy-production
  tags:
    - windows
  only:
    - main
  when: manual
  environment:
    name: Production
  script:
    - nuget restore $SOLUTION_PATH
    - |
      msbuild $UI_PROJECT_PATH `
        /p:Configuration=Production `
        /p:DeployOnBuild=true `
        /p:PublishProfile=Production-IIS `
        /p:Password=$env:IIS_DEPLOY_PASSWORD `
        /p:AllowUntrustedCertificate=true
```

## Jenkins Pipeline

### Jenkinsfile

```groovy
pipeline {
    agent {
        label 'windows'
    }
    
    parameters {
        choice(
            name: 'ENVIRONMENT',
            choices: ['Development', 'QA', 'Production'],
            description: 'Select deployment environment'
        )
        booleanParam(
            name: 'DEPLOY_TO_IIS',
            defaultValue: false,
            description: 'Deploy directly to IIS server'
        )
    }
    
    environment {
        SOLUTION_PATH = 'csharp-web-exam\\csharp-web-exam.sln'
        UI_PROJECT_PATH = 'csharp-web-exam\\ui\\ui.csproj'
        MSBUILD = 'C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe'
    }
    
    stages {
        stage('Restore') {
            steps {
                bat "nuget restore ${SOLUTION_PATH}"
            }
        }
        
        stage('Build') {
            steps {
                bat "\"${MSBUILD}\" ${SOLUTION_PATH} /p:Configuration=Release /p:Platform=\"Any CPU\""
            }
        }
        
        stage('Publish') {
            steps {
                script {
                    def profile = params.DEPLOY_TO_IIS ? "${params.ENVIRONMENT}-IIS" : params.ENVIRONMENT
                    def publishCmd = "\"${MSBUILD}\" ${UI_PROJECT_PATH} /p:Configuration=${params.ENVIRONMENT} /p:DeployOnBuild=true /p:PublishProfile=${profile}"
                    
                    if (params.DEPLOY_TO_IIS) {
                        withCredentials([string(credentialsId: 'iis-deploy-password', variable: 'DEPLOY_PASSWORD')]) {
                            publishCmd += " /p:Password=${DEPLOY_PASSWORD} /p:AllowUntrustedCertificate=true"
                        }
                    }
                    
                    bat publishCmd
                }
            }
        }
        
        stage('Archive') {
            steps {
                archiveArtifacts artifacts: "csharp-web-exam\\ui\\bin\\Publish\\${params.ENVIRONMENT}\\**\\*", fingerprint: true
            }
        }
    }
    
    post {
        success {
            echo "Deployment to ${params.ENVIRONMENT} completed successfully!"
        }
        failure {
            echo "Deployment to ${params.ENVIRONMENT} failed!"
        }
    }
}
```

## Scripts PowerShell para CI/CD

### Script Genérico

```powershell
# deploy-ci.ps1
param(
    [Parameter(Mandatory=$true)]
    [ValidateSet('Development','QA','Production')]
    [string]$Environment,
    
    [switch]$DeployToIIS,
    
    [string]$Password
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "CI/CD Deployment - Environment: $Environment" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Restore NuGet packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
nuget restore csharp-web-exam\csharp-web-exam.sln

if ($LASTEXITCODE -ne 0) {
    Write-Error "NuGet restore failed"
    exit 1
}

# Determine profile
$profile = if ($DeployToIIS) { "$Environment-IIS" } else { $Environment }

# Build publish command
$publishArgs = @(
    "csharp-web-exam\ui\ui.csproj",
    "/p:Configuration=$Environment",
    "/p:DeployOnBuild=true",
    "/p:PublishProfile=$profile",
    "/verbosity:minimal"
)

if ($DeployToIIS -and $Password) {
    $publishArgs += "/p:Password=$Password"
    $publishArgs += "/p:AllowUntrustedCertificate=true"
}

# Publish
Write-Host "Publishing to $Environment..." -ForegroundColor Green
& msbuild $publishArgs

if ($LASTEXITCODE -eq 0) {
    Write-Host "Deployment completed successfully!" -ForegroundColor Green
    exit 0
} else {
    Write-Error "Deployment failed"
    exit 1
}
```

### Uso en CI/CD

```powershell
# FileSystem deployment
.\deploy-ci.ps1 -Environment QA

# IIS deployment
.\deploy-ci.ps1 -Environment Production -DeployToIIS -Password $env:DEPLOY_PASSWORD
```

## Variables de Entorno Requeridas

### Azure DevOps
- `IIS_DEPLOY_PASSWORD`: Contraseña para despliegue IIS (Variable secreta)

### GitHub Actions
- `IIS_DEPLOY_PASSWORD`: Secret para despliegue IIS

### GitLab CI
- `IIS_DEPLOY_PASSWORD`: Variable protegida para despliegue IIS

### Jenkins
- `iis-deploy-password`: Credential tipo Secret Text

## Configuración de Ambientes

### Azure DevOps Environments
1. Ir a Pipelines → Environments
2. Crear ambientes: QA, Production
3. Configurar aprobaciones para Production

### GitHub Environments
1. Ir a Settings → Environments
2. Crear: QA, Production
3. Configurar protection rules para Production

### GitLab Environments
1. Configurar en `.gitlab-ci.yml`
2. Usar `when: manual` para Production

## Best Practices CI/CD

1. **Usar artifacts**: Guardar archivos publicados como artifacts
2. **Aprobaciones manuales**: Requerir aprobación para Production
3. **Variables secretas**: Nunca hardcodear contraseñas
4. **Rollback plan**: Mantener artifacts anteriores
5. **Logs detallados**: Usar `/verbosity:detailed` si hay errores
6. **Health checks**: Verificar aplicación después del deploy
7. **Notificaciones**: Configurar alertas de éxito/fallo

## Troubleshooting CI/CD

### Build falla en CI
- Verificar que MSBuild esté disponible
- Verificar versión de .NET Framework
- Revisar logs detallados

### Transformaciones no se aplican
- Verificar que se use la configuración correcta
- Agregar `/p:Configuration=QA` explícitamente

### Deploy a IIS falla
- Verificar Web Deploy en servidor
- Verificar credenciales
- Verificar firewall (puerto 8172)
- Usar `/p:AllowUntrustedCertificate=true` si es necesario

---

**Nota**: Estos ejemplos son plantillas base. Ajústalos según tu infraestructura y requisitos específicos.
