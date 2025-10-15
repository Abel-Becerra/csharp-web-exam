# Perfiles de Publicación - UI Project

Este documento describe los perfiles de publicación disponibles y cómo usarlos para desplegar la aplicación en diferentes ambientes.

## Perfiles Disponibles

### 1. FileSystem Profiles (Publicación a Carpeta Local)

#### Development.pubxml
- **Configuración**: Development
- **Destino**: `bin\Publish\Development`
- **Uso**: Desarrollo y pruebas locales
- **ApiBaseUrl**: `https://localhost:5001/api`
- **Debug Symbols**: Sí

#### QA.pubxml
- **Configuración**: QA
- **Destino**: `bin\Publish\QA`
- **Uso**: Ambiente de pruebas/calidad
- **ApiBaseUrl**: `https://qa-api.example.com/api` (actualizar)
- **Debug Symbols**: Sí

#### Production.pubxml
- **Configuración**: Production
- **Destino**: `bin\Publish\Production`
- **Uso**: Ambiente de producción
- **ApiBaseUrl**: `https://api.example.com/api` (actualizar)
- **Debug Symbols**: No

### 2. IIS Profiles (Publicación a Servidor IIS)

#### Development-IIS.pubxml
- **Configuración**: Development
- **Método**: MSDeploy (InProc)
- **Servidor**: localhost
- **IIS Path**: `Default Web Site/csharp-web-exam-dev`
- **URL**: `https://localhost:44339`

#### QA-IIS.pubxml
- **Configuración**: QA
- **Método**: MSDeploy (WMSVC)
- **Servidor**: `qa-server.example.com` (actualizar)
- **IIS Path**: `Default Web Site/csharp-web-exam-qa` (actualizar)
- **Usuario**: `deploy-user` (actualizar)

#### Production-IIS.pubxml
- **Configuración**: Production
- **Método**: MSDeploy (WMSVC)
- **Servidor**: `prod-server.example.com` (actualizar)
- **IIS Path**: `Default Web Site/csharp-web-exam` (actualizar)
- **Usuario**: `deploy-user` (actualizar)

## Métodos de Publicación

### 1. Visual Studio (GUI)

1. Click derecho en el proyecto **ui** en el Solution Explorer
2. Selecciona **Publish...**
3. Elige el perfil deseado de la lista
4. Click en **Publish**

### 2. Scripts PowerShell (Recomendado)

Los scripts automatizan el proceso y validan la configuración:

```powershell
# Development
cd csharp-web-exam\ui
.\publish-development.ps1

# QA
.\publish-qa.ps1

# Production (requiere confirmación)
.\publish-production.ps1

# Production sin confirmación (para CI/CD)
.\publish-production.ps1 -SkipConfirmation
```

### 3. MSBuild (Línea de Comandos)

```powershell
# Publicar a carpeta local
msbuild ui.csproj `
    /p:Configuration=QA `
    /p:DeployOnBuild=true `
    /p:PublishProfile=QA

# Publicar a IIS
msbuild ui.csproj `
    /p:Configuration=Production `
    /p:DeployOnBuild=true `
    /p:PublishProfile=Production-IIS `
    /p:Password=<tu-password>
```

### 4. Integración Continua (CI/CD)

#### Azure DevOps Pipeline

```yaml
- task: MSBuild@1
  inputs:
    solution: 'csharp-web-exam/ui/ui.csproj'
    msbuildArguments: '/p:Configuration=Production /p:DeployOnBuild=true /p:PublishProfile=Production'
    platform: 'Any CPU'
```

#### GitHub Actions

```yaml
- name: Publish UI
  run: |
    msbuild csharp-web-exam/ui/ui.csproj `
      /p:Configuration=Production `
      /p:DeployOnBuild=true `
      /p:PublishProfile=Production
```

## Configuración de Perfiles IIS

### Prerrequisitos

1. **Web Deploy** instalado en el servidor destino
2. **Credenciales de despliegue** configuradas
3. **Firewall** permitiendo conexiones en puerto 8172 (Web Deploy)

### Actualizar Perfiles IIS

Edita los archivos `.pubxml` en `Properties/PublishProfiles/`:

