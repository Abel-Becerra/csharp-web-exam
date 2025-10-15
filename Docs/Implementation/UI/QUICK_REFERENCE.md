# Guía Rápida - Configuración y Publicación por Ambientes

## 🚀 Publicación Rápida

```powershell
# Development
.\publish-development.ps1

# QA
.\publish-qa.ps1

# Production
.\publish-production.ps1
```

## 📁 Estructura de Archivos

```
ui/
├── Configuration/
│   ├── AppSettings.cs              # Clase de configuración centralizada
│   └── README.md                   # Documentación completa
├── Properties/
│   └── PublishProfiles/
│       ├── Development.pubxml      # Perfil FileSystem - Dev
│       ├── Development-IIS.pubxml  # Perfil IIS - Dev
│       ├── QA.pubxml              # Perfil FileSystem - QA
│       ├── QA-IIS.pubxml          # Perfil IIS - QA
│       ├── Production.pubxml      # Perfil FileSystem - Prod
│       └── Production-IIS.pubxml  # Perfil IIS - Prod
├── Web.config                     # Configuración base
├── Web.Development.config         # Transformación Development
├── Web.QA.config                  # Transformación QA
├── Web.Production.config          # Transformación Production
├── publish-development.ps1        # Script publicación Dev
├── publish-qa.ps1                 # Script publicación QA
└── publish-production.ps1         # Script publicación Prod
```

## ⚙️ Configuraciones por Ambiente

| Ambiente    | ApiBaseUrl                        | Debug | Optimize |
|-------------|-----------------------------------|-------|----------|
| Development | https://localhost:5001/api        | ✅    | ❌       |
| QA          | https://qa-api.example.com/api    | ✅    | ✅       |
| Production  | https://api.example.com/api       | ❌    | ✅       |

## 💻 Uso en Código

```csharp
using csharp_web_exam.Configuration;

// Obtener ApiBaseUrl
string apiUrl = AppSettings.ApiBaseUrl;

// Obtener otros settings
string value = AppSettings.GetSetting("MiClave", "default");
int timeout = AppSettings.GetSettingAsInt("Timeout", 30);
bool enabled = AppSettings.GetSettingAsBool("Feature", false);
```

## 🔧 Agregar Nuevo Setting

### 1. Web.config
```xml
<appSettings>
  <add key="NuevoSetting" value="valorBase" />
</appSettings>
```

### 2. Web.[Ambiente].config
```xml
<appSettings>
  <add key="NuevoSetting" value="valorAmbiente" 
       xdt:Transform="SetAttributes" xdt:Locator="Match(key)" />
</appSettings>
```

### 3. AppSettings.cs (Opcional)
```csharp
public static string NuevoSetting => GetSetting("NuevoSetting", "default");
```

## 📦 Métodos de Publicación

### Visual Studio
1. Click derecho en proyecto → **Publish**
2. Selecciona perfil
3. Click **Publish**

### PowerShell Scripts
```powershell
.\publish-[ambiente].ps1
```

### MSBuild
```powershell
msbuild ui.csproj /p:Configuration=QA /p:DeployOnBuild=true /p:PublishProfile=QA
```

## 🔍 Verificar Configuración

```powershell
# Ver configuración aplicada
Get-Content bin\Publish\QA\Web.config | Select-String "ApiBaseUrl"

# Listar archivos publicados
Get-ChildItem bin\Publish\QA -Recurse
```

## 🏗️ Build Configurations

```powershell
# Build específico
msbuild ui.csproj /p:Configuration=Development

# Clean + Build
msbuild ui.csproj /t:Clean,Build /p:Configuration=QA

# Rebuild
msbuild ui.csproj /t:Rebuild /p:Configuration=Production
```

## 🎯 Destinos de Publicación

### FileSystem
- **Development**: `bin\Publish\Development`
- **QA**: `bin\Publish\QA`
- **Production**: `bin\Publish\Production`

### IIS
- **Development**: `localhost` → `Default Web Site/csharp-web-exam-dev`
- **QA**: `qa-server.example.com` → `Default Web Site/csharp-web-exam-qa`
- **Production**: `prod-server.example.com` → `Default Web Site/csharp-web-exam`

## ⚠️ Checklist Pre-Publicación

### QA
- [ ] Actualizar URL en `Web.QA.config`
- [ ] Actualizar perfil `QA-IIS.pubxml` si usas IIS
- [ ] Probar configuración localmente
- [ ] Verificar credenciales de despliegue

### Production
- [ ] Actualizar URL en `Web.Production.config`
- [ ] Actualizar perfil `Production-IIS.pubxml` si usas IIS
- [ ] Backup del ambiente actual
- [ ] Notificar al equipo
- [ ] Verificar en QA primero
- [ ] Usar script con confirmación

## 🐛 Troubleshooting Rápido

### Transformaciones no se aplican
```powershell
# Limpiar y rebuild
msbuild ui.csproj /t:Clean,Rebuild /p:Configuration=QA
```

### Error de conexión IIS
```powershell
# Verificar Web Deploy
Get-WindowsFeature -Name Web-Deploy

# Test conexión
Test-NetConnection -ComputerName servidor -Port 8172
```

### Archivos faltantes
- Verifica que estén en `.csproj`
- Revisa `<ExcludeApp_Data>` en perfil
- Usa `<DeleteExistingFiles>False</DeleteExistingFiles>`

## 📚 Documentación Completa

- **Configuration/README.md**: Guía completa de configuración
- **PUBLISH_PROFILES.md**: Guía detallada de perfiles de publicación
- **UI_CONFIGURATION_CHANGES.md**: Resumen de todos los cambios

## 🔗 Enlaces Útiles

- [Web.config Transformations](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/transform-webconfig)
- [MSBuild Reference](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-reference)
- [Web Deploy](https://www.iis.net/downloads/microsoft/web-deploy)

## 💡 Tips

1. Usa los scripts PowerShell para publicación consistente
2. Siempre prueba en QA antes de Production
3. El script de Production requiere confirmación por seguridad
4. Las transformaciones solo aplican al publicar, no en F5
5. Mantén las URLs actualizadas en los archivos de transformación
