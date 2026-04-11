# Documentación de Cambios - Sprint 1 (Domain Layer)

## Tareas Completadas

### 1.1 Creación del Value Object `Moneda`
- Ubicación: `GalponERP.Domain/ValueObjects/Moneda.cs`
- Descripción: Encapsula el monto en Bolivianos (decimal) y asegura que no haya errores de redondeo mediante `Math.Round` con `MidpointRounding.AwayFromZero` a 2 decimales. Incluye operadores matemáticos básicos.

### 1.2 Entidades de Dominio
- **Base Primitives**: Se creó `Entity.cs` para manejar la identidad (`Guid Id`) y el Soft Delete (`IsActive`).
- **Galpon**: Representa el galpón físico con validación de capacidad.
- **Usuario**: Vinculado con FirebaseUid y gestión de roles.
- **Lote**: Entidad central que maneja la cantidad de pollos vivos y permite `RegistrarBajas(int cantidad)`. Incluye validaciones de estado.
- **Producto**: Define insumos (alimento, vacunas) con tipos y unidades de medida.
- **MovimientoInventario**: Registra entradas y salidas de productos vinculados o no a un lote.
- **MortalidadDiaria**: Histórico detallado de bajas por fecha.

### 1.3 Excepciones de Dominio
- Ubicación: `GalponERP.Domain/Exceptions/`
- Se implementaron:
  - `DomainException`: Base para todas las excepciones del dominio.
  - `LoteDomainException`: Errores específicos de lógica de lotes.
  - `InventarioDomainException`: Errores específicos de inventario.

### 1.4 Interfaces de Repositorio
- Ubicación: `GalponERP.Domain/Interfaces/Repositories/`
- Se definieron `ILoteRepository` e `IInventarioRepository` con métodos asíncronos básicos para persistencia.

# Documentación de Cambios - Sprint 2 (Infrastructure Layer)

## Tareas Completadas

### 2.1 Configuración de Entity Framework Core y DbContext
- **Paquetes NuGet**: Se instalaron `Npgsql.EntityFrameworkCore.PostgreSQL` y `Microsoft.EntityFrameworkCore.Design` en los proyectos `GalponERP.Infrastructure` y `GalponERP.Api`.
- **DbContext**: Se creó `GalponDbContext.cs` en `GalponERP.Infrastructure/Persistence/`.
  - Hereda de `DbContext`.
  - Incluye `DbSet` para todas las entidades del dominio (`Galpon`, `Lote`, `MortalidadDiaria`, `Producto`, `MovimientoInventario`, `Usuario`).
  - Configurado para cargar automáticamente las configuraciones de Fluent API desde el mismo ensamblado.

### 2.2 Implementación de Fluent API y Soft Delete Global
- **Configuraciones**: Se crearon clases de configuración en `GalponERP.Infrastructure/Persistence/Configurations/` para cada entidad.
- **Fluent API**:
  - Se definieron nombres de tablas, llaves primarias, longitudes máximas y requerimientos.
  - Se configuraron conversiones para Enums (guardados como string en la BD).
  - Se utilizó `ComplexProperty` para el Value Object `Moneda` (disponible en EF Core 10).
  - Se establecieron relaciones y comportamientos de borrado (Restrict/SetNull).
- **Soft Delete**: Se aplicó un filtro global `HasQueryFilter(e => e.IsActive)` en todas las entidades para asegurar que los registros desactivados no sean recuperados en consultas estándar.

### 2.3 Repositorios Concretos
- **LoteRepository**: Implementación de `ILoteRepository` para la gestión de lotes, incluyendo filtrado por estado activo y operaciones básicas de persistencia.
- **InventarioRepository**: Implementación de `IInventarioRepository` para el seguimiento de movimientos de inventario por lote o producto, ordenados por fecha.

### 2.4 Servicio de Autenticación (Firebase)
- **Interfaz IAuthenticationService**: Creada en `GalponERP.Application/Interfaces/` para desacoplar la lógica de autenticación de la infraestructura.
- **FirebaseAuthService**: Implementación en `GalponERP.Infrastructure/Authentication/`.
  - Utiliza `IHttpContextAccessor` para acceder a las claims del usuario autenticado.
  - Extrae el Firebase UID (NameIdentifier o claim 'user_id') y el Email.
  - Proporciona un método para verificar si el usuario está autenticado.

### 2.5 Inyección de Dependencias
- **DependencyInjection.cs**: Creado en la raíz de `GalponERP.Infrastructure`.
  - Proporciona el método de extensión `AddInfrastructure`.
  - Registra el `GalponDbContext` configurado para usar PostgreSQL con la cadena de conexión `DefaultConnection`.
  - Registra los repositorios (`LoteRepository`, `InventarioRepository`) con ciclo de vida Scoped.
  - Registra el servicio de autenticación y configura el acceso al `HttpContext`.

# Documentación de Cambios - Sprint 3 (Application Layer)

## Tareas Completadas

### 3.1 Configuración de MediatR y FluentValidation
- **Paquetes NuGet**: Se instalaron `MediatR` y `FluentValidation.DependencyInjectionExtensions` en `GalponERP.Application`.
- **DependencyInjection.cs**: Creado en `GalponERP.Application`.
  - Registra MediatR escaneando el ensamblado de la capa de aplicación.
  - Registra automáticamente todos los validadores de FluentValidation definidos en el mismo ensamblado.
- **Registro Global**: Se actualizaron los servicios en `GalponERP.Api/Program.cs` para incluir las llamadas a `AddApplication()` y `AddInfrastructure()`, asegurando la correcta orquestación de todas las capas.

# Documentación de Cambios - Sprint 4 (API Layer)

## Tareas Completadas

### 4.1 Configuración de Global Exception Handler
- **Middleware**: Se implementó `GlobalExceptionHandler.cs` en `GalponERP.Api/Middleware/` utilizando la interfaz `IExceptionHandler` de .NET 8+.
- **Manejo de Excepciones**:
  - `DomainException` (y sus derivadas): Retorna `400 Bad Request`.
  - `ValidationException`: Retorna `422 Unprocessable Entity` con un diccionario de errores detallado.
  - Otras excepciones: Retornan `500 Internal Server Error`.
- **ValidationException**: Se creó en `GalponERP.Application/Exceptions/` para capturar y formatear errores de `FluentValidation`.
- **Configuración**: Se registró el manejador en `Program.cs` mediante `builder.Services.AddExceptionHandler<GlobalExceptionHandler>()` y se activó con `app.UseExceptionHandler()`. Se habilitó `AddProblemDetails()` para estandarizar las respuestas de error siguiendo el RFC 7807.

