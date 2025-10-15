# Schema SQL - Actualización Completada

## ✅ Cambios Realizados

### 1. Tabla Users Agregada

```sql
CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    Email TEXT NOT NULL,
    Role TEXT NOT NULL DEFAULT 'User',
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT
);
```

**Características**:
- ✅ Username único (UNIQUE constraint)
- ✅ PasswordHash para seguridad (SHA256)
- ✅ Email para notificaciones
- ✅ Role para autorización (Admin/User)
- ✅ Timestamps de auditoría

### 2. Índices Agregados

```sql
CREATE INDEX IF NOT EXISTS idx_users_username ON Users(Username);
CREATE INDEX IF NOT EXISTS idx_users_email ON Users(Email);
CREATE INDEX IF NOT EXISTS idx_categories_name ON Categories(Name);
```

**Beneficios**:
- ✅ Búsqueda rápida de usuarios por username
- ✅ Búsqueda rápida por email
- ✅ Búsqueda optimizada de categorías

### 3. Seed Data de Usuarios

```sql
INSERT INTO Users (Username, PasswordHash, Email, Role, CreatedAt) VALUES 
('admin', 'YourHashHere', 'admin@example.com', 'Admin', datetime('now')),
('user1', 'YourHashHere', 'user1@example.com', 'User', datetime('now')),
('user2', 'YourHashHere', 'user2@example.com', 'User', datetime('now'));
```

**Nota**: El hash real se genera en `DbInitializer.cs` usando SHA256.

### 4. Documentación Mejorada

Se agregaron secciones de documentación:
- ✅ **NOTES**: Información general del schema
- ✅ **AUTHENTICATION**: Cómo autenticarse
- ✅ **DATABASE STRUCTURE**: Estructura visual de tablas

## 📊 Estructura Completa de la Base de Datos

```
┌─────────────────────────────────────────┐
│              Users Table                │
├─────────────────────────────────────────┤
│ Id (PK)                                 │
│ Username (UNIQUE)                       │
│ PasswordHash                            │
│ Email                                   │
│ Role (Admin/User)                       │
│ CreatedAt                               │
│ UpdatedAt                               │
└─────────────────────────────────────────┘

┌─────────────────────────────────────────┐
│           Categories Table              │
├─────────────────────────────────────────┤
│ Id (PK)                                 │
│ Name                                    │
│ CreatedAt                               │
│ UpdatedAt                               │
└─────────────────────────────────────────┘
                    ↑
                    │ FK
                    │
┌─────────────────────────────────────────┐
│            Products Table               │
├─────────────────────────────────────────┤
│ Id (PK)                                 │
│ Name                                    │
│ Price                                   │
│ CategoryId (FK) → Categories.Id         │
│ CreatedAt                               │
│ UpdatedAt                               │
└─────────────────────────────────────────┘
```

## 🔍 Índices Creados

| Tabla | Columna | Índice | Propósito |
|-------|---------|--------|-----------|
| Users | Username | idx_users_username | Login rápido |
| Users | Email | idx_users_email | Búsqueda por email |
| Categories | Name | idx_categories_name | Búsqueda de categorías |
| Products | CategoryId | idx_products_categoryid | Joins rápidos |
| Products | Name | idx_products_name | Búsqueda de productos |

## 📝 Datos de Prueba

### Usuarios (3)
| Username | Password | Role | Email |
|----------|----------|------|-------|
| admin | SampleEx4mF0rT3st!ñ | Admin | admin@example.com |
| user1 | SampleEx4mF0rT3st!ñ | User | user1@example.com |
| user2 | SampleEx4mF0rT3st!ñ | User | user2@example.com |

### Categorías (5)
- Electronics
- Books
- Clothing
- Home & Garden
- Sports

### Productos (15)
- 3 productos por categoría
- Precios variados ($9.99 - $999.99)

## 🔐 Seguridad

### Password Hashing
```csharp
// En DbInitializer.cs
var passwordHash = Convert.ToBase64String(
    System.Security.Cryptography.SHA256.HashData(
        System.Text.Encoding.UTF8.GetBytes("SampleEx4mF0rT3st!ñ")
    )
);
```

### Constraints
- ✅ Username UNIQUE - Previene duplicados
- ✅ NOT NULL en campos críticos
- ✅ Foreign Key con CASCADE DELETE
- ✅ Role con valor por defecto 'User'

## 📍 Ubicación del Schema

```
csharp-web-exam/
└── database/
    └── schema.sql  ← Actualizado
```

## ⚠️ Importante

1. **Este archivo es de referencia**: La base de datos real se crea automáticamente por `DbInitializer.cs`

2. **No ejecutar manualmente**: El schema se aplica automáticamente al iniciar la API

3. **Para resetear la DB**: Eliminar el archivo `.db` en `api/app_data/` y reiniciar la API

4. **Ubicación de la DB**:
   - Development: `api/app_data/app_dev.db`
   - QA: `api/app_data/app_qa.db`
   - Production: `api/app_data/app.db`

## 🔄 Sincronización con DbInitializer

El schema SQL está sincronizado con `DbInitializer.cs`:

| Aspecto | schema.sql | DbInitializer.cs |
|---------|------------|------------------|
| Tabla Users | ✅ | ✅ |
| Tabla Categories | ✅ | ✅ |
| Tabla Products | ✅ | ✅ |
| Índices | ✅ | ✅ |
| Seed Users | ✅ | ✅ |
| Seed Categories | ✅ | ✅ |
| Seed Products | ✅ | ✅ |

## 🎯 Uso del Schema

### Para Referencia
Consultar `database/schema.sql` para ver la estructura completa de la base de datos.

### Para Documentación
El schema incluye comentarios detallados sobre:
- Estructura de tablas
- Relaciones
- Índices
- Datos de prueba
- Autenticación JWT

### Para Testing Manual
Si necesitas crear una DB manualmente para testing:

```bash
sqlite3 test.db < database/schema.sql
```

## ✅ Checklist de Actualización

- [x] Tabla Users agregada
- [x] Índices de Users creados
- [x] Índice de Categories agregado
- [x] Seed data de Users incluido
- [x] Documentación de autenticación
- [x] Notas de uso agregadas
- [x] Estructura visual documentada
- [x] Sincronizado con DbInitializer.cs

---

**Estado**: ✅ **Schema Actualizado y Documentado**

**Archivo**: `database/schema.sql`

**Última actualización**: 2025-10-14
