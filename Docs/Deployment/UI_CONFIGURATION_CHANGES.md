# Cambios en la Configuración del Proyecto UI

## Resumen
Se ha modificado el proyecto UI para soportar configuración basada en ambientes (Development, QA, Production) con la capacidad de agregar más settings fácilmente.

## Archivos Creados

### 1. Configuration/AppSettings.cs
Clase estática centralizada para acceder a la configuración de la aplicación de manera tipada.

**Características:**
- Propiedad `ApiBaseUrl` para acceder a la URL base de la API
- Métodos utilitarios para obtener settings como string, int, o bool
- Valores por defecto configurables

**Uso:**
```csharp
using csharp_web_exam.Configuration;

string apiUrl = AppSettings.ApiBaseUrl;
int timeout = AppSettings.GetSettingAsInt("Timeout", 30);
bool enabled = AppSettings.GetSettingAsBool("FeatureEnabled", false);
```

### 2. Web.Development.config
Archivo de transformación para el ambiente de desarrollo.
- **ApiBaseUrl**: `https://localhost:5001/api`

### 3. Web.QA.config
Archivo de transformación para el ambiente de QA.
- **ApiBaseUrl**: `https://qa-api.example.com/api` (actualizar con URL real)

### 4. Web.Production.config
Archivo de transformación para el ambiente de producción.
- **ApiBaseUrl**: `https://api.example.com/api` (actualizar con URL real)

### 5. Configuration/README.md
Documentación completa sobre cómo usar el sistema de configuración por ambientes.

## Archivos Modificados

### 1. Services/ApiClient.cs
- Agregado `using csharp_web_exam.Configuration;`
- Modificado el constructor para usar `AppSettings.ApiBaseUrl` en lugar de leer directamente de `ConfigurationManager`
- Agregado logging de la URL base al inicializar

**Antes:**
```csharp
_baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "https://localhost:5001/api";
```

**Después:**
```csharp
_baseUrl = AppSettings.ApiBaseUrl;
_log.Info($"ApiClient initialized with base URL: {_baseUrl}");
```

### 2. ui.csproj
- Agregado `Configuration\AppSettings.cs` a los archivos compilados
- Agregados los archivos de transformación:
  - `Web.Development.config`
  - `Web.QA.config`
  - `Web.Production.config`
- Agregadas configuraciones de build para Development, QA y Production con sus respectivas constantes de compilación

## Configuraciones de Build

### Development
- **DefineConstants**: `DEBUG;TRACE;DEVELOPMENT`
- **Optimize**: false
- **DebugSymbols**: true

### QA
- **DefineConstants**: `TRACE;QA`
- **Optimize**: true
- **DebugSymbols**: true

### Production
- **DefineConstants**: `TRACE;PRODUCTION`
- **Optimize**: true
- **DebugSymbols**: true

## Cómo Usar

### En Visual Studio
1. Selecciona la configuración deseada en el dropdown (Development, QA, Production)
2. Al publicar, las transformaciones se aplicarán automáticamente

### Desde Línea de Comandos
```powershell
# Build para un ambiente específico
msbuild ui.csproj /p:Configuration=QA

# Publicar para un ambiente específico
msbuild ui.csproj /p:Configuration=Production /p:DeployOnBuild=true
```

## Agregar Nuevos Settings

### Paso 1: Agregar al Web.config base
```xml
<appSettings>
  <add key="NuevoSetting" value="valorPorDefecto" />
</appSettings>
```

### Paso 2: Agregar transformaciones en cada ambiente
En `Web.Development.config`, `Web.QA.config`, `Web.Production.config`:
```xml
<appSettings>
  <add key="NuevoSetting" value="valorParaEsteAmbiente" 
       xdt:Transform="SetAttributes" xdt:Locator="Match(key)" />
</appSettings>
```

### Paso 3: (Opcional) Agregar propiedad tipada en AppSettings.cs
```csharp
public static string NuevoSetting => GetSetting("NuevoSetting", "valorPorDefecto");
```

## Beneficios

1. **Centralización**: Toda la configuración se accede a través de la clase `AppSettings`
2. **Tipo Seguro**: Propiedades tipadas reducen errores de runtime
3. **Ambiente Específico**: Cada ambiente tiene su propia configuración
4. **Escalable**: Fácil agregar nuevos settings sin modificar código existente
5. **Documentado**: README completo en la carpeta Configuration

## Perfiles de Publicación

Se han creado perfiles de publicación para facilitar el despliegue:

### Perfiles FileSystem
- **Development.pubxml**: Publica a `bin\Publish\Development`
- **QA.pubxml**: Publica a `bin\Publish\QA`
- **Production.pubxml**: Publica a `bin\Publish\Production`

### Perfiles IIS
- **Development-IIS.pubxml**: Publica a IIS local
- **QA-IIS.pubxml**: Publica a servidor IIS de QA
- **Production-IIS.pubxml**: Publica a servidor IIS de producción

### Scripts PowerShell
- **publish-development.ps1**: Script automatizado para Development
- **publish-qa.ps1**: Script automatizado para QA
- **publish-production.ps1**: Script automatizado para Production (con confirmación)

Ver `PUBLISH_PROFILES.md` para documentación completa.

## Próximos Pasos

1. Actualizar las URLs de QA y Production en los archivos de transformación con las URLs reales
2. Actualizar los perfiles IIS (`*-IIS.pubxml`) con las URLs y credenciales de servidores reales
3. Agregar más settings según sea necesario (timeouts, feature flags, etc.)
4. Considerar agregar configuración de connection strings si se requiere
5. Configurar Web Deploy en los servidores de QA y Production si se usarán los perfiles IIS

## Notas Importantes

- Las transformaciones solo se aplican durante el proceso de publicación o build con configuración específica
- En desarrollo local (F5), se usa el `Web.config` base sin transformaciones
- Los valores en los archivos de transformación son ejemplos y deben actualizarse con valores reales
- La clase `AppSettings` puede extenderse con más propiedades según sea necesario

## Testing

Para verificar que las transformaciones funcionan correctamente:

```powershell
# Build con configuración específica
msbuild ui.csproj /p:Configuration=QA /t:TransformWebConfig

# Verificar el archivo transformado en obj\QA\TransformWebConfig\transformed\Web.config
```