# Documentación de Cambios - Sprint 5 (Motor Financiero y Costos)

## Tareas Completadas

### 5.1 Entidad `GastoOperativo` e Interfaz de Repositorio
- **GastoOperativo**: Se creó la entidad en `GalponERP.Domain/Entities/GastoOperativo.cs`. Permite registrar gastos de luz, agua, sueldos, etc., vinculados a un galpón y opcionalmente a un lote.
- **IGastoOperativoRepository**: Se definió la interfaz en `GalponERP.Domain/Interfaces/Repositories/IGastoOperativoRepository.cs` con métodos para obtener gastos por galpón y por lote.

### 5.2 Servicio de Dominio `CalculadoraCostosLote`
- **Carpeta Services**: Se creó `GalponERP.Domain/Services/` para albergar la lógica de negocio compleja que no pertenece a una sola entidad.
- **CalculadoraCostosLote**: Implementa métodos puros para:
  - `CalcularFCR`: Cálculo del Índice de Conversión Alimenticia redondeado a 2 decimales.
  - `CalcularCostoTotal`: Suma de amortización, costo de pollitos, alimento y gastos operativos.
- **Mejora en `Moneda`**: Se agregaron operadores de multiplicación (`*`) al Value Object `Moneda` para facilitar el cálculo de costos (ej. Costo Unitario * Cantidad).

# Documentación de Cambios - Sprint 6 (Módulo de Ventas y Cierre de Lote)

## Tareas Completadas

### 6.1 Actualización de `Lote` y Nuevas Entidades
- **Lote**: Se agregaron las propiedades `MortalidadAcumulada` y `PollosVendidos`. Se implementó el método `RegistrarVenta(int cantidad)` con validación crítica: no se puede vender más pollos de los disponibles según `CantidadInicial - MortalidadAcumulada - PollosVendidos`.
- **Estado Lote**: Se renombró el estado `Finalizado` a `Cerrado` y se implementó el método `CerrarLote()`.
- **Cliente**: Nueva entidad para gestionar los compradores.
- **Venta**: Nueva entidad que registra la venta de pollos vinculada a un lote y un cliente, calculando el total automáticamente.
- **Repositorios**: Se crearon las interfaces `IClienteRepository` y `IVentaRepository`.

### 6.2 Infraestructura y Persistencia
- **Configuraciones EF Core**: Se implementaron `ClienteConfiguration` y `VentaConfiguration`. Se actualizó `LoteConfiguration` para mapear las nuevas propiedades.
- **Repositorios Concretos**: Implementación de `ClienteRepository` y `VentaRepository`. 
- **GastoOperativoRepository**: Se implementó este repositorio que estaba pendiente del Sprint 5.
- **GalponDbContext**: Se registraron los nuevos `DbSet` para `Cliente`, `Venta` y `GastoOperativo`.
- **IUnitOfWork**: Se implementó el patrón Unit of Work para asegurar la atomicidad de las transacciones que involucran múltiples repositorios.

### 6.3 Caso de Uso: Registrar Venta Parcial
- **Comando**: `RegistrarVentaParcialCommand` recibe el ID del lote, cliente, cantidad y precio.
- **Lógica**: El handler valida la existencia de las entidades, ejecuta `lote.RegistrarVenta()` (donde reside la validación de negocio) y persiste tanto la nueva `Venta` como el estado actualizado del `Lote` mediante el `UnitOfWork`.

### 6.4 Caso de Uso: Cierre de Lote
- **Comando**: `CerrarLoteCommand` orquesta el cierre financiero del lote.
- **Cálculo de Utilidad**:
  - Suma el `TotalVenta` de todas las ventas del lote.
  - Obtiene el costo de adquisición de pollitos (`CostoUnitarioPollito * CantidadInicial`).
  - Obtiene los gastos operativos asociados.
  - Utiliza el servicio `CalculadoraCostosLote` para determinar el Costo Total de producción.
  - Calcula la `UtilidadNeta = Ingresos - Costos`.
- **Cierre**: Cambia el estado del lote a `Cerrado`. Un lote cerrado bloquea futuras operaciones de venta o bajas.
- **Resultado**: Retorna un DTO `CerrarLoteResponse` con el resumen financiero.

# Documentación de Cambios - Sprint 7 (Background Jobs y Testing)

## Tareas Completadas

### 7.1 Servicio de Notificaciones (Push)
- **Interfaz INotificationService**: Definida en `GalponERP.Application/Interfaces/` con el método `EnviarAlertaPushAsync`.
- **FirebaseNotificationService**: Implementación en `GalponERP.Infrastructure/Notifications/` utilizando el SDK `FirebaseAdmin`.
- **Registro**: El servicio se registró en la inyección de dependencias como Scoped.

### 7.2 Verificación de Niveles de Alimento
- **Caso de Uso**: `VerificarNivelesAlimentoQuery` en `GalponERP.Application/Inventario/Queries/VerificarNivelesAlimento/`.
- **Lógica de Alerta**: 
  - Calcula el `StockActualAlimento` sumando todos los movimientos de tipo `Alimento`.
  - Calcula el `ConsumoDiarioGlobal` promediando las salidas de todos los lotes activos desde su fecha de ingreso.
  - Determina los `DiasRestantes = Stock / ConsumoDiario`.
  - Dispara `RequiereAlerta = true` si los días restantes son menores a 3.

### 7.3 Background Job (Worker)
- **AlertaInventarioJob**: Implementado como `BackgroundService` en `GalponERP.Api/BackgroundJobs/`.
- **Ejecución**: Se ejecuta cada 24 horas. Utiliza `IServiceScopeFactory` para crear un scope, resolver MediatR y ejecutar la verificación de inventario.
- **Notificación**: Si se detecta nivel crítico, busca a todos los usuarios con rol `Admin` mediante `IUsuarioRepository` y envía una notificación push individual.

### 7.4 Testing Unitario
- **Proyecto GalponERP.Tests**: Creado usando xUnit.
- **LoteTests**: Valida el ciclo de vida del lote, incluyendo restricciones de bajas y ventas en lotes cerrados, y validación de inventario disponible.
- **CalculadoraCostosLoteTests**: Verifica la precisión matemática del cálculo del FCR y la suma de costos totales.
- **Resultado**: 11 pruebas unitarias ejecutadas y aprobadas exitosamente.

