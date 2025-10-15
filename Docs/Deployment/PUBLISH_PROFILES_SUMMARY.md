# Resumen - Perfiles de Publicación por Ambiente

## ✅ Archivos Creados

### Perfiles de Publicación (6 archivos)
```
ui/Properties/PublishProfiles/
├── Development.pubxml          # FileSystem - Development
├── Development-IIS.pubxml      # IIS - Development
├── QA.pubxml                   # FileSystem - QA
├── QA-IIS.pubxml               # IIS - QA
├── Production.pubxml           # FileSystem - Production
└── Production-IIS.pubxml       # IIS - Production
```

### Scripts de Publicación (3 archivos)
```
ui/
├── publish-development.ps1     # Script automatizado - Development
├── publish-qa.ps1              # Script automatizado - QA
└── publish-production.ps1      # Script automatizado - Production (con confirmación)
```

### Documentación (3 archivos)
```
ui/
├── PUBLISH_PROFILES.md         # Guía detallada de perfiles
├── QUICK_REFERENCE.md          # Referencia rápida
└── Configuration/README.md     # (Actualizado con info de perfiles)
```

## 🎯 Características Principales

### Perfiles FileSystem
- ✅ Publicación a carpeta local
- ✅ Transformaciones automáticas de Web.config
- ✅ Limpieza de archivos anteriores
- ✅ Configuración específica por ambiente
- ✅ Fácil para copiar a servidores manualmente

### Perfiles IIS
- ✅ Publicación directa a IIS (local o remoto)
- ✅ Web Deploy (MSDeploy)
- ✅ Backup automático (QA y Production)
- ✅ Configuración de credenciales
- ✅ Ideal para automatización CI/CD

### Scripts PowerShell
- ✅ Interfaz amigable con colores
- ✅ Validación de errores
- ✅ Limpieza automática
- ✅ Confirmación para Production
- ✅ Mensajes informativos

## 📊 Matriz de Configuración

| Ambiente    | Perfil FileSystem | Perfil IIS          | Script PowerShell          | Destino                              |
|-------------|-------------------|---------------------|----------------------------|--------------------------------------|
| Development | ✅ Development     | ✅ Development-IIS   | ✅ publish-development.ps1  | bin\Publish\Development              |
| QA          | ✅ QA              | ✅ QA-IIS            | ✅ publish-qa.ps1           | bin\Publish\QA                       |
| Production  | ✅ Production      | ✅ Production-IIS    | ✅ publish-production.ps1   | bin\Publish\Production               |

## 🚀 Uso Rápido

### Opción 1: Scripts PowerShell (Recomendado)
```powershell
cd csharp-web-exam\ui

# Development
.\publish-development.ps1

# QA
.\publish-qa.ps1

# Production (requiere confirmación)
.\publish-production.ps1
```

### Opción 2: Visual Studio
1. Click derecho en proyecto **ui**
2. Seleccionar **Publish...**
3. Elegir perfil (Development, QA, Production, etc.)
4. Click **Publish**

### Opción 3: MSBuild
```powershell
# FileSystem
msbuild ui.csproj /p:Configuration=QA /p:DeployOnBuild=true /p:PublishProfile=QA

# IIS
msbuild ui.csproj /p:Configuration=Production /p:DeployOnBuild=true /p:PublishProfile=Production-IIS /p:Password=xxx
```

## 🔧 Configuración Requerida

### Antes de Usar

#### 1. Actualizar URLs en Web.QA.config
```xml
<add key="ApiBaseUrl" value="https://TU-SERVIDOR-QA/api" 
     xdt:Transform="SetAttributes" xdt:Locator="Match(key)" />
```

#### 2. Actualizar URLs en Web.Production.config
```xml
<add key="ApiBaseUrl" value="https://TU-SERVIDOR-PROD/api" 
     xdt:Transform="SetAttributes" xdt:Locator="Match(key)" />
```

#### 3. Actualizar Perfiles IIS (si se usan)

**QA-IIS.pubxml**:
```xml
<MSDeployServiceURL>tu-servidor-qa.com</MSDeployServiceURL>
<DeployIisAppPath>Default Web Site/tu-app-qa</DeployIisAppPath>
<UserName>tu-usuario</UserName>
```

