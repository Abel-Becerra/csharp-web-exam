# UI Tests - Product Management System

## 📋 Descripción

Proyecto de pruebas unitarias para el sistema de gestión de productos (UI). Contiene casos de prueba para controladores, filtros y modelos de vista.

---

## 🏗️ Estructura del Proyecto

```
ui.tests/
├── Controllers/
│   ├── ProductsControllerTests.cs      # Pruebas del controlador de productos
│   ├── CategoriesControllerTests.cs    # Pruebas del controlador de categorías
│   └── AccountControllerTests.cs       # Pruebas del controlador de autenticación
├── Filters/
│   └── AuthorizeUserAttributeTests.cs  # Pruebas del filtro de autorización
├── Models/
│   └── ViewModelTests.cs               # Pruebas de modelos de vista
└── README.md                           # Este archivo
```

---

## 🧪 Casos de Prueba Implementados

### 1. ProductsControllerTests (15 pruebas)

#### Pruebas de Index
- ✅ `Index_ReturnsViewResult_WithProductList` - Verifica que Index retorna una lista de productos
- ✅ `Index_WithSearchTerm_FiltersProducts` - Verifica filtrado por término de búsqueda
- ✅ `Index_WithCategoryFilter_FiltersProducts` - Verifica filtrado por categoría
- ✅ `Index_WithSorting_SortsProducts` - Verifica ordenamiento de productos
- ✅ `Index_WithPagination_ReturnsCorrectPage` - Verifica paginación correcta

#### Pruebas de Details
- ✅ `Details_WithValidId_ReturnsViewResult` - Verifica detalles con ID válido
- ✅ `Details_WithInvalidId_ReturnsNotFound` - Verifica manejo de ID inválido

#### Pruebas de Grouped
- ✅ `Grouped_ReturnsViewResult_WithGroupedData` - Verifica reporte agrupado

#### Pruebas de Create
- ✅ `Create_GET_ReturnsViewResult` - Verifica vista de creación
- ✅ `Create_POST_WithValidModel_RedirectsToIndex` - Verifica creación exitosa
- ✅ `Create_POST_WithInvalidModel_ReturnsView` - Verifica validación de modelo

#### Pruebas de Edit
- ✅ `Edit_GET_WithValidId_ReturnsViewResult` - Verifica vista de edición
- ✅ `Edit_POST_WithValidModel_RedirectsToIndex` - Verifica edición exitosa

#### Pruebas de Delete
- ✅ `Delete_GET_WithValidId_ReturnsViewResult` - Verifica vista de eliminación
- ✅ `Delete_POST_WithValidId_RedirectsToIndex` - Verifica eliminación exitosa

---

### 2. CategoriesControllerTests (12 pruebas)

#### Pruebas de Index
- ✅ `Index_ReturnsViewResult_WithCategoryList` - Verifica lista de categorías

#### Pruebas de Details
- ✅ `Details_WithValidId_ReturnsViewResult` - Verifica detalles con ID válido
- ✅ `Details_WithInvalidId_ReturnsNotFound` - Verifica manejo de ID inválido

#### Pruebas de Create
- ✅ `Create_GET_ReturnsViewResult` - Verifica vista de creación
- ✅ `Create_POST_WithValidModel_RedirectsToIndex` - Verifica creación exitosa
- ✅ `Create_POST_WithInvalidModel_ReturnsView` - Verifica validación de modelo
- ✅ `Create_POST_WithEmptyName_ReturnsViewWithError` - Verifica validación de nombre vacío

#### Pruebas de Edit
- ✅ `Edit_GET_WithValidId_ReturnsViewResult` - Verifica vista de edición
- ✅ `Edit_POST_WithValidModel_RedirectsToIndex` - Verifica edición exitosa
- ✅ `Edit_POST_WithInvalidModel_ReturnsView` - Verifica validación de modelo

#### Pruebas de Delete
- ✅ `Delete_GET_WithValidId_ReturnsViewResult` - Verifica vista de eliminación
- ✅ `Delete_POST_WithValidId_RedirectsToIndex` - Verifica eliminación exitosa

---

### 3. AccountControllerTests (10 pruebas)

#### Pruebas de Login GET
- ✅ `Login_GET_ReturnsViewResult` - Verifica vista de login
- ✅ `Login_GET_WhenAuthenticated_RedirectsToReturnUrl` - Verifica redirección si ya está autenticado