## Decisiones de Diseño
- **Inyección de Dependencias en BackgroundService**: Se respetó la recomendación de usar `IServiceScopeFactory` para manejar servicios Scoped dentro de un servicio Singleton (Job).
- **Consumo Global**: Para la alerta se optó por un cálculo de consumo diario global de la granja, lo cual es más preciso para determinar la duración del inventario compartido.
- **Repositorio de Productos y Usuarios**: Se crearon `IProductoRepository` e `IUsuarioRepository` para desacoplar el acceso a datos necesario para el Job de Alertas.

# Documentación de Cambios - Sprint 8 (Planificación e Inteligencia)

## Tareas Completadas

### 8.1 Entidad `CalendarioSanitario` y Repositorio
- **CalendarioSanitario**: Nueva entidad para gestionar el cronograma de vacunas y tratamientos por lote.
- **ICalendarioSanitarioRepository**: Interfaz definida en el dominio e implementada en la infraestructura.
- **CalendarioDomainException**: Excepción específica para reglas de negocio del calendario.

### 8.2 Servicio de Dominio `SimuladorProyeccionLote`
- Implementación de un servicio "puro" (sin dependencias de BD) para proyectar:
  - Consumo de alimento fraccionado por etapas (Inicio 20%, Crecimiento 35%, Engorde 45%).
  - Utilidad bruta basada en parámetros "What-If".
  - Peso total esperado y costos de alimentación.

### 8.3 Infraestructura y Persistencia
- **Fluent API**: Configuración de `CalendarioSanitarioConfiguration` con relación uno-a-muchos con `Lote`.
- **Mapeo de Enums**: El estado del calendario se guarda como string (`Pendiente`, `Aplicado`).
- **Migración**: Generada exitosamente (`AddCalendarioSanitario`).
- **Corrección**: Se implementó `GastoOperativoConfiguration` que faltaba en la base de código anterior y causaba errores de mapeo con el Value Object `Moneda`.
- **Dependency Injection**: Se registraron los nuevos repositorios y servicios de dominio.

### 8.4 Generación Automática de Calendario (Principio Abierto/Cerrado)
- **Caso de Uso**: Se creó `CrearLoteCommand` y su Handler.
- **Automatización**: Al crear un nuevo lote, el handler genera automáticamente los registros base de sanidad:
  - Día 7: Vacuna Newcastle.
  - Día 14: Vacuna Gumboro.
- **Atomicidad**: Se utiliza el mismo `UnitOfWork` para persistir el Lote y su Calendario en una sola transacción.

### 8.5 API de Planificación
- **Controlador**: `PlanificacionController` expone dos endpoints críticos:
  - `GET /api/planificacion/simulacion`: Permite realizar proyecciones financieras rápidas.
  - `POST /api/planificacion/lote`: Orquesta la creación de lotes con su calendario automático.

## Decisiones de Diseño
- **Servicios Puros**: El simulador es agnóstico a la persistencia para facilitar pruebas y uso en escenarios hipotéticos.
- **FrameworkReference**: Se añadió `Microsoft.AspNetCore.App` en la capa de infraestructura para soportar correctamente `AddHttpContextAccessor` en .NET 10.
- **ComplexProperty**: Se mantuvo el uso de `ComplexProperty` para el Value Object `Moneda` asegurando consistencia en todas las entidades.

# Configuración de Seguridad y Despliegue (Setup Crítico)

## Base de Datos PostgreSQL
- Se configuró la cadena de conexión `DefaultConnection` en `appsettings.Development.json`.
- El sistema está listo para ejecutar `dotnet ef database update` una vez que las credenciales locales sean validadas por el usuario.
- Se corrigieron problemas de mapeo en las configuraciones de EF Core para asegurar compatibilidad total con PostgreSQL.

## Seguridad de Credenciales (Firebase Admin)
- **Protección**: Se añadió `firebase-admin.json` al archivo `.gitignore` para prevenir fugas accidentales al repositorio.
- **Inicialización**: Se implementó el código de arranque en `Program.cs` para cargar el SDK de Firebase Admin usando las credenciales del archivo JSON.
- **Compilación**: Verificada la compatibilidad del paquete `FirebaseAdmin` con .NET 10 y el flujo de inyección de dependencias.

# BITÁCORA DE ARQUITECTURA - GALPON ERP

## SPRINT 9: Exposición de API y Seguridad
Se ha habilitado la capa de presentación (API) para interactuar con los casos de uso definidos en la capa de Aplicación.

### Cambios Realizados:
- **Limpieza:** Se eliminaron los controladores y clases de ejemplo `WeatherForecast`.
- **Controladores:**
  - `LotesController`: Expone la creación y cierre de lotes.
  - `InventarioController`: Expone la verificación de niveles de alimento para alertas tempranas.
  - `VentasController`: Expone el registro de ventas parciales.
  - `PlanificacionController`: Se actualizó para incluir seguridad y se movió la lógica de lotes a su propio controlador.
- **Seguridad:**
  - Se instaló `Microsoft.AspNetCore.Authentication.JwtBearer`.
  - Se configuró la autenticación JWT contra Firebase Auth. El `ProjectId` se obtiene de la configuración con un fallback seguro.
  - Se habilitó el botón **'Authorize'** en Swagger para permitir pruebas con tokens Bearer.
  - Se añadió el middleware `UseAuthentication` al pipeline de ASP.NET Core.
- **Documentación:**
  - Se creó `docs/endpoints.md` detallando el contrato de la API para todos los endpoints expuestos.

### Decisiones de Diseño:
- **CQRS:** Se mantiene el uso de `IMediator` en los controladores para desacoplar la API de la lógica de negocio.
- **Seguridad por defecto:** Se aplicó el atributo `[Authorize]` a nivel de clase en todos los controladores para garantizar que ningún endpoint sea público por error (excepto si se decide lo contrario en el futuro).
- **Consistencia de Rutas:** Se utiliza el prefijo `api/` y el nombre del controlador en minúsculas/plural según sea conveniente.

## SPRINT 9.3: Seeding Automático y Gestión Total
Se ha automatizado la creación del administrador inicial y se han completado los CRUDs de usuarios y galpones.

### Cambios Realizados:
- **Seeding Automático (Program.cs):**
  - Se implementó un bloque de inicialización que verifica la existencia del usuario "Admin Maestro" con el Firebase UID `utq0GMrQZESnNsyQWUEFOV5fKf23`.
  - Si no existe, se crea automáticamente en la base de datos local para asegurar que el primer acceso tenga permisos de administración.
