# UI Project - Sistema de Configuración y Publicación por Ambientes

## 📋 Índice de Documentación

Este proyecto incluye un sistema completo de configuración por ambientes y perfiles de publicación. A continuación encontrarás la guía de toda la documentación disponible.

### 🚀 Inicio Rápido
- **[QUICK_REFERENCE.md](QUICK_REFERENCE.md)** - Guía rápida para uso diario
  - Comandos de publicación
  - Configuraciones por ambiente
  - Solución rápida de problemas

### 📚 Documentación Completa

#### Configuración
- **[Configuration/README.md](Configuration/README.md)** - Guía completa del sistema de configuración
  - Clase AppSettings
  - Archivos de transformación
  - Cómo agregar nuevos settings
  - Uso en código

#### Perfiles de Publicación
- **[PUBLISH_PROFILES.md](PUBLISH_PROFILES.md)** - Documentación detallada de perfiles
  - Perfiles FileSystem
  - Perfiles IIS
  - Configuración avanzada
  - Troubleshooting

#### CI/CD
- **[CI_CD_EXAMPLES.md](CI_CD_EXAMPLES.md)** - Ejemplos de integración CI/CD
  - Azure DevOps Pipelines
  - GitHub Actions
  - GitLab CI/CD
  - Jenkins
  - Scripts PowerShell para CI/CD

### 📊 Resúmenes
- **[PUBLISH_PROFILES_SUMMARY.md](../../PUBLISH_PROFILES_SUMMARY.md)** - Resumen ejecutivo de perfiles
- **[UI_CONFIGURATION_CHANGES.md](../../UI_CONFIGURATION_CHANGES.md)** - Resumen de cambios realizados

## 🎯 Ambientes Disponibles

| Ambiente    | Configuración | ApiBaseUrl                        | Uso                          |
|-------------|---------------|-----------------------------------|------------------------------|
| Development | Development   | https://localhost:5001/api        | Desarrollo local             |
| QA          | QA            | https://qa-api.example.com/api    | Pruebas y validación         |
| Production  | Production    | https://api.example.com/api       | Ambiente de producción       |

## 🚀 Publicación Rápida

### Usando Scripts PowerShell (Recomendado)

```powershell
# Development
.\publish-development.ps1

# QA
.\publish-qa.ps1

# Production (requiere confirmación)
.\publish-production.ps1
```

### Usando Visual Studio

1. Click derecho en proyecto **ui**
2. Seleccionar **Publish...**
3. Elegir perfil (Development, QA, Production, etc.)
4. Click **Publish**

### Usando MSBuild

```powershell
# Publicar a carpeta local
msbuild ui.csproj /p:Configuration=QA /p:DeployOnBuild=true /p:PublishProfile=QA

# Publicar a IIS
msbuild ui.csproj /p:Configuration=Production /p:DeployOnBuild=true /p:PublishProfile=Production-IIS
```

## 📁 Estructura del Proyecto

```
ui/
├── Configuration/
│   ├── AppSettings.cs              # Clase de configuración centralizada
│   └── README.md                   # Documentación de configuración
│
├── Properties/
│   └── PublishProfiles/
│       ├── Development.pubxml      # Perfil FileSystem - Dev
│       ├── Development-IIS.pubxml  # Perfil IIS - Dev
│       ├── QA.pubxml              # Perfil FileSystem - QA
│       ├── QA-IIS.pubxml          # Perfil IIS - QA
│       ├── Production.pubxml      # Perfil FileSystem - Prod
│       └── Production-IIS.pubxml  # Perfil IIS - Prod
│
├── Controllers/                    # Controladores MVC
├── Models/                         # Modelos de vista
├── Services/                       # Servicios (ApiClient, etc.)
├── Views/                          # Vistas Razor
│
├── Web.config                      # Configuración base
├── Web.Development.config          # Transformación Development
├── Web.QA.config                   # Transformación QA
├── Web.Production.config           # Transformación Production
│
├── publish-development.ps1         # Script publicación Dev
├── publish-qa.ps1                  # Script publicación QA
├── publish-production.ps1          # Script publicación Prod
│
├── PUBLISH_PROFILES.md             # Guía de perfiles
├── QUICK_REFERENCE.md              # Referencia rápida
├── CI_CD_EXAMPLES.md               # Ejemplos CI/CD
└── README.md                       # Este archivo
```