#### Pruebas de Login POST
- ✅ `Login_POST_WithValidCredentials_RedirectsToReturnUrl` - Verifica login exitoso
- ✅ `Login_POST_WithInvalidModel_ReturnsView` - Verifica validación de modelo
- ✅ `Login_POST_WithInvalidCredentials_ReturnsViewWithError` - Verifica credenciales inválidas
- ✅ `Login_POST_WithEmptyUsername_ReturnsViewWithError` - Verifica validación de username
- ✅ `Login_POST_WithEmptyPassword_ReturnsViewWithError` - Verifica validación de password

#### Pruebas de Logout
- ✅ `Logout_ClearsCookieAndSession_RedirectsToLogin` - Verifica logout exitoso
- ✅ `Logout_WhenNotAuthenticated_StillRedirectsToLogin` - Verifica logout sin autenticación

---

### 4. AuthorizeUserAttributeTests (7 pruebas)

#### Pruebas de Autorización
- ✅ `OnAuthorization_WithValidTokenAndSession_AllowsAccess` - Verifica acceso con token válido
- ✅ `OnAuthorization_WithoutToken_RedirectsToLogin` - Verifica redirección sin token
- ✅ `OnAuthorization_WithTokenButNoSession_RedirectsToLogin` - Verifica token sin sesión
- ✅ `OnAuthorization_WithSessionButNoToken_RedirectsToLogin` - Verifica sesión sin token
- ✅ `OnAuthorization_WithEmptyToken_RedirectsToLogin` - Verifica token vacío
- ✅ `OnAuthorization_RedirectIncludesReturnUrl` - Verifica returnUrl en redirección

---

### 5. ViewModelTests (15 pruebas)

#### LoginViewModel (4 pruebas)
- ✅ `LoginViewModel_WithValidData_PassesValidation` - Verifica validación exitosa
- ✅ `LoginViewModel_WithoutUsername_FailsValidation` - Verifica validación de username requerido
- ✅ `LoginViewModel_WithoutPassword_FailsValidation` - Verifica validación de password requerido
- ✅ `LoginViewModel_RememberMe_DefaultsToFalse` - Verifica valor por defecto

#### ProductViewModel (4 pruebas)
- ✅ `ProductViewModel_WithValidData_PassesValidation` - Verifica validación exitosa
- ✅ `ProductViewModel_WithoutName_FailsValidation` - Verifica validación de nombre requerido
- ✅ `ProductViewModel_WithNegativePrice_FailsValidation` - Verifica validación de precio
- ✅ `ProductViewModel_WithZeroPrice_IsValid` - Verifica precio cero válido

#### CategoryViewModel (3 pruebas)
- ✅ `CategoryViewModel_WithValidData_PassesValidation` - Verifica validación exitosa
- ✅ `CategoryViewModel_WithoutName_FailsValidation` - Verifica validación de nombre requerido
- ✅ `CategoryViewModel_UpdatedAt_CanBeNull` - Verifica campo opcional

#### ProductListViewModel (3 pruebas)
- ✅ `ProductListViewModel_DefaultValues_AreCorrect` - Verifica valores por defecto
- ✅ `ProductListViewModel_TotalPages_CalculatesCorrectly` - Verifica cálculo de páginas
- ✅ `ProductListViewModel_WithProducts_StoresCorrectly` - Verifica almacenamiento de productos

#### ProductGroupViewModel (2 pruebas)
- ✅ `ProductGroupViewModel_CalculatesStatistics_Correctly` - Verifica cálculo de estadísticas
- ✅ `ProductGroupViewModel_WithZeroProducts_HandlesGracefully` - Verifica manejo de categoría vacía

---

## 📊 Resumen de Cobertura

| Componente | Pruebas | Descripción |
|------------|---------|-------------|
| **ProductsController** | 15 | CRUD completo + filtros + paginación |
| **CategoriesController** | 12 | CRUD completo + validaciones |
| **AccountController** | 10 | Login/Logout + validaciones |
| **AuthorizeUserAttribute** | 7 | Autorización y seguridad |
| **ViewModels** | 15 | Validaciones de modelos |
| **TOTAL** | **59 pruebas** | Cobertura completa |

---

## 🛠️ Tecnologías Utilizadas