- **Gestión de Usuarios:**
  - Se añadieron `ActualizarUsuarioCommand` (edición de nombre y rol) y `EliminarUsuarioCommand` (Soft Delete mediante la propiedad `IsActive` de la entidad base).
  - Se expusieron los endpoints `PUT /api/usuarios/{id}` y `DELETE /api/usuarios/{id}`.
- **Gestión de Galpones:**
  - Se creó la entidad `Galpon` con soporte para CRUD completo.
  - Se implementaron los casos de uso: `CrearGalponCommand`, `ListarGalponesQuery` y `EditarGalponCommand`.
  - Se creó el controlador `GalponesController` con todos los métodos protegidos por `[Authorize]`.
- **Arquitectura:**
  - Se integró `IGalponRepository` siguiendo el patrón de Repositorio y Unidad de Trabajo (Unit of Work).
  - Se actualizaron las entidades para incluir métodos de actualización de estado siguiendo los principios de DDD.

# BITÁCORA DE ARQUITECTURA - GALPON ERP

## SPRINT 10: Inicialización del Frontend y Autenticación
Se ha inicializado el proyecto Frontend usando Next.js 14 y se ha implementado la base de la autenticación con Firebase.

### Cambios Realizados:
- **Proyecto Frontend:**
  - Se creó el proyecto `frontend` usando `create-next-app` con TypeScript, Tailwind CSS y App Router.
  - Se configuró el `src` directory para una mejor organización.
- **Firebase Client:**
  - Se instaló la librería `firebase`.
  - Se creó `frontend/src/config/firebase.ts` para inicializar el SDK del cliente utilizando variables de entorno.
- **Autenticación:**
  - Se implementó `frontend/src/context/AuthContext.tsx` utilizando React Context para gestionar el estado del usuario de forma global.
  - El contexto escucha cambios en el estado de autenticación mediante `onAuthStateChanged` y expone el JWT Token (`getIdToken()`) para futuras integraciones con el backend.
- **Interfaz de Usuario (UI):**
  - Se creó la página de login en `frontend/src/app/login/page.tsx` con un diseño limpio y responsivo usando Tailwind CSS.
  - Se actualizó la página principal (`frontend/src/app/page.tsx`) para actuar como un dashboard protegido que redirige al login si no hay un usuario autenticado.
- **Configuración Global:**
  - Se envolvió la aplicación con el `AuthProvider` en `frontend/src/app/layout.tsx`.

### Decisiones de Diseño:
- **Next.js App Router:** Se eligió por sus capacidades modernas de renderizado y facilidad de enrutamiento.
- **React Context para Auth:** Permite un acceso sencillo al estado del usuario y al token en cualquier componente del frontend sin necesidad de librerías externas de gestión de estado complejas.
- **Seguridad en Cliente:** Aunque la validación final ocurre en el backend, el frontend gestiona la persistencia de la sesión y la obtención de tokens de forma segura a través de Firebase SDK.

---

## SPRINT 9: Exposición de API y Seguridad
Se ha habilitado la capa de presentación (API) para interactuar con los casos de uso definidos en la capa de Aplicación.
...

# BITÁCORA DE ARQUITECTURA - GALPON ERP

## SPRINT 11: Operaciones Diarias y Auth Backend
Se han expuesto las entidades operativas del dominio y se ha facilitado la integración con un endpoint de Login en el backend.

### Cambios Realizados:
- **Autenticación (Login Backend):**
  - Se implementó `LoginCommand` y `LoginCommandHandler` en la capa de Aplicación.
  - El handler utiliza `HttpClient` para comunicarse directamente con la REST API de Firebase Auth, permitiendo obtener el `idToken` y `refreshToken` desde el backend.
  - Se configuró la `ApiKey` de Firebase en `appsettings.json`.
  - Se creó `AuthController` con el endpoint `POST /api/auth/login` (`[AllowAnonymous]`).
- **Mortalidad (Bajas):**
  - Se actualizó la entidad `MortalidadDiaria` para incluir el campo `Causa`.
  - Se implementó `RegistrarMortalidadCommand` que actualiza los contadores de bajas en la entidad `Lote` y persiste el registro diario.
  - Se creó `MortalidadController` protegido por `[Authorize]`.
- **Gastos Operativos:**
  - Se actualizó la entidad `GastoOperativo` para incluir el campo `TipoGasto`.
  - Se implementaron los casos de uso `RegistrarGastoOperativoCommand` y `ObtenerGastosQuery` (con filtros por GalponId y LoteId).
  - Se creó `GastosController` con endpoints `POST` y `GET`.
- **Calendario Sanitario (Vacunas):**
  - Se implementaron los casos de uso `MarcarVacunaAplicadaCommand` y `ObtenerCalendarioPorLoteQuery`.
  - Se creó `CalendarioSanitarioController` con endpoints `GET /api/calendario/{loteId}` y `PUT /api/calendario/{actividadId}/aplicar`.
- **Infraestructura:**
  - Se crearon e integraron `IMortalidadRepository` y su implementación.
  - Se actualizó `IGastoOperativoRepository` para incluir `ObtenerTodosAsync`.
  - Se registró `HttpClient` en el contenedor de dependencias de la capa de Aplicación.

### Decisiones de Diseño:
- **Login Proxy:** Se decidió implementar el login en el backend para facilitar el uso de Swagger y herramientas de prueba, manteniendo Firebase como el proveedor de identidad centralizado.
- **Validación en el Dominio:** El registro de mortalidad invoca métodos de la entidad `Lote` (`RegistrarBajas`), asegurando que las reglas de negocio (ej. no registrar más bajas que pollos vivos) se validen en el corazón del dominio.
- **Flexibilidad en Gastos:** Los gastos operativos pueden asociarse a un galpón de forma general o a un lote específico, permitiendo un análisis de costos más granular.


Sprint 12 is complete.
   1. Domain: Added Apellidos, FechaNacimiento, Direccion, and
      Profesion to Usuario. Created RolesGalpon constants and
      updated entity methods.
   2. Application: Updated RegistrarUsuarioCommand,
      ActualizarUsuarioCommand, and their handlers. Added
      FluentValidation validators to enforce mandatory fields and
      valid roles.
   3. Infrastructure: Updated UsuarioConfiguration and implemented
      Actualizar in UsuarioRepository. Successfully generated and
      applied the AddPerfilUsuario migration.
   4. API & Docs: Updated Admin Maestro seeding in Program.cs and
      the API contract in docs/endpoints.md.

  The migration is applied, and the system is ready with the
  enriched user profile.

  # Documentación de Integración y Seguridad - Galpon ERP