## 💻 Uso de Configuración en Código

```csharp
using csharp_web_exam.Configuration;

// Obtener ApiBaseUrl
string apiUrl = AppSettings.ApiBaseUrl;

// Obtener otros settings
string value = AppSettings.GetSetting("MiClave", "valorPorDefecto");
int timeout = AppSettings.GetSettingAsInt("Timeout", 30);
bool enabled = AppSettings.GetSettingAsBool("FeatureEnabled", false);
```

## 🔧 Agregar Nuevo Setting

### 1. Web.config (Base)
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

## 📦 Perfiles de Publicación

### FileSystem (Carpeta Local)
- ✅ **Development.pubxml** → `bin\Publish\Development`
- ✅ **QA.pubxml** → `bin\Publish\QA`
- ✅ **Production.pubxml** → `bin\Publish\Production`

### IIS (Servidor)
- ✅ **Development-IIS.pubxml** → IIS Local
- ✅ **QA-IIS.pubxml** → Servidor IIS QA
- ✅ **Production-IIS.pubxml** → Servidor IIS Production

## ⚙️ Configuración Inicial

### Antes de Usar en QA/Production

1. **Actualizar URLs en transformaciones**
   - Editar `Web.QA.config`
   - Editar `Web.Production.config`

2. **Configurar perfiles IIS (si aplica)**
   - Editar `Properties/PublishProfiles/QA-IIS.pubxml`
   - Editar `Properties/PublishProfiles/Production-IIS.pubxml`

3. **Instalar Web Deploy en servidores (si aplica)**
   - Requerido para perfiles IIS

## 🎓 Flujo de Trabajo Recomendado

```
1. Desarrollo Local (F5 en Visual Studio)
   ↓
2. Publicar a Development
   .\publish-development.ps1
   ↓ Verificar localmente
3. Publicar a QA
   .\publish-qa.ps1
   ↓ Pruebas en QA
4. Publicar a Production
   .\publish-production.ps1
   ↓ Confirmar despliegue
5. Verificar en Production
```

## 🔐 Seguridad

- ✅ Scripts de Production requieren confirmación
- ✅ No se guardan contraseñas en archivos
- ✅ Usar variables de entorno para credenciales
- ✅ Backup automático en QA y Production (perfiles IIS)

## 🆘 Soporte y Troubleshooting

### Problemas Comunes

**Transformaciones no se aplican**
```powershell
msbuild ui.csproj /t:Clean,Rebuild /p:Configuration=QA
```

**Error de conexión IIS**
```powershell
Test-NetConnection -ComputerName servidor -Port 8172
```

**Ver configuración aplicada**
```powershell
Get-Content bin\Publish\QA\Web.config | Select-String "ApiBaseUrl"
```

### Documentación Detallada
- Ver [QUICK_REFERENCE.md](QUICK_REFERENCE.md) para soluciones rápidas
- Ver [PUBLISH_PROFILES.md](PUBLISH_PROFILES.md) para troubleshooting detallado

## 🔗 Enlaces Útiles

- [Web.config Transformations](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/transform-webconfig)
- [MSBuild Reference](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-reference)
- [Web Deploy](https://www.iis.net/downloads/microsoft/web-deploy)

## ✨ Características

- ✅ Configuración por ambiente (Development, QA, Production)
- ✅ Transformaciones automáticas de Web.config
- ✅ Clase AppSettings centralizada y tipada
- ✅ 6 perfiles de publicación (FileSystem + IIS)
- ✅ 3 scripts PowerShell automatizados
- ✅ Documentación completa
- ✅ Ejemplos de integración CI/CD
- ✅ Fácil de extender con nuevos settings

## 📝 Licencia

Este proyecto es parte de csharp-web-exam. Ver LICENSE en la raíz del repositorio.

## 👥 Contribuir

Para agregar nuevos ambientes o modificar configuraciones:
1. Seguir la estructura existente
2. Actualizar documentación correspondiente
3. Probar en todos los ambientes
4. Documentar cambios

---

**Última actualización**: 2025-10-14

Para más información, consulta la documentación específica en los enlaces de arriba.
