# JWT Authentication - Usage Guide

## 🎯 Overview

La API ahora requiere **autenticación JWT** para todos los endpoints de Categories y Products. Solo los endpoints de autenticación (`/api/auth/login` y `/api/auth/register`) son públicos.

## 🔐 Usuarios de Prueba

La base de datos se inicializa con 3 usuarios de prueba:

| Username | Password | Role | Email |
|----------|----------|------|-------|
| **admin** | `SampleEx4mF0rT3st!ñ` | Admin | admin@example.com |
| **user1** | `SampleEx4mF0rT3st!ñ` | User | user1@example.com |
| **user2** | `SampleEx4mF0rT3st!ñ` | User | user2@example.com |

## 🚀 Cómo Usar la API

### Paso 1: Iniciar la API

```powershell
cd c:\proyectos\csharp-web-exam\csharp-web-exam\api
dotnet restore
dotnet build
dotnet run --launch-profile Development
```

La API estará disponible en: `https://localhost:5001`

### Paso 2: Abrir Swagger UI

Navega a: `https://localhost:5001`

Verás la documentación interactiva con un botón **"Authorize"** 🔒 en la parte superior derecha.

### Paso 3: Obtener un Token JWT

#### Opción A: Usando Swagger UI

1. **Expandir** el endpoint `POST /api/auth/login`
2. **Click** en "Try it out"
3. **Ingresar** las credenciales:
   ```json
   {
     "username": "admin",
     "password": "SampleEx4mF0rT3st!ñ"
   }
   ```
4. **Click** en "Execute"
5. **Copiar** el token de la respuesta

**Respuesta esperada**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "email": "admin@example.com",
  "role": "Admin",
  "expiresAt": "2025-10-14T12:00:00Z"
}
```

#### Opción B: Usando cURL

```bash
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "SampleEx4mF0rT3st!ñ"
  }'