## Módulo de Autenticación y Usuarios (Actualización Firestore)

### 1. Sincronización con Firebase Firestore
Se ha implementado la persistencia de usuarios en las colecciones de Firestore para asegurar que los datos del perfil estén disponibles tanto en Firebase Auth como en la base de datos de documentos.

- **Paquete Instalado:** `Google.Cloud.Firestore` en la capa de Infraestructura.
- **Colección:** `users`
- **Identificador:** Se utiliza el `Uid` de Firebase Auth como ID del documento en Firestore para mantener una relación 1:1.
- **Campos Sincronizados:**
  - `uid`: Identificador único de Firebase.
  - `email`: Correo electrónico del usuario.
  - `displayName`: Nombre completo (Nombre + Apellidos).
  - `nombre`: Nombre de pila.
  - `apellidos`: Apellidos del usuario.
  - `rol`: Rol asignado (Admin, Veterinario, Operador).
  - `direccion`: Dirección física.
  - `profesion`: Profesión o cargo.
  - `fechaNacimiento`: Fecha de nacimiento formateada (YYYY-MM-DD).
  - `createdAt`: Timestamp de creación en Firestore.

### 2. Cambios en la Capa de Aplicación
- **IAuthenticationService:** Se actualizó la interfaz para permitir el envío de `IDictionary<string, object> extraUserData` durante la creación del usuario.
- **RegistrarUsuarioCommandHandler:** Se modificó la lógica para capturar todos los campos del comando y enviarlos a Firestore a través del servicio de autenticación antes de persistir en la base de datos local (PostgreSQL).

### 3. Cambios en la Capa de Infraestructura
- **FirebaseAuthService:**
  - Inicialización de `FirestoreDb` utilizando el `ProjectId` configurado.
  - Implementación de `SetAsync` en la colección `users` al ejecutar `CreateUserAsync`.
  - Manejo de excepciones para recuperación de UID si el email ya existe en Firebase.

## Flujo de Registro de Usuario
1. **Validación Local:** Se verifica si el email ya existe en PostgreSQL.
2. **Firebase Auth:** Se crea el registro de autenticación (credenciales).
3. **Firebase Firestore:** Se crea el documento en la colección `users` con los metadatos del perfil.
4. **Persistencia Local:** Se guarda el usuario en PostgreSQL para relaciones relacionales (Lotes, Galpones, etc.).

---
*Documentación actualizada al: 10 de Abril, 2026*

# Documentación de Integración y Seguridad - Galpon ERP

## Módulo de Autenticación y Usuarios (Actualización Firestore)

### 1. Sincronización con Firebase Firestore
Se ha implementado la persistencia de usuarios en las colecciones de Firestore para asegurar que los datos del perfil estén disponibles tanto en Firebase Auth como en la base de datos de documentos.

- **Paquete Instalado:** `Google.Cloud.Firestore` en la capa de Infraestructura.
- **Colección:** `users`
- **Identificador:** Se utiliza el `Uid` de Firebase Auth como ID del documento en Firestore para mantener una relación 1:1.
- **Campos Sincronizados:**
  - `uid`: Identificador único de Firebase.
  - `email`: Correo electrónico del usuario.
  - `displayName`: Nombre completo (Nombre + Apellidos).
  - `nombre`: Nombre de pila.
  - `apellidos`: Apellidos del usuario.
  - `rol`: Rol asignado (Admin, Veterinario, Operador).
  - `direccion`: Dirección física.
  - `profesion`: Profesión o cargo.
  - `fechaNacimiento`: Fecha de nacimiento formateada (YYYY-MM-DD).
  - `createdAt`: Timestamp de creación en Firestore.

### 2. Cambios en la Capa de Aplicación
- **IAuthenticationService:** Se actualizó la interfaz para permitir el envío de `IDictionary<string, object> extraUserData` durante la creación del usuario.
- **RegistrarUsuarioCommandHandler:** Se modificó la lógica para capturar todos los campos del comando y enviarlos a Firestore a través del servicio de autenticación antes de persistir en la base de datos local (PostgreSQL).

### 3. Cambios en la Capa de Infraestructura
- **FirebaseAuthService:**
  - Inicialización de `FirestoreDb` utilizando el `ProjectId` configurado.
  - Implementación de `SetAsync` en la colección `users` al ejecutar `CreateUserAsync`.
  - Manejo de excepciones para recuperación de UID si el email ya existe en Firebase.

## Flujo de Registro de Usuario
1. **Validación Local:** Se verifica si el email ya existe en PostgreSQL.
2. **Firebase Auth:** Se crea el registro de autenticación (credenciales).
3. **Firebase Firestore:** Se crea el documento en la colección `users` con los metadatos del perfil.
4. **Persistencia Local:** Se guarda el usuario en PostgreSQL para relaciones relacionales (Lotes, Galpones, etc.).

---
*Documentación actualizada al: 10 de Abril, 2026*


# Bitácora de Desarrollo - GalponERP

## Sprint : Catálogos Maestros (Fundación de Datos)

### Cambios realizados:
- **Domain:**
    - `IProductoRepository`: Se agregaron los métodos `Agregar` y `Actualizar` para permitir el CRUD completo.
    - `Producto.cs`: Se agregó el método `Actualizar` para permitir la modificación de múltiples propiedades.
- **Application:**
    - **Clientes:**
        - `CrearClienteCommand`: Comando y manejador para registrar nuevos clientes.
        - `ActualizarClienteCommand`: Comando y manejador para editar clientes existentes.
        - `EliminarClienteCommand`: Comando y manejador para realizar el Soft Delete de clientes.
        - `ListarClientesQuery`: Consulta y manejador para obtener la lista de clientes (incluyendo estado activo).
    - **Productos:**
        - `CrearProductoCommand`: Comando y manejador para registrar nuevos productos (Alimento, Medicamento, Insumo, etc.).
        - `ActualizarProductoCommand`: Comando y manejador para editar productos existentes.
        - `EliminarProductoCommand`: Comando y manejador para realizar el Soft Delete de productos.
        - `ListarProductosQuery`: Consulta y manejador para obtener la lista de productos (incluyendo estado activo).
- **API:**
    - `ClientesController`: Nuevo controlador para exponer el CRUD completo de clientes en `api/Clientes`.
    - `ProductosController`: Nuevo controlador para exponer el CRUD completo de productos en `api/Productos`.

