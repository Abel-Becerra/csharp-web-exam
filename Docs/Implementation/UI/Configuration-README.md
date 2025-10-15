# Configuración por Ambiente

Este proyecto está configurado para soportar múltiples ambientes mediante transformaciones de Web.config y una clase centralizada de configuración.

## Ambientes Disponibles

- **Development**: Ambiente de desarrollo local
- **QA**: Ambiente de pruebas/calidad
- **Production**: Ambiente de producción

## Archivos de Configuración

### Web.config (Base)
Archivo de configuración base con valores por defecto para desarrollo local.

### Web.Development.config
Transformaciones específicas para el ambiente de desarrollo.
- ApiBaseUrl: `https://localhost:5001/api`

### Web.QA.config
Transformaciones específicas para el ambiente de QA.
- ApiBaseUrl: `https://qa-api.example.com/api`

### Web.Production.config
Transformaciones específicas para el ambiente de producción.
- ApiBaseUrl: `https://api.example.com/api`

## Clase AppSettings

La clase `AppSettings` proporciona acceso tipado y centralizado a la configuración de la aplicación.

### Propiedades Disponibles

```csharp
// URL base de la API
string apiUrl = AppSettings.ApiBaseUrl;
```

### Métodos Utilitarios

```csharp
// Obtener un setting como string
string value = AppSettings.GetSetting("MiClave", "valorPorDefecto");

// Obtener un setting como entero
int timeout = AppSettings.GetSettingAsInt("Timeout", 30);

// Obtener un setting como booleano
bool enabled = AppSettings.GetSettingAsBool("FeatureEnabled", false);
```

## Perfiles de Publicación

El proyecto incluye perfiles de publicación para cada ambiente:

### Perfiles FileSystem (Publicación a carpeta local)
- **Development.pubxml**: Publica a `bin\Publish\Development`
- **QA.pubxml**: Publica a `bin\Publish\QA`
- **Production.pubxml**: Publica a `bin\Publish\Production`

### Perfiles IIS (Publicación a servidor IIS)
- **Development-IIS.pubxml**: Publica a IIS local
- **QA-IIS.pubxml**: Publica a servidor IIS de QA
- **Production-IIS.pubxml**: Publica a servidor IIS de producción

## Cómo Usar

### En Visual Studio

1. Click derecho en el proyecto UI → **Publish**
2. Selecciona el perfil deseado (Development, QA, Production, etc.)
3. Click en **Publish**
4. Las transformaciones se aplicarán automáticamente

### Usando Scripts PowerShell (Recomendado)

```powershell
# Publicar Development
.\publish-development.ps1

# Publicar QA
.\publish-qa.ps1

# Publicar Production (requiere confirmación)
.\publish-production.ps1

# Publicar Production sin confirmación
.\publish-production.ps1 -SkipConfirmation
```

### Desde la Línea de Comandos

```powershell
# Build para un ambiente específico
msbuild ui.csproj /p:Configuration=QA

# Publicar usando un perfil específico
msbuild ui.csproj /p:Configuration=QA /p:DeployOnBuild=true /p:PublishProfile=QA

# Publicar a IIS
msbuild ui.csproj /p:Configuration=Production /p:DeployOnBuild=true /p:PublishProfile=Production-IIS
```

### Publicación con Transformaciones

```powershell
# Publicar para QA
msbuild ui.csproj /p:Configuration=QA /p:DeployOnBuild=true /p:PublishProfile=QA

# Publicar para Production
msbuild ui.csproj /p:Configuration=Production /p:DeployOnBuild=true /p:PublishProfile=Production
```

## Agregar Nuevas Configuraciones

### 1. Agregar al Web.config Base

```xml
<appSettings>
  <add key="NuevaSetting" value="valorPorDefecto" />
</appSettings>
```

### 2. Agregar Transformaciones por Ambiente

En `Web.Development.config`, `Web.QA.config`, y `Web.Production.config`:

```xml
<appSettings>
  <add key="NuevaSetting" value="valorParaEsteAmbiente" 
       xdt:Transform="SetAttributes" xdt:Locator="Match(key)" />
</appSettings>
```

### 3. (Opcional) Agregar Propiedad a AppSettings.cs

```csharp
public static class AppSettings
{
    public static string NuevaSetting => GetSetting("NuevaSetting", "valorPorDefecto");
}
```

## Ejemplo de Uso en Código

```csharp
using csharp_web_exam.Configuration;

public class MiServicio
{
    private readonly string _apiUrl;
    
    public MiServicio()
    {
        // Usar la propiedad tipada
        _apiUrl = AppSettings.ApiBaseUrl;
        
        // O usar el método genérico
        var timeout = AppSettings.GetSettingAsInt("ApiTimeout", 30);
    }
}
```

## Notas Importantes

- Las transformaciones solo se aplican durante el proceso de publicación o build con la configuración específica
- En desarrollo local (F5), se usa el `Web.config` base sin transformaciones
- Para probar transformaciones localmente, usa el comando de build con la configuración deseada
- Los valores en los archivos de transformación son ejemplos; actualízalos con las URLs reales de tus ambientes

## Troubleshooting

### Las transformaciones no se aplican
- Verifica que los archivos de transformación estén marcados como `DependentUpon` Web.config en el .csproj
- Asegúrate de estar usando la configuración correcta al compilar/publicar

### No puedo ver las configuraciones en Visual Studio
- Cierra y vuelve a abrir la solución
- Verifica que las PropertyGroup para cada configuración estén en el .csproj

### Errores de compilación
- Verifica que todos los archivos de configuración tengan XML válido
- Asegúrate de que el namespace `xmlns:xdt` esté presente en los archivos de transformación