```xml
<MSDeployServiceURL>tu-servidor.com</MSDeployServiceURL>
<DeployIisAppPath>Default Web Site/tu-app</DeployIisAppPath>
<UserName>tu-usuario</UserName>
```

### Publicar con Credenciales

```powershell
msbuild ui.csproj `
    /p:Configuration=Production `
    /p:DeployOnBuild=true `
    /p:PublishProfile=Production-IIS `
    /p:Password=TuPassword `
    /p:AllowUntrustedCertificate=true
```

## Verificación Post-Publicación

### 1. Verificar Transformaciones

Después de publicar, verifica que el `Web.config` tenga los valores correctos:

```powershell
# Para publicación FileSystem
Get-Content bin\Publish\QA\Web.config | Select-String "ApiBaseUrl"

# Debería mostrar la URL del ambiente correcto
```

### 2. Verificar Archivos Publicados

```powershell
# Listar archivos publicados
Get-ChildItem -Path bin\Publish\Production -Recurse | Select-Object FullName
```

### 3. Probar la Aplicación

```powershell
# Si publicaste localmente, puedes probar con IIS Express
# O copiar los archivos a IIS y probar en el navegador
```

## Troubleshooting

### Las transformaciones no se aplican

**Problema**: El Web.config publicado tiene los valores del archivo base.

**Solución**:
1. Verifica que el perfil use la configuración correcta: `<LastUsedBuildConfiguration>QA</LastUsedBuildConfiguration>`
2. Asegúrate de usar `/p:Configuration=QA` al publicar
3. Limpia la solución: `msbuild /t:Clean`

### Error de conexión a IIS

**Problema**: No se puede conectar al servidor IIS remoto.

**Solución**:
1. Verifica que Web Deploy esté instalado: `Get-WindowsFeature -Name Web-Deploy`
2. Verifica el firewall: `Test-NetConnection -ComputerName servidor -Port 8172`
3. Verifica las credenciales en el perfil

### Archivos faltantes después de publicar

**Problema**: Algunos archivos no se copian.

**Solución**:
1. Verifica que los archivos estén incluidos en el `.csproj`
2. Cambia `<SkipExtraFilesOnServer>` a `False` si quieres mantener archivos adicionales
3. Usa `<ExcludeApp_Data>False</ExcludeApp_Data>` si necesitas publicar App_Data

### Error de permisos en IIS

**Problema**: La aplicación no funciona después de publicar.

**Solución**:
1. Verifica permisos de la carpeta en IIS: `IIS_IUSRS` debe tener acceso
2. Verifica el Application Pool identity
3. Revisa los logs de IIS: `C:\inetpub\logs\LogFiles`

## Personalización Avanzada

### Excluir Archivos de la Publicación

Edita el `.pubxml`:

```xml
<ItemGroup>
  <MsDeploySkipRules Include="SkipConfigFiles">
    <ObjectName>filePath</ObjectName>
    <AbsolutePath>Web.config</AbsolutePath>
  </MsDeploySkipRules>
</ItemGroup>
```

### Pre/Post Build Events

Agrega al `.pubxml`:

```xml
<Target Name="CustomPrePublish" BeforeTargets="BeforePublish">
  <Message Text="Ejecutando tareas pre-publicación..." />
  <!-- Tus comandos aquí -->
</Target>

<Target Name="CustomPostPublish" AfterTargets="AfterPublish">
  <Message Text="Ejecutando tareas post-publicación..." />
  <!-- Tus comandos aquí -->
</Target>
```

### Publicar con Precompilación

```xml
<PrecompileBeforePublish>True</PrecompileBeforePublish>
<EnableUpdateable>False</EnableUpdateable>
```

## Best Practices

1. **Nunca guardes contraseñas** en los archivos `.pubxml`
2. **Usa variables de entorno** para información sensible en CI/CD
3. **Prueba en QA** antes de publicar en Production
4. **Mantén backups** antes de publicar en Production
5. **Documenta cambios** en las URLs y configuraciones
6. **Usa el script de Production** que requiere confirmación
7. **Revisa los logs** después de cada publicación

## Recursos Adicionales

- [Web.config Transformations](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/transform-webconfig)
- [Web Deploy](https://www.iis.net/downloads/microsoft/web-deploy)
- [MSBuild Reference](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-reference)