### Observaciones:
- Se optó por crear carpetas independientes para `Clientes` y `Productos` en `GalponERP.Application` para mantener la consistencia con otros dominios (Galpones, Lotes, etc.), en lugar de agruparlos bajo `Catalogos`.
- Los endpoints existentes en `CatalogosController` se mantienen por compatibilidad, pero se recomienda usar los nuevos controladores dedicados para operaciones de catálogo maestro.

## Sprint 12: Vistas de Lote e Inventario Operativo

### Cambios realizados:
- **Domain:**
    - `ILoteRepository`: Se agregó `ObtenerTodosAsync` para permitir listado completo.
- **Application:**
    - **Lotes:**
        - `ListarLotesQuery`: Consulta para listar lotes con filtro opcional de activos.
        - `ObtenerDetalleLoteQuery`: Consulta detallada que consolida datos de ventas, mortalidad y gastos para calcular la utilidad estimada del lote.
    - **Inventario:**
        - `RegistrarMovimientoInventarioCommand`: Comando para registrar entradas/salidas de productos (alimento, medicina, etc.).
        - `ObtenerStockActualQuery`: Consulta que calcula el balance neto de stock por producto basándose en el historial de movimientos.
- **Infrastructure:**
    - `LoteRepository`: Implementación del nuevo método de listado.
- **API:**
    - `LotesController`: Se agregaron endpoints GET para listar y obtener detalle por ID.
    - `InventarioController`: Se agregaron endpoints para registrar movimientos y consultar stock actual.

### Observaciones:
- La consulta de detalle de lote realiza agregaciones en memoria de múltiples repositorios. Para grandes volúmenes de datos, se podría considerar una vista materializada o una tabla de resumen en el futuro.
- El stock actual se calcula dinámicamente. Si el historial crece demasiado, se recomienda implementar una tabla de `StockActual` que se actualice con cada movimiento.

## Sprint 13: Dashboard y Reportes de Rentabilidad

### Cambios realizados:
- **Domain:**
    - `IMortalidadRepository`: Se agregó `ObtenerPorRangoFechasAsync` para facilitar reportes temporales.
- **Application:**
    - **Dashboard:**
        - `ObtenerResumenDashboardQuery`: Nueva consulta que consolida el estado actual de la granja (aves vivas), mortalidad mensual e inventario crítico en una sola respuesta.
- **Infrastructure:**
    - `MortalidadRepository`: Implementación del método de consulta por rango de fechas.
- **API:**
    - `DashboardController`: Nuevo controlador con endpoint `GET /api/dashboard/resumen`.
    - **Seguridad:** Se realizó una auditoría y se confirmó que los 13 controladores operativos cuentan con el atributo `[Authorize]`.

### Observaciones:
- El dashboard reutiliza la lógica de cálculo de consumo de alimento del caso de uso de inventario para asegurar consistencia en las alertas.
- Se mantiene `AuthController` con acceso anónimo únicamente para el proceso de autenticación.


# BITÁCORA ARQUITECTÓNICA - GALPON ERP

## Decisión 14.1: Implementación de RBAC (Role-Based Access Control) con Enums
Se ha refactorizado el sistema de roles de un esquema basado en strings constantes a un `enum RolGalpon` numérico para mayor control y tipado fuerte.

**Jerarquía de Roles:**
*   `Empleado = 0`: Acceso operativo (Lotes, Mortalidad, Galpones, Inventario, Calendario).
*   `SubAdmin = 1`: Acceso de gestión (Ventas, Gastos, Productos, Clientes, Dashboard, Planificación).
*   `Admin = 2`: Acceso total (Gestión de Usuarios, Configuración del Sistema).

**Mecánica de Inyección:**
El `Rol` se almacena como un entero en la base de datos PostgreSQL mediante EF Core. Al validar el JWT de Firebase, se intercepta el evento `OnTokenValidated`, se busca al usuario por su `FirebaseUid` en la BD local y se inyecta un `ClaimTypes.Role` con el nombre del enum (Admin, SubAdmin, Empleado).

## Decisión 14.2: Migración RefactorRolesEnum
Se generó una migración no destructiva para convertir la columna `Rol` de `character varying(50)` a `integer`. Se recomienda que antes de aplicar la migración en producción, se limpien o mapeen los valores de texto existentes a sus equivalentes numéricos (Admin -> 2, etc.).

## Decisión 14.3: Blindaje de Controladores
Se aplicó el atributo `[Authorize(Roles = "Admin,SubAdmin,Empleado")]` según la criticidad del endpoint, asegurando que el principio de menor privilegio se cumpla.

## Decisión 15.1: Módulo de Pesajes y Cálculo de FCR
Se implementó la entidad `PesajeLote` para registrar el peso promedio de muestras de pollos. El Índice de Conversión Alimenticia (FCR) se calcula dinámicamente en la consulta de detalle del lote utilizando la fórmula:
`FCR = Alimento Consumido (Kg) / Incremento de Biomasa (Kg)`.

**Consideraciones Técnicas:**
- El peso inicial del pollito se estima en 40g para el cálculo del incremento.
- Se filtran los movimientos de inventario de tipo "Salida" para productos de tipo "Alimento" vinculados al lote.
- Se requiere al menos un registro de pesaje para obtener el FCR actual.

## Decisión 16.1: Venta basada en Peso
Se refactorizó la entidad `Venta` para alejarse de un modelo de "precio unitario por pollo" hacia uno de "peso total vendido y precio por kilo", alineándose con la realidad comercial de la industria avícola.

## Decisión 17.1: Identidad y Gestión de Catálogos
Se implementó el endpoint `/api/usuarios/me` para permitir al frontend obtener los datos del usuario logueado y su rol (RBAC) de forma atómica. Se completó la trazabilidad de catálogos permitiendo `Update` y `Soft Delete` en Clientes y Productos.

## Decisión 18.1: Trazabilidad de Inventario (Kardex) y Ajustes
Se habilitó la consulta histórica de movimientos para auditoría. Para los ajustes manuales (mermas/robos), se extendió la entidad `MovimientoInventario` con un campo `Justificacion` obligatorio para mantener la integridad de la bitácora de almacén.

## Decisión 19.1: Desacoplamiento de Históricos
Se crearon endpoints específicos para listar Ventas y Mortalidad fuera del contexto de un solo lote, permitiendo análisis transversales de la operación.

## Decisión 20.1: Inteligencia de Negocio y Proyecciones
Se implementó un algoritmo de proyección de sacrificio que utiliza el **FCR Actual** del lote para ajustar la **Ganancia Diaria Estimada**. 
- Si un lote tiene un FCR ineficiente (alto), el sistema proyecta un crecimiento más lento y una fecha de sacrificio más tardía.
- Se añadió una comparativa financiera entre los últimos 5 lotes para identificar tendencias de rentabilidad y eficiencia biológica.