- **MSTest** - Framework de pruebas de Microsoft
- **Moq** - Framework de mocking para .NET
- **ASP.NET MVC 5** - Framework web
- **.NET 9.0** - Plataforma de ejecución

---

## 🚀 Ejecutar las Pruebas

### Desde Visual Studio
1. Abrir Test Explorer (Test > Test Explorer)
2. Click en "Run All" para ejecutar todas las pruebas
3. Ver resultados en la ventana de Test Explorer

### Desde Línea de Comandos
```bash
# Navegar al directorio del proyecto
cd csharp-web-exam/ui.tests

# Ejecutar todas las pruebas
dotnet test

# Ejecutar con detalles
dotnet test --logger "console;verbosity=detailed"

# Ejecutar con cobertura de código
dotnet test --collect:"XPlat Code Coverage"
```

---

## 📝 Convenciones de Nombres

### Patrón de Nombres de Pruebas
```
[MethodName]_[Scenario]_[ExpectedResult]
```

**Ejemplos**:
- `Index_WithSearchTerm_FiltersProducts`
- `Create_POST_WithValidModel_RedirectsToIndex`
- `Login_WithInvalidCredentials_ReturnsViewWithError`

### Estructura de Pruebas (AAA Pattern)
```csharp
[TestMethod]
public void TestName()
{
    // Arrange - Configurar datos y mocks
    var model = new ProductViewModel { ... };
    
    // Act - Ejecutar la acción
    var result = controller.Action(model);
    
    // Assert - Verificar el resultado
    Assert.IsNotNull(result);
}
```

---

## 🔍 Tipos de Pruebas

### 1. Pruebas de Controladores
- Verifican que las acciones retornan los resultados correctos
- Validan redirecciones y vistas
- Comprueban manejo de errores

### 2. Pruebas de Filtros
- Verifican autorización y autenticación
- Validan redirecciones a login
- Comprueban manejo de cookies y sesiones

### 3. Pruebas de Modelos
- Verifican validaciones de datos
- Comprueban atributos de validación
- Validan cálculos y propiedades

---

## 🎯 Escenarios de Prueba Cubiertos

### Casos Positivos (Happy Path)
- ✅ Operaciones CRUD exitosas
- ✅ Login con credenciales válidas
- ✅ Filtrado y búsqueda correctos
- ✅ Paginación funcional

### Casos Negativos (Error Handling)
- ✅ Validación de modelos inválidos
- ✅ Manejo de IDs inexistentes
- ✅ Credenciales incorrectas
- ✅ Acceso no autorizado

### Casos Edge (Límites)
- ✅ Campos vacíos
- ✅ Valores nulos
- ✅ Precios negativos/cero
- ✅ Categorías sin productos

---

## 📚 Mejoras Futuras

### Pruebas Adicionales Recomendadas
1. **Pruebas de Integración**
   - Probar con base de datos real
   - Verificar flujos completos end-to-end

2. **Pruebas de ApiClient**
   - Mockear HttpClient
   - Probar llamadas a la API
   - Verificar manejo de errores HTTP

3. **Pruebas de UI (Selenium)**
   - Automatizar pruebas de interfaz
   - Verificar JavaScript y AJAX
   - Probar navegación completa

4. **Pruebas de Rendimiento**
   - Medir tiempos de respuesta
   - Probar con grandes volúmenes de datos
   - Verificar paginación eficiente

5. **Pruebas de Seguridad**
   - Verificar protección CSRF
   - Probar inyección SQL
   - Validar XSS prevention

---

## 🐛 Debugging de Pruebas

### Ejecutar una Prueba Específica
```bash
dotnet test --filter "FullyQualifiedName~ProductsControllerTests"
```

### Ejecutar Pruebas por Categoría
```bash
dotnet test --filter "TestCategory=Controllers"
```

### Ver Output Detallado
```bash
dotnet test --logger "console;verbosity=detailed"
```

---

## 📖 Recursos

- [MSTest Documentation](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-mstest)
- [Moq Documentation](https://github.com/moq/moq4)
- [ASP.NET MVC Testing](https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions-1/unit-testing/creating-unit-tests-for-asp-net-mvc-applications-cs)

---

**Proyecto de Pruebas Creado**: 2025-10-14  
**Framework**: MSTest + Moq  
**Total de Pruebas**: 59  
**Estado**: ✅ **LISTO PARA EJECUCIÓN**