```

#### Opción C: Usando PowerShell

```powershell
$body = @{
    username = "admin"
    password = "SampleEx4mF0rT3st!ñ"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "https://localhost:5001/api/auth/login" `
    -Method Post `
    -Body $body `
    -ContentType "application/json"

$token = $response.token
Write-Host "Token: $token"
```

### Paso 4: Autorizar en Swagger

1. **Click** en el botón **"Authorize"** 🔒 (arriba a la derecha)
2. **Ingresar**: `Bearer {tu-token-aquí}`
   - Ejemplo: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
3. **Click** en "Authorize"
4. **Click** en "Close"

¡Ahora puedes usar todos los endpoints! 🎉

### Paso 5: Usar los Endpoints Protegidos

Ahora todos los endpoints de Categories y Products funcionarán:

#### Ejemplos en Swagger:
- `GET /api/categories` - Obtener todas las categorías
- `GET /api/products` - Obtener productos paginados
- `POST /api/categories` - Crear categoría
- `PUT /api/products/{id}` - Actualizar producto
- `DELETE /api/categories/{id}` - Eliminar categoría

#### Ejemplo con cURL:
```bash
curl -X GET "https://localhost:5001/api/categories" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

#### Ejemplo con PowerShell:
```powershell
$headers = @{
    "Authorization" = "Bearer $token"
}

$categories = Invoke-RestMethod -Uri "https://localhost:5001/api/categories" `
    -Method Get `
    -Headers $headers

$categories | ConvertTo-Json
```

## 📝 Registrar un Nuevo Usuario

### Usando Swagger

1. **Expandir** `POST /api/auth/register`
2. **Click** "Try it out"
3. **Ingresar** datos:
   ```json
   {
     "username": "newuser",
     "email": "newuser@example.com",
     "password": "MySecurePass123!"
   }
   ```
4. **Click** "Execute"

**Respuesta**: Recibirás un token JWT inmediatamente (auto-login)

### Usando PowerShell

```powershell
$body = @{
    username = "newuser"
    email = "newuser@example.com"
    password = "MySecurePass123!"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "https://localhost:5001/api/auth/register" `
    -Method Post `
    -Body $body `
    -ContentType "application/json"

Write-Host "Token: $($response.token)"
```

## 🔄 Flujo Completo de Autenticación

```
1. Usuario → POST /api/auth/login
            ↓
2. API valida credenciales
            ↓
3. API genera JWT Token
            ↓
4. Usuario recibe Token
            ↓
5. Usuario incluye Token en header: Authorization: Bearer {token}
            ↓
6. API valida Token en cada request
            ↓
7. Si válido → Procesa request
   Si inválido → 401 Unauthorized
```

## 🛡️ Seguridad del Token

### Características del JWT

- **Algoritmo**: HMAC-SHA256
- **Duración**: 60 minutos (Development: 120 minutos)
- **Issuer**: CSharpWebExamAPI
- **Audience**: CSharpWebExamClient
- **Claims incluidos**:
  - `sub`: User ID
  - `unique_name`: Username
  - `email`: Email
  - `role`: User role (Admin/User)
  - `jti`: Token ID único

### Validaciones

El token es validado en cada request:
- ✅ Firma válida
- ✅ No expirado
- ✅ Issuer correcto
- ✅ Audience correcto

## ❌ Errores Comunes

### 401 Unauthorized

**Causa**: Token no proporcionado, inválido o expirado

**Solución**:
1. Verificar que el header incluya: `Authorization: Bearer {token}`
2. Verificar que el token no haya expirado (60 min)
3. Obtener un nuevo token con `/api/auth/login`

**Ejemplo de error**:
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

### 400 Bad Request (Login)

**Causa**: Credenciales incorrectas

**Solución**: Verificar username y password

### Token Expirado

**Síntoma**: Endpoints dejan de funcionar después de 60 minutos

**Solución**: Hacer login nuevamente para obtener un nuevo token

## 🔧 Configuración JWT

### appsettings.json

```json
{
  "JwtSettings": {
    "SecretKey": "CSharpWebExam-SuperSecretKey-MinLength32Characters!",
    "Issuer": "CSharpWebExamAPI",
    "Audience": "CSharpWebExamClient",
    "ExpirationMinutes": "60"
  }
}
```

### Cambiar Duración del Token

Editar `appsettings.Development.json`:
```json
{
  "JwtSettings": {
    "ExpirationMinutes": "120"  // 2 horas
  }
}
```

## 📊 Endpoints Disponibles

### 🔓 Públicos (No requieren autenticación)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/auth/register` | Registrar nuevo usuario |
| POST | `/api/auth/login` | Iniciar sesión |

### 🔒 Protegidos (Requieren JWT)

#### Categories

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/categories` | Obtener todas las categorías |
| GET | `/api/categories/{id}` | Obtener categoría por ID |
| POST | `/api/categories` | Crear categoría |
| PUT | `/api/categories/{id}` | Actualizar categoría |
| DELETE | `/api/categories/{id}` | Eliminar categoría |

#### Products

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/products` | Obtener productos (paginado) |
| GET | `/api/products/grouped` | Obtener productos agrupados |
| GET | `/api/products/{id}` | Obtener producto por ID |
| POST | `/api/products` | Crear producto |
| PUT | `/api/products/{id}` | Actualizar producto |
| DELETE | `/api/products/{id}` | Eliminar producto |

## 🧪 Testing con Postman

### 1. Crear Colección

1. Crear nueva colección: "CSharp Web Exam API"
2. Agregar variable: `{{baseUrl}}` = `https://localhost:5001`
3. Agregar variable: `{{token}}` = (se llenará automáticamente)

### 2. Request de Login

```
POST {{baseUrl}}/api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "SampleEx4mF0rT3st!ñ"
}
```

**Test Script** (para guardar token automáticamente):
```javascript
var jsonData = pm.response.json();
pm.collectionVariables.set("token", jsonData.token);
```

### 3. Requests Protegidos

Agregar en el tab "Authorization":
- Type: `Bearer Token`
- Token: `{{token}}`

O en Headers:
```
Authorization: Bearer {{token}}
```

## 🎓 Arquitectura Implementada

### Clean Architecture + Use Cases

```
┌─────────────────────────────────────┐
│   API Layer (Minimal Endpoints)     │ ← JWT Protected
├─────────────────────────────────────┤
│   Use Cases Layer                   │ ← Business Logic
├─────────────────────────────────────┤
│   Application Layer (Services)      │ ← Domain Logic
├─────────────────────────────────────┤
│   Infrastructure (Repositories)     │ ← Data Access
├─────────────────────────────────────┤
│   Domain Layer (Entities)           │ ← Pure Models
└─────────────────────────────────────┘
```

### Use Cases Implementados

**Categories** (5 Use Cases):
- GetAllCategoriesUseCase
- GetCategoryByIdUseCase
- CreateCategoryUseCase
- UpdateCategoryUseCase
- DeleteCategoryUseCase

**Products** (6 Use Cases):
- GetProductsUseCase
- GetProductByIdUseCase
- GetGroupedProductsUseCase
- CreateProductUseCase
- UpdateProductUseCase
- DeleteProductUseCase

## 📱 Integración con UI

### Actualizar ApiClient (ui/Services/ApiClient.cs)

```csharp
private string? _token;

public void SetToken(string token)
{
    _token = token;
}

private HttpClient GetClient()
{
    var client = new HttpClient();
    client.BaseAddress = new Uri(_baseUrl);
    
    if (!string.IsNullOrEmpty(_token))
    {
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _token);
    }
    
    return client;
}
```

### Flujo en UI

1. Usuario ingresa credenciales en Login page
2. UI llama a `/api/auth/login`
3. UI guarda token (Session/Cookie)
4. UI incluye token en todas las llamadas subsecuentes
5. Al expirar, redirigir a Login

## 🔍 Debugging

### Ver Claims del Token

Usar [jwt.io](https://jwt.io) para decodificar el token y ver los claims:

```json
{
  "sub": "1",
  "unique_name": "admin",
  "email": "admin@example.com",
  "role": "Admin",
  "jti": "abc123...",
  "exp": 1697284800,
  "iss": "CSharpWebExamAPI",
  "aud": "CSharpWebExamClient"
}
```

### Logs

Revisar `api/logs/api.log` para ver:
- Login attempts
- Token generation
- Authorization failures

## ✅ Checklist de Implementación

- [x] JWT Authentication configurado
- [x] JWT Authorization configurado
- [x] Swagger con soporte Bearer
- [x] Todos los endpoints protegidos
- [x] AuthEndpoints (Login/Register)
- [x] Use Cases implementados (11 total)
- [x] UserRepository y AuthService
- [x] JwtTokenGenerator
- [x] Usuarios de prueba seeded
- [x] Configuración por ambiente

## 🎉 ¡Listo para Usar!

La API está completamente funcional con:
- ✅ Autenticación JWT
- ✅ Arquitectura Use Cases
- ✅ Clean Architecture
- ✅ Minimal API Endpoints
- ✅ Swagger con Bearer Auth
- ✅ Multi-ambiente (Dev/QA/Prod)

---

**Contraseña de prueba**: `SampleEx4mF0rT3st!ñ`

**Usuarios**: admin, user1, user2

**Swagger**: https://localhost:5001