**Production-IIS.pubxml**:
```xml
<MSDeployServiceURL>tu-servidor-prod.com</MSDeployServiceURL>
<DeployIisAppPath>Default Web Site/tu-app</DeployIisAppPath>
<UserName>tu-usuario</UserName>
```

## 📋 Checklist de Implementación

### Configuración Inicial
- [x] Perfiles de publicación creados
- [x] Scripts PowerShell creados
- [x] Documentación completa
- [x] Archivos agregados al .csproj
- [ ] URLs de QA actualizadas
- [ ] URLs de Production actualizadas
- [ ] Credenciales IIS configuradas (si aplica)
- [ ] Web Deploy instalado en servidores (si aplica)

### Pruebas
- [ ] Publicar Development localmente
- [ ] Verificar transformaciones en Development
- [ ] Publicar QA y verificar
- [ ] Probar aplicación en QA
- [ ] Publicar Production (después de QA)
- [ ] Verificar aplicación en Production

## 🎓 Flujo de Trabajo Recomendado

```
1. Desarrollo Local (F5)
   ↓
2. Publicar a Development (.\publish-development.ps1)
   ↓ Verificar
3. Publicar a QA (.\publish-qa.ps1)
   ↓ Pruebas QA
4. Publicar a Production (.\publish-production.ps1)
   ↓ Confirmar
5. Verificar en Production
```

## 🔐 Seguridad

### Buenas Prácticas
- ✅ Script de Production requiere confirmación
- ✅ No se guardan contraseñas en archivos
- ✅ Usar variables de entorno para credenciales
- ✅ Backup automático en QA y Production
- ✅ Logs de publicación para auditoría

### Credenciales
```powershell
# Usar variables de entorno
$env:DEPLOY_PASSWORD = "tu-password"

# O pasar como parámetro
msbuild /p:Password=$env:DEPLOY_PASSWORD
```

## 📚 Documentación Disponible

| Archivo                          | Descripción                                    |
|----------------------------------|------------------------------------------------|
| **QUICK_REFERENCE.md**           | Guía rápida de uso diario                     |
| **PUBLISH_PROFILES.md**          | Documentación detallada de perfiles           |
| **Configuration/README.md**      | Guía completa de configuración                |
| **UI_CONFIGURATION_CHANGES.md**  | Resumen de todos los cambios realizados       |
| **PUBLISH_PROFILES_SUMMARY.md**  | Este documento - Resumen de perfiles          |

## 🆘 Soporte

### Problemas Comunes

**Transformaciones no se aplican**
- Verifica que uses la configuración correcta al publicar
- Limpia y rebuild: `msbuild /t:Clean,Rebuild /p:Configuration=QA`

**Error de conexión IIS**
- Verifica Web Deploy: `Get-WindowsFeature -Name Web-Deploy`
- Test conexión: `Test-NetConnection -ComputerName servidor -Port 8172`

**Archivos faltantes**
- Verifica que estén en el .csproj
- Revisa configuración de exclusión en el perfil

### Recursos
- Ver logs en: `csharp-web-exam\ui\obj\[Configuration]\Package\PackageTmp\`
- Documentación completa en archivos mencionados arriba

## ✨ Beneficios

1. **Automatización**: Scripts listos para usar
2. **Consistencia**: Mismo proceso para todos los ambientes
3. **Seguridad**: Confirmación en Production
4. **Flexibilidad**: Múltiples métodos de publicación
5. **Documentación**: Guías completas y ejemplos
6. **Escalabilidad**: Fácil agregar nuevos ambientes
7. **CI/CD Ready**: Compatible con pipelines

## 🎉 Próximos Pasos

1. Actualizar URLs en archivos de transformación
2. Configurar credenciales IIS si es necesario
3. Probar publicación en Development
4. Probar publicación en QA
5. Documentar URLs y credenciales en lugar seguro
6. Entrenar al equipo en el uso de scripts
7. Integrar con pipeline CI/CD si aplica

---

**Nota**: Todos los archivos están incluidos en el proyecto y listos para usar. Solo necesitas actualizar las URLs y credenciales específicas de tu infraestructura.