# BITÁCORA ARQUITECTÓNICA - GALPON ERP

## Decisión 14.1: Implementación de RBAC (Role-Based Access Control) con Enums
Se ha refactorizado el sistema de roles de un esquema basado en strings constantes a un `enum RolGalpon` numérico para mayor control y tipado fuerte.

**Jerarquía de Roles:**
*   `Empleado = 0`: Acceso operativo (Lotes, Mortalidad, Galpones, Inventario, Calendario).
*   `SubAdmin = 1`: Acceso de gestión (Ventas, Gastos, Productos, Clientes, Dashboard, Planificación).
*   `Admin = 2`: Acceso total (Gestión de Usuarios, Configuración del Sistema).

**Mecánica de Inyección:**
El `Rol` se almacena como un entero en la base de datos PostgreSQL mediante EF Core. Al validar el JWT de Firebase, se intercepta el evento `OnTokenValidated`, se busca al usuario por su `FirebaseUid` en la BD local y se inyecta un `ClaimTypes.Role` con el nombre del enum (Admin, SubAdmin, Empleado).

## Decisión 14.2: Migración RefactorRolesEnum
Se generó una migración no destructiva para convertir la columna `Rol` de `character varying(50)` a `integer`. Se recomienda que antes de aplicar la migración en producción, se limpien o mapeen los valores de texto existentes a sus equivalentes numéricos (Admin -> 2, etc.).

## Decisión 14.3: Blindaje de Controladores
Se aplicó el atributo `[Authorize(Roles = "Admin,SubAdmin,Empleado")]` según la criticidad del endpoint, asegurando que el principio de menor privilegio se cumpla.

## Decisión 15.1: Módulo de Pesajes y Cálculo de FCR
Se implementó la entidad `PesajeLote` para registrar el peso promedio de muestras de pollos. El Índice de Conversión Alimenticia (FCR) se calcula dinámicamente en la consulta de detalle del lote utilizando la fórmula:
`FCR = Alimento Consumido (Kg) / Incremento de Biomasa (Kg)`.

**Consideraciones Técnicas:**
- El peso inicial del pollito se estima en 40g para el cálculo del incremento.
- Se filtran los movimientos de inventario de tipo "Salida" para productos de tipo "Alimento" vinculados al lote.
- Se requiere al menos un registro de pesaje para obtener el FCR actual.

## Decisión 16.1: Venta basada en Peso
Se refactorizó la entidad `Venta` para alejarse de un modelo de "precio unitario por pollo" hacia uno de "peso total vendido y precio por kilo", alineándose con la realidad comercial de la industria avícola.

## Decisión 17.1: Identidad y Gestión de Catálogos
Se implementó el endpoint `/api/usuarios/me` para permitir al frontend obtener los datos del usuario logueado y su rol (RBAC) de forma atómica. Se completó la trazabilidad de catálogos permitiendo `Update` y `Soft Delete` en Clientes y Productos.

## Decisión 18.1: Trazabilidad de Inventario (Kardex) y Ajustes
Se habilitó la consulta histórica de movimientos para auditoría. Para los ajustes manuales (mermas/robos), se extendió la entidad `MovimientoInventario` con un campo `Justificacion` obligatorio para mantener la integridad de la bitácora de almacén.

## Decisión 19.1: Desacoplamiento de Históricos
Se crearon endpoints específicos para listar Ventas y Mortalidad fuera del contexto de un solo lote, permitiendo análisis transversales de la operación.

## Decisión 21.1: Limpieza de warnings y GoogleCredential
Se refactorizó la carga de credenciales de Firebase en `FirebaseAuthService` y `Program.cs` para utilizar `GoogleCredential.GetApplicationDefault()`. Para compatibilidad en desarrollo local con archivos JSON, se inyecta la ruta del archivo en la variable de entorno `GOOGLE_APPLICATION_CREDENTIALS` dinámicamente. Se eliminaron todos los warnings de compilación (posibles nulos y métodos obsoletos).

## Decisión 22.1: Accountability y Auditoría de Transacciones
Se implementó un sistema de auditoría obligatorio para todas las transacciones financieras (Ventas, Gastos) y operativas (Movimientos de Inventario, Pesajes, Mortalidad). 
1. **Identidad Segura:** El `UsuarioId` local (Guid) se extrae del JWT a través del `FirebaseUid` buscando en `IUsuarioRepository` en cada controlador.
2. **Inmutabilidad:** El `UsuarioId` se inyecta en los comandos de MediatR y se guarda permanentemente en la base de datos para saber exactamente quién registró cada acción.
3. **Migración Segura:** Se corrigió un error de casting automático en PostgreSQL para la columna `Rol` mediante una cláusula `USING (CASE ...)` en la migración `RefactorRolesEnum`, asegurando la integridad de los datos existentes.

## Decisión 22.2: Actualización de Contratos API
Se actualizó `endpoints.md` para notificar al Frontend que la auditoría es transparente; el cliente no debe (ni puede) enviar el `UsuarioId` en los payloads JSON, delegando la responsabilidad de identidad totalmente al Backend.

## Decisión 23.1: Flexibilidad Financiera en Objeto Moneda
Se eliminó la restricción de montos no negativos en el Value Object `Moneda`. Esto es necesario para representar correctamente conceptos de **Pérdida Neta** o **Utilidad Negativa** al cerrar un lote donde los costos superan los ingresos, evitando excepciones de negocio fatales durante el cierre contable.

## Decisión 23.2: Corrección de Filtros en Listado de Lotes
Se corrigió un error en `LoteRepository` donde el parámetro `soloActivos` no funcionaba correctamente debido a los filtros globales de EF Core. Se implementó `.IgnoreQueryFilters()` en la consulta de "todos los lotes" para asegurar que el sistema pueda distinguir correctamente entre lotes activos, cerrados y eliminados.

## Decisión 23.3: Integridad Contable y Snapshots de Cierre
Se implementó un sistema de "congelación" de datos al cerrar un lote para asegurar la inmutabilidad de los reportes históricos.
1. **Snapshots en Lote:** Se agregaron los campos `FCRFinal`, `CostoTotalFinal`, `UtilidadNetaFinal` y `PorcentajeMortalidadFinal` a la entidad `Lote`. Estos valores se calculan y guardan permanentemente en la base de datos al ejecutar el comando `CerrarLote`.
2. **Estado de Pago en Ventas:** Se introdujo el enum `EstadoPago` (`Pagado`, `Pendiente`, `Parcial`) en la entidad `Venta` para permitir el seguimiento de cuentas por cobrar. Por defecto, todas las ventas se registran como `Pagado`.
3. **Mecánica de Anulación Segura:** Se creó el caso de uso `AnularVentaCommand`. Esta operación realiza un **Soft Delete** de la venta (`IsActive = false`) y **devuelve automáticamente la cantidad de pollos vendidos al inventario del lote**, garantizando la consistencia del conteo biológico. Esta acción está restringida únicamente al rol `Admin` y solo es permitida si el lote asociado no ha sido cerrado.
4. **Cálculo de FCR de Cierre:** El FCR final se calcula sumando todos los movimientos de salida de productos tipo `Alimento` vinculados al lote y dividiéndolos por el peso total vendido.

## Decisión 24.1: Optimización de Base de Datos y Background Jobs
1. **Indexación Estratégica:** Se configuraron índices explícitos en las columnas de alta frecuencia de consulta: `Fecha` en `Ventas` y `MovimientosInventario`. Las llaves foráneas (`LoteId`, `ProductoId`, `ClienteId`) ya cuentan con índices implícitos creados por EF Core. Esto optimiza los reportes transversales y el cálculo de FCR en tiempo real.
2. **Automatización de Alertas Sanitarias:** Se implementó `AlertaSanitariaJob` como un `BackgroundService`. Este servicio escanea diariamente todos los lotes activos, calcula su edad actual (días desde ingreso) y compara contra el `CalendarioSanitario`. Cualquier actividad pendiente (Vacunas/Tratamientos) que deba aplicarse hasta la fecha actual es reportada mediante el Logger del sistema para acción inmediata.

## Decisón 25.1: Dominio Dinámico y Ancla Matemática (SaaS)
Se ha migrado la estructura de productos de Enums estáticos a un modelo de catálogos dinámicos para soportar la escalabilidad multi-tenant y la flexibilidad de tipos de insumos.

1. **Entidades de Catálogo:** Se crearon las entidades `CategoriaProducto` (Nombre, Descripcion) y `UnidadMedida` (Nombre, Abreviatura). Esto permite que cada usuario defina sus propios tipos de productos (Iniciador, Crecimiento, Vacuna Newcastle, etc.) y unidades (Saco 40kg, Frasco 50 dosis, Litro) sin cambios de código.
2. **Equivalencia en Kg (El Ancla):** Se introdujo la propiedad `EquivalenciaEnKg` en la entidad `Producto`. Esta es la decisión técnica más crítica: todos los cálculos de FCR e inventario ahora dependen de este multiplicador decimal.
   - *Ejemplo:* Si un producto es "Alimento Iniciador" y su unidad es "Saco 40kg", su `EquivalenciaEnKg` es `40.0`. Los movimientos de inventario se registran en "Sacos", pero el motor de FCR los procesa en "Kg" automáticamente.
3. **Refactorización de Producto:** Se eliminaron los enums `TipoProducto` y `UnidadMedida`. La entidad `Producto` ahora utiliza llaves foráneas (`CategoriaProductoId`, `UnidadMedidaId`) con navegación obligatoria y carga mediante `.Include()`.

## Decisión 26.1: Estrategia de Migración de Datos SaaS (Zero Data Loss)
Para evitar la pérdida de información de productos existentes durante el cambio de esquema de base de datos, se implementó una migración de EF Core personalizada:

1. **Procedimiento Up():**
   - Se crean las nuevas tablas `CategoriasProductos` y `UnidadesMedida`.
   - Se añaden las columnas de FK a `Productos` como nulables inicialmente.
   - Se realiza un **Seeding de Identidad**: Inserción de categorías y unidades por defecto mediante SQL directo para garantizar IDs consistentes.
   - **Mapeo Transaccional:** Se ejecutaron sentencias `UPDATE` para migrar los antiguos valores de texto (Enums) a los nuevos IDs de catálogo y asignar equivalencias por defecto (ej. Saco -> 40kg).
   - Se eliminan las columnas obsoletas y se aplica la restricción `NOT NULL`.

## Decisión 26.2: Refactorización del Motor de Cálculo (FCR y Stock)
Se actualizaron todos los casos de uso que consumen inventario para alinearse al nuevo modelo:
1. **Identificación de Alimento:** En `CerrarLote` y `ObtenerDetalleLote`, los productos se filtran ahora comparando `p.Categoria.Nombre == "Alimento"`.
2. **Cálculo de Consumo:** La fórmula de alimento consumido cambió de `Sum(Cantidad)` a `Sum(Cantidad * Producto.EquivalenciaEnKg)`, garantizando que el FCR sea siempre una relación Kg/Kg independientemente de la unidad de despacho.
3. **Normalización de Unidades:** El reporte de stock ahora muestra tanto el nombre de la categoría como la unidad de medida dinámica, mejorando la legibilidad para el usuario final.

## Decisión 26.3: Exposición de Catálogos y Seguridad
Se implementaron controladores específicos (`CategoriasController`, `UnidadesMedidaController`) protegidos bajo RBAC:
- **Lectura:** Disponible para `Admin, SubAdmin`.
- **Escritura/Anulación:** Restringida estrictamente a `Admin, SubAdmin`.
- **Soft Delete:** Todas las acciones de eliminación en catálogos utilizan el patrón `IsActive = false` heredado de la clase base `Entity`.

## Decisión 27.1: Ciclo de Vida de Gastos Operativos
Se completó la trazabilidad de gastos operativos permitiendo la edición y anulación segura.
1. **Edición (Update):** Se implementó `ActualizarGastoOperativoCommand` permitiendo modificar descripción, monto, fecha, tipo de gasto y lote asociado.
2. **Anulación (Soft Delete):** Se implementó `EliminarGastoOperativoCommand` que marca el gasto como `IsActive = false`. Ambas operaciones capturan el `UsuarioId` del JWT para auditoría obligatoria.
3. **Seguridad:** Endpoints `PUT` y `DELETE` protegidos bajo los roles `Admin` y `SubAdmin`.

## Decisión 27.2: Trazabilidad de Ventas por Lote
Se implementó el endpoint `GET /api/ventas/lote/{loteId}` para facilitar la conciliación financiera y el análisis de rentabilidad específico de cada lote desde el frontend, permitiendo listar todas las transacciones de venta (activas) asociadas a una unidad de producción.
