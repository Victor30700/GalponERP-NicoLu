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


# Bitácora de Arquitectura - Pollos NicoLu Fase 2.0

## Sprint 32: Finanzas Reales (Cuentas por Cobrar y Pagos)

### Decisiones de Diseño
- **Mapeo de Pagos**: Se implementó `PagoVenta` como una entidad relacionada con `Venta`. Aunque el dominio ya tenía la lógica, faltaba la persistencia en base de datos.
- **Relación Venta-Pagos**: Se configuró una relación 1:N en EF Core usando acceso por campo (`_pagos`) para respetar el encapsulamiento de DDD. El `SaldoPendiente` se calcula dinámicamente en el dominio basado en la suma de los montos de los pagos.
- **Estado de Pago**: Se cambió el valor por defecto de `EstadoPago` a `Pendiente` (2) en la configuración de base de datos para alinearse con el constructor de la entidad `Venta`.
- **Cuentas por Cobrar**: Se expuso el endpoint `GET /api/finanzas/cuentas-por-cobrar` que utiliza el `ObtenerCuentasPorCobrarQuery` ya implementado, permitiendo al frontend visualizar el saldo pendiente de cada venta no pagada totalmente.

### Cambios Técnicos
- Creado `PagoVentaConfiguration.cs`.
- Actualizado `VentaConfiguration.cs` con la relación y valor por defecto.
- Actualizado `GalponDbContext.cs` con el `DbSet<PagoVenta>`.
- Refactorizado `VentaRepository.cs` para incluir la colección de `Pagos` en todas las consultas (Eager Loading), necesario para que el dominio pueda calcular el saldo pendiente.
- Registrada la migración `AddPagosCuentasPorCobrar`.
- Expuesto endpoint en `FinanzasController.cs`.

## Sprint 33: Sanidad SaaS (Plantillas Dinámicas)

### Decisiones de Diseño
- **Plantillas de Sanidad**: Se crearon las entidades `PlantillaSanitaria` y `ActividadPlantilla` para permitir que los usuarios definan sus propios planes de vacunación y tratamiento, eliminando la lógica hardcodeada.
- **Relación con Productos**: Se añadió `ProductoIdRecomendado` tanto en las plantillas como en el `CalendarioSanitario` final, permitiendo sugerir qué insumo de inventario debe usarse para cada tarea.
- **Inmutabilidad del Calendario**: Al crear un lote, las actividades de la plantilla se "clonan" hacia la tabla `CalendarioSanitario` del lote. Esto garantiza que si la plantilla original cambia después, los lotes ya creados mantengan su plan original (trazabilidad).
- **Fallback**: Si no se provee una plantilla al crear un lote, se mantiene un calendario base de 2 vacunas por retrocompatibilidad.

### Cambios Técnicos
- Creadas entidades `PlantillaSanitaria`, `ActividadPlantilla` y enum `TipoActividad`.
- Actualizada entidad `CalendarioSanitario` con `ProductoIdRecomendado`.
- Implementado CRUD completo para Plantillas en la capa de Aplicación.
- Refactorizado `CrearLoteCommandHandler` para inyectar `IPlantillaSanitariaRepository` y generar el calendario dinámicamente.
- Creado `PlantillasController` y aplicada la migración `AddPlantillasSanitarias`.

## Sprint 34: Operaciones de Ciclo de Vida Avanzado

### Decisiones de Diseño
- **Cancelación de Lote**: Se añadió soporte para cancelar lotes, lo cual requiere una justificación obligatoria. Esta acción cambia el estado del lote a `Cancelado` e inactiva automáticamente todos los recordatorios pendientes en su calendario sanitario (mediante soft delete de los items).
- **Traslado de Lote**: Se habilitó la capacidad de mover un lote de un galpón a otro. Esta acción es puramente logística y actualiza el `GalponId` del lote, manteniendo todo su historial operativo intacto.
- **Trazabilidad de Auditoría**: Gracias al `AuditoriaBehavior` existente, las acciones de cancelación y traslado quedan automáticamente registradas en los logs del sistema, capturando quién realizó el cambio y los datos enviados.

### Cambios Técnicos
- Actualizada entidad `Lote` con propiedad `JustificacionCancelacion` y métodos `Cancelar()` y `Trasladar()`.
- Implementados `CancelarLoteCommandHandler` y `TrasladarLoteCommandHandler`.
- Añadidos endpoints correspondientes en `LotesController`.
- Aplicada migración `AddJustificacionCancelacion`.

## Sprint 35: UX, Auditoría y Documentación

### Decisiones de Diseño
- **Filtrado de Auditoría**: Se extendió la capacidad de consulta de logs para permitir filtros por rango de fechas, usuario específico y tipo de entidad. Esto facilita la labor del administrador al rastrear cambios en el sistema.
- **Documentación Swagger**: Se habilitó la generación de archivos XML de documentación tanto en la capa de API como en Application. Swagger fue configurado para consumir ambos archivos, permitiendo que las descripciones de los DTOs y comandos sean visibles desde la interfaz de Swagger UI, mejorando la experiencia de integración para el frontend.

### Cambios Técnicos
- Actualizado `IAuditoriaRepository` y su implementación con `ObtenerFiltradosAsync`.
- Modificado `ObtenerAuditoriaLogsQuery` y su handler para soportar parámetros opcionales.
- Actualizado `AuditoriaController` para exponer los nuevos filtros vía Query String.
- Modificados archivos `.csproj` de API y Application para activar `GenerateDocumentationFile`.
- Configurado `Program.cs` para incluir los comentarios XML en Swagger.

# BITÁCORA DE ARQUITECTURA - FASE 2.1

## Sprint 36: Ejecución Sanitaria Integrada

### Decisiones de Diseño
1. **Automatización de Inventario en Casos de Uso Operativos:** 
   Anteriormente, la aplicación de vacunas era un simple cambio de estado. Para cumplir con la **Regla de Oro de Trazabilidad**, se integró el descuento de stock directamente en el Handler de `MarcarVacunaAplicada`. Esto asegura que no haya discrepancias entre lo "informado" en el calendario y lo "existente" en la bodega.

2. **Validación Preventiva de Negocio:**
   Se implementó la verificación de stock *antes* de cualquier cambio de estado. Si no hay suficiente vacuna, la operación falla atómicamente, obligando al usuario a regularizar el inventario primero. Esto protege la integridad del FCR y los costos del lote.

3. **Seguridad Transparente (Contexto de Usuario):**
   Se mejoró `Program.cs` para inyectar automáticamente el `UsuarioId` desde el JWT hacia el `ICurrentUserContext`. Esto elimina la necesidad de que el programador "recuerde" extraer el ID en cada controlador, reduciendo errores de auditoría y mejorando la ergonomía del código.

4. **Semántica de API (PATCH vs PUT):**
   Se eligió `PATCH` para el endpoint de aplicación porque representa una actualización parcial del recurso `CalendarioSanitario` (cambio de estado y registro de consumo) en lugar de un reemplazo total.

### Cambios Técnicos
- **Domain:** Se añadió `ObtenerStockPorProductoIdAsync` a `IInventarioRepository` para facilitar la validación de stock en capas superiores.
- **Application:** Refactorización de `MarcarVacunaAplicadaCommand` y su Handler.
- **Infrastructure:** Implementación eficiente del cálculo de stock mediante agregación directa en base de datos (`SumAsync`).
- **API:** Actualización de `CalendarioSanitarioController` con nueva ruta `api/calendario` y método `PATCH`.

## Sprint 37: Flujo Optimizado de Alimentación Diaria

### Decisiones de Diseño
1. **Normalización de Consumo (Kilogramos Reales):**
   Aunque el alimento puede ingresarse en unidades variadas (bultos, sacos, etc.), la lógica de biomasa requiere kilogramos. El Handler de consumo ahora utiliza la `EquivalenciaEnKg` definida en el Producto para asegurar que todos los KPIs biológicos (como el FCR) se calculen sobre una base común de peso, eliminando errores de conversión en el Frontend.

2. **Endpoint de Propósito Específico:**
   En lugar de usar el registro general de movimientos de inventario, se creó un endpoint dedicado `/api/inventario/consumo-diario`. Esto permite aplicar validaciones específicas de alimentación y simplifica la interfaz para el operario de campo.

3. **Garantía de Stock:**
   Se mantiene la política de "Cero Negativos". No se permite registrar un consumo si el stock actual en bodega es insuficiente, asegurando que el Kárdex refleje la realidad física de la granja.

### Cambios Técnicos
- **Application:** Creación de `RegistrarConsumoAlimentoCommand` y su Handler.
- **API:** Nuevo endpoint `POST` en `InventarioController`.
- **Domain:** Uso de `InventarioDomainException` para fallos de stock en alimentación.

# BITÁCORA DE ARQUITECTURA - SPRINT 47: SELLO DE AUDITORÍA Y CONSISTENCIA FINAL

## 1. Gestión de Pagos (Anulación Segura)
Se ha implementado una arquitectura de anulación de pagos que garantiza la integridad financiera:
- **Entidad Venta:** Se modificó la propiedad `SaldoPendiente` para que solo sume los pagos donde `IsActive == true`.
- **AnularPago (Método de Dominio):** Al anular un pago, se invoca `ActualizarEstadoPagoSegunSaldos()`, lo que permite que una venta que estaba "Pagada" regrese a estado "Parcial" o "Pendiente" de forma automática y atómica.
- **Seguridad:** El endpoint `DELETE /api/ventas/{id}/pagos/{pagoId}` está restringido estrictamente al rol `Admin`.

## 2. Consistencia de Stock (Kárdex vs Dashboard)
Se identificó y corrigió una discrepancia en el cálculo del stock actual:
- **Fuga de Datos:** `ObtenerStockActualQueryHandler` omitía los movimientos de tipo `Compra`, lo que generaba un stock inferior al real en la pantalla de inventario comparado con el Dashboard.
- **Fórmula Unificada:**
  ```csharp
  Impacto = (Tipo == Entrada || Tipo == Compra || Tipo == AjusteEntrada) ? Cantidad : -Cantidad;
  ```
- **Normalización a Kg:** Se añadió `StockActualKg` calculado como `StockActual * Producto.EquivalenciaEnKg` para permitir al frontend mostrar biomasa total disponible para alimento.

## 3. Auditoría de Queries
Se habilitó el endpoint `GET /api/ventas/{id}/pagos` que muestra todos los pagos asociados, incluyendo los anulados (`IsActive = false`), permitiendo a los auditores ver quién anuló un pago y cuándo (vía campos de auditoría de la entidad).

## Estatus de Compilación
El proyecto compila correctamente sin errores. Todas las dependencias de `IUnitOfWork` y `IVentaRepository` fueron respetadas.
# BITÁCORA DE ARQUITECTURA - FASE 3.0: MOTOR FINANCIERO AVANZADO

## SPRINT 49: El Pasivo del Negocio (Proveedores)
- **Entidad Proveedor:** Se ha implementado la entidad `Proveedor` heredando de `Entity` base para soportar Auditoría y Soft Delete.
- **Relación de Dominio:** Los proveedores ahora son entidades de primer nivel, esenciales para el registro de compras formales y el futuro costeo PPP.
- **Seguridad:** El CRUD de proveedores sigue las reglas de roles: `Admin` para eliminación, `SubAdmin` para creación/edición, y `Empleado` para lectura.

## SPRINT 50: Integración de Compras y Cuentas por Pagar
- **Entidad CompraInventario:** Nueva entidad que gestiona la deuda con proveedores. Soporta `EstadoPago` (Pagado, Pendiente, Parcial) y calcula el `SaldoPendiente` dinámicamente.
- **Transaccionalidad (IUnitOfWork):** El comando `RegistrarIngresoMercaderiaCommand` ahora es atómico. En una sola transacción:
    1. Se crea el registro financiero (`CompraInventario`).
    2. Se crea el registro físico en el Kárdex (`MovimientoInventario` tipo `Compra`).
    3. Se actualiza el stock biológico del producto.
- **Kárdex Valorado:** Se vinculó `CompraId` a `MovimientoInventario` para permitir la trazabilidad total desde el stock hasta el origen de la compra.
- **Cuentas por Pagar:** Se habilitó el query `ObtenerCuentasPorPagar` que filtra todas las compras con saldo pendiente, permitiendo una gestión de tesorería proactiva.

## SPRINT 51: El Eslabón Perdido (Costeo PPP)
- **Algoritmo PPP:** Se implementó `RecalcularCostoPPP` en la entidad `Producto`. El costo unitario se actualiza dinámicamente con cada compra formal: `((StockActual * CostoActual) + (CantidadComprada * PrecioCompra)) / (StockActual + CantidadComprada)`.
- **Salidas Valoradas:** `RegistrarConsumoAlimentoCommandHandler` ahora asigna el `CostoTotal` al movimiento de salida basado en el PPP vigente.
- **Cierre de Lote Real:** `CerrarLoteCommandHandler` ahora suma el costo real del alimento consumido (desde el Kárdex) en lugar de usar estimaciones o valor cero.
- **Valoración de Bodega:** Nuevo endpoint `GET /api/inventario/valoracion` que calcula el capital inmovilizado en insumos.

## SPRINT 52: Inteligencia Predictiva y Papelera Forense
- **Proyección de Stock:** Algoritmo que cruza el stock actual con el consumo diario estimado por edad de ave (gramos/día/ave) para predecir cuántos días de alimento restan.
- **Restauración Universal:** Comando `RestaurarEntidadCommand` que utiliza reflexión para revertir el `IsActive = false` en cualquier entidad del sistema (Lotes, Ventas, Gastos, etc.), restringido a `Admin`.
- **Abstracción de Persistencia:** Se creó e implementó `IGalponDbContext` para desacoplar la Application de la implementación concreta de Infraestructura.

## Estatus de Compilación
El proyecto compila correctamente. Se han aplicado las migraciones `AddProveedoresYCompras` y `AddCostoPPP`.
# BITÁCORA DE ARQUITECTURA - FASE 3.1

## SPRINT 56: Conciliación de Almacén y Reportabilidad SaaS

### Decisiones Tomadas
1. **Motor de Conciliación Masiva:** Se implementó `RegistrarConciliacionStockCommand` para automatizar los ajustes tras inventarios físicos. El sistema:
   - Calcula el stock teórico (sistema) vs el físico (conteo).
   - Genera automáticamente movimientos de `AjusteEntrada` o `AjusteSalida` solo por la diferencia.
   - Valora estos ajustes automáticamente usando el PPP actual del producto para mantener la integridad contable.
2. **Infraestructura de Reportes PDF:** Se integró la librería `QuestPDF` para la generación de documentos profesionales. Se optó por una arquitectura de "Servicio de Infraestructura" (`PdfService`) inyectado mediante una interfaz en Application, permitiendo generar la "Ficha de Liquidación de Lote" de forma programática.
3. **Optimización de Descargas:** El endpoint `/api/lotes/{id}/reporte-cierre-pdf` devuelve un flujo de bytes con el MIME type `application/pdf`, permitiendo al navegador o aplicación móvil previsualizar o descargar el documento directamente.

# BITÁCORA DE ARQUITECTURA - FASE 3.1

## SPRINT 58: Configuración Global (Tenant Settings)

### Decisiones Tomadas
1. **Entidad Singleton de Configuración:** Se implementó `ConfiguracionSistema` para centralizar los datos de identidad de la granja (Nombre, NIT, Moneda). 
   - **Arquitectura:** Aunque en la base de datos es una tabla regular, el `ConfiguracionRepository` y el Handler aseguran que se comporte como un Singleton operativo, devolviendo siempre el primer registro encontrado.
2. **Inyección en Capa de Reportabilidad:** Se refactorizó `PdfService` para aceptar opcionalmente la configuración global.
   - **Dinamicidad:** Las cabeceras de los PDFs (Ficha de Liquidación) ahora muestran datos reales de la empresa en lugar de texto estático (Hardcoded).
   - **Resiliencia:** El sistema provee valores por defecto ("Pollos NicoLu") si aún no se ha realizado la primera configuración, evitando errores en tiempo de ejecución.
3. **Seguridad Administrativa:** El endpoint `POST /api/configuracion` está restringido exclusivamente al rol `Admin`, asegurando que solo el propietario o administrador pueda alterar la identidad legal del sistema.


# BITÁCORA DE ARQUITECTURA - FASE 4: IA Y ORQUESTACIÓN

## Sprint 61: Cerebro Base y Orquestador

### 1. Registro del Kernel en el Contenedor DI
Se ha configurado el registro de Semantic Kernel en la capa de `Infrastructure` (`GalponERP.Infrastructure/DependencyInjection.cs`) para mantener la separación de responsabilidades, ya que la conexión con el modelo local Ollama se considera un detalle de infraestructura.

- **Endpoint:** `http://localhost:11434`
- **Modelo:** `gemma4:e4b`
- **Paquete de Conector:** `Microsoft.SemanticKernel.Connectors.Ollama`

```csharp
// Registro en DependencyInjection.cs de Infrastructure
services.AddKernel()
        .AddOllamaChatCompletion(
            modelId: "gemma4:e4b",
            endpoint: new Uri("http://localhost:11434")
        );
```

### 2. Implementación de Plugins (Skills)
Se ha creado el primer plugin de dominio en `GalponERP.Application/Agentes/Plugins/ProduccionPlugin.cs`. Este plugin actúa como puente entre el lenguaje natural y los comandos existentes del sistema.

- **Método:** `RegistrarBajasGalpon`
- **Comando Invocado:** `RegistrarMortalidadCommand` via `IMediator`.
- **Inyección de Dependencias:** El plugin recibe `IMediator` e `ICurrentUserContext` a través de su constructor, los cuales son resueltos por el ServiceProvider del Kernel.

### 3. Servicio Orquestador
El `AgenteOrquestadorService` centraliza la lógica de procesamiento. Se encarga de:
- Inicializar el historial de chat con un System Prompt descriptivo.
- Registrar los plugins dinámicamente en el Kernel.
- Habilitar `FunctionChoiceBehavior.Auto()` para permitir que la IA decida cuándo invocar herramientas.
- Manejar la comunicación con el servicio de chat completion de forma genérica para evitar dependencias directas de conectores específicos en la capa de Aplicación.

### 4. Exposición vía API
Se ha creado el `AgentesController` con un endpoint seguro (`[Authorize]`) que permite interactuar con el orquestador. Esto permite probar la IA desde herramientas como Postman antes de integrarla con servicios de mensajería.

**Ruta:** `POST /api/agentes/chat`
**Body:** `{ "mensaje": "Registra 5 muertes por calor en el galpón 1" }`

## Sprint 61.5: Resolución Automática de Contexto

### 1. Inyección de Realidad Temporal
Se ha actualizado el `AgenteOrquestadorService` para inyectar dinámicamente la fecha y hora actual en el `SystemMessage`. Esto permite que el LLM entienda referencias relativas como "hoy" o "ayer" sin necesidad de procesar lógica de fechas compleja por su cuenta.

### 2. Abstracción de IDs Técnicos
Siguiendo el principio de "Traducción Humano-Máquina", se ha refactorizado el `ProduccionPlugin` para eliminar la necesidad de que el LLM maneje GUIDs.
- El parámetro `Guid loteId` fue reemplazado por `string nombreGalpon`.
- La resolución técnica se realiza internamente en el plugin mediante la orquestación de queries de MediatR (`ListarGalponesQuery` y `ListarLotesQuery`).

### 3. Orquestación de Consultas para Resolución
El flujo de resolución implementado es:
1. Buscar el `GalponId` correspondiente al nombre proporcionado.
2. Buscar el `LoteId` del lote actualmente **Activo** en dicho galpón.
3. Si alguna de estas condiciones falla, se retorna un mensaje de error descriptivo que el LLM utiliza para informar al usuario final (ej. "No hay lotes activos").

### 4. Cambios en LoteResponse
Se ha modificado el DTO `LoteResponse` en `ListarLotesQuery.cs` para incluir la propiedad `GalponId`, necesaria para realizar el cruce de información en la capa de Aplicación sin acceder directamente a la base de datos.

# BITÁCORA DE ARQUITECTURA - FASE 4: IA Y ORQUESTACIÓN

## Sprint 61: Cerebro Base y Orquestador

### 1. Registro del Kernel en el Contenedor DI
Se ha configurado el registro de Semantic Kernel en la capa de `Infrastructure` (`GalponERP.Infrastructure/DependencyInjection.cs`) para mantener la separación de responsabilidades, ya que la conexión con el modelo local Ollama se considera un detalle de infraestructura.

- **Endpoint:** `http://localhost:11434`
- **Modelo:** `gemma4:e4b`
- **Paquete de Conector:** `Microsoft.SemanticKernel.Connectors.Ollama`

### 2. Implementación de Plugins (Skills)
Se ha creado el primer plugin de dominio en `GalponERP.Application/Agentes/Plugins/ProduccionPlugin.cs`. Este plugin actúa como puente entre el lenguaje natural y los comandos existentes del sistema.

- **Método:** `RegistrarBajasGalpon`
- **Comando Invocado:** `RegistrarMortalidadCommand` via `IMediator`.

### 3. Servicio Orquestador
El `AgenteOrquestadorService` centraliza la lógica de procesamiento. Se encarga de inicializar el historial de chat con un System Prompt descriptivo y habilitar `FunctionChoiceBehavior.Auto()` para permitir que la IA decida cuándo invocar herramientas.

## Sprint 61.5: Resolución Automática de Contexto

### 1. Inyección de Realidad Temporal
Se ha actualizado el `AgenteOrquestadorService` para inyectar dinámicamente la fecha y hora actual en el `SystemMessage`. Esto permite que el LLM entienda referencias relativas como "hoy" o "ayer".

### 2. Abstracción de IDs Técnicos
Siguiendo el principio de "Traducción Humano-Máquina", se ha refactorizado el `ProduccionPlugin` para eliminar la necesidad de que el LLM maneje GUIDs técnicos, reemplazándolos por nombres legibles.

## Sprint 61.6: Resolución Inteligente de Relaciones

### 1. Enriquecimiento del Modelo de Dominio
Se ha añadido la propiedad de navegación `Galpon` a la entidad `Lote` en `GalponERP.Domain/Entities/Lote.cs`. Esto permite realizar consultas con `.Include(l => l.Galpon)` para obtener información relacionada directamente.

### 2. Refactor de Repositorios e infraestructura
- Se actualizó `LoteConfiguration.cs` para mapear explícitamente la relación `HasOne(l => l.Galpon)`.
- El `LoteRepository` en la capa de infraestructura ahora incluye automáticamente la entidad `Galpon` en sus métodos de obtención.

### 3. Mejora en DTOs de Aplicación
Los DTOs `LoteResponse` (Listado) y `LoteDetalleResponse` (Detalle) ahora incluyen `GalponId` y `NombreGalpon`. Esto es crítico para que el `ProduccionPlugin` pueda realizar la inferencia de IDs basándose en nombres legibles extraídos por la IA.

### 4. Lógica de Inferencia Inteligente en Plugins
El `ProduccionPlugin` se ha optimizado con un algoritmo de resolución de dos niveles:
1. **Fallback Automático:** Si el sistema detecta que existe exactamente **un solo lote activo**, el plugin lo selecciona automáticamente (máxima usabilidad).
2. **Coincidencia por Nombre:** Si hay múltiples lotes, busca el galpón cuyo nombre coincida con el texto proporcionado por el usuario.
3. **Gestión de Ambigüedad:** Si no hay coincidencia clara, se retorna una lista de galpones activos al LLM para que este solicite la aclaración necesaria al usuario final.


## Sprint 61.5 y 61.6: Resolución Contextual

### 1. Inyección de Realidad Temporal
El `AgenteOrquestadorService` inyecta `DateTime.Now` en el System Message para que la IA conozca la fecha actual.

### 2. Resolución de Identidades
- Los plugins ahora reciben nombres legibles (ej. "Galpón 1") en lugar de GUIDs.
- Se enriquecieron los DTOs de Lotes con `NombreGalpon`.
- **Inferencia Automática:** Si solo hay 1 lote activo, el plugin lo selecciona por defecto para reducir fricción.

### 3. Hotfix: Serialización Ollama (Fechas)
Se eliminaron los parámetros `DateTime` de los métodos `[KernelFunction]` para evitar errores de serialización JSON en el conector de Ollama.
- La fecha de registro se genera internamente en el plugin usando `DateTime.UtcNow`.
- Los parámetros del plugin se limitan a tipos primitivos (`string`, `int`).

## Sprint 62: Cerebros de Catálogo, Inventario y Finanzas

### 1. Expansión de Plugins (Skills)
Se crearon tres nuevos plugins en `GalponERP.Application/Agentes/Plugins`:
- **CatalogosPlugin.cs:** Proporciona un "Directorio" mental a la IA con productos, proveedores, clientes y categorías.
- **InventarioPlugin.cs:** Gestión de stock, registro de consumo de alimento y proyecciones de niveles críticos.
- **FinanzasPlugin.cs:** Resumen financiero, flujo de caja y registro rápido de gastos operativos.

### 2. Implementación de Regla 8 (Fallback Conversacional)
Todos los nuevos plugins aplican estrictamente la Regla 8:
- Si no se encuentra una coincidencia exacta por nombre (Producto, Galpón, Categoría), el sistema retorna un listado de opciones válidas al LLM para que este consulte al usuario.
- **Inferencia Inteligente:** Si existe un único registro activo, se selecciona automáticamente.

### 3. Registro Dinámico en el Orquestador
Se actualizaron las dependencias en `AgenteOrquestadorService` para registrar dinámicamente los nuevos plugins en el Kernel de Semantic Kernel.


## Sprint 62.5: Hotfix - Toma de Control en C# (Inferencia y Fallback)

### 1. Resolución de Bloqueo en Semantic Kernel
Se detectó que Semantic Kernel interceptaba la falta de parámetros antes de ejecutar el código C#. 
- **Solución:** Se refactorizaron las firmas de los métodos `[KernelFunction]` para que los parámetros de nombres (Galpón, Producto, Categoría) sean opcionales (`string? = null`).
- **Impacto:** Ahora el control pasa siempre al código C#, permitiendo que nuestras reglas de negocio se ejecuten sin interferencia del orquestador.

### 2. Refuerzo de Regla 7 (Inferencia) y Regla 8 (Fallback)
- Se trasladó la lógica de validación al cuerpo de los métodos.
- **Inferencia (Regla 7):** Si el parámetro es nulo o no coincide, pero solo hay una opción válida en el sistema, el plugin la selecciona automáticamente.
- **Fallback (Regla 8):** Si hay múltiples opciones o ninguna coincide, el plugin retorna un string estructurado listando las opciones reales de la base de datos para que el LLM consulte al usuario.
- **Análisis en Cascada:** Los plugins resuelven dependencias de forma secuencial (ej. primero Galpón, luego Lote, luego Producto), deteniendo la ejecución y solicitando aclaración en el punto exacto de ambigüedad.

## Sprint 63: El Operador Maestro (Flujos Complejos y Escritura)

### 1. Nuevo Plugin de Gestión de Catálogos (`GestionCatalogosPlugin.cs`)
Se habilitó la capacidad de **escritura** para la IA, permitiendo flujos de creación dinámica:
- **Creación de Productos:** La IA ahora puede intentar crear productos. Si la categoría o unidad de medida no existe o es ambigua, el código C# devuelve las opciones disponibles para que la IA guíe al usuario.
- **Creación de Categorías, Clientes y Proveedores:** Comandos directos para expandir los catálogos desde lenguaje natural.

### 2. Evolución del System Prompt (Operador Maestro)
Se redefinió la identidad de la IA en `AgenteOrquestadorService`:
- **Persona:** "Operador Maestro de GalponERP", un asistente de élite proactivo.
- **Directiva de No-Detención:** Se instruye a la IA a ejecutar funciones incluso con datos parciales para que la lógica de C# (Reglas 7 y 8) tome el control y proporcione las opciones reales del sistema.
- **Enfoque en Flujos:** Capacidad para encadenar acciones (ej. crear categoría -> crear producto) de forma conversacional.

### 3. Refuerzo de la Regla de Oro de UX
El sistema ahora garantiza que el usuario nunca llegue a un callejón sin salida técnico, siempre recibiendo alternativas válidas extraídas directamente de los Queries del sistema.
## Sprint 64: El Operador Total (Brechas Críticas)

### 1. Módulo de Ventas (`VentasPlugin.cs`)
- Se habilitó la capacidad de registrar ventas parciales directamente desde el chat.
- La IA resuelve automáticamente el cliente y el lote activo, aplicando la Regla 8 en caso de ambigüedad.
- Consulta de ventas recientes para seguimiento financiero.

### 2. Gestión del Ciclo de Vida del Lote (`GestionLotesPlugin.cs`)
- **Apertura de Lotes:** Capacidad para iniciar nuevos ciclos de producción en galpones vacíos, vinculando plantillas sanitarias de forma inteligente.
- **Cierre de Lotes:** Proceso de liquidación que devuelve un resumen detallado de rentabilidad (Utilidad, FCR, Mortalidad).

### 3. Identificación de Brechas Pendientes
Se realizó un escaneo profundo del sistema identificando los siguientes plugins a desarrollar para completar la cobertura:
- **SanidadPlugin:** Control de calendario y aplicación de vacunas.
- **PesajesPlugin:** Seguimiento del crecimiento de las aves.
- **AbastecimientoPlugin:** Carga de stock mediante registro de compras.
- **AuditoriaPlugin:** Consulta de historial de acciones por usuario.

# Bitácora de Arquitectura - Operador Maestro de Sistemas (OMS)

## Registro del Kernel y Plugins
El Kernel de Microsoft Semantic Kernel se ha configurado centralmente para ser inyectado en los servicios necesarios.

### Configuración en el Contenedor DI
El registro principal se realiza en `GalponERP.Infrastructure/DependencyInjection.cs`:

```csharp
services.AddKernel()
        .AddOllamaChatCompletion(
            modelId: "gemma4:e4b",
            endpoint: new Uri("http://localhost:11434")
        );
```

### Registro de Plugins en el Orquestador
Los plugins se registran en el constructor de `AgenteOrquestadorService` en `GalponERP.Application/Agentes/AgenteOrquestadorService.cs`. Se han añadido los siguientes plugins en el Sprint 64:

4. **AbastecimientoPlugin**: Gestiona el ciclo de compras de insumos, permitiendo registrar ingresos de mercadería, consultar cuentas por pagar y registrar pagos a proveedores.
5. **PesajesPlugin**: Permite el seguimiento del crecimiento de las aves mediante el registro de pesajes y la comparación automática contra los estándares de la raza Cobb 500.
6. **ConfiguracionPlugin**: Facilita el ajuste de parámetros técnicos del sistema, como los umbrales de stock mínimo para alertas automáticas.
7. **ReportesPlugin**: Interfaz con el servicio de reportería para generar documentos oficiales (PDF) y resúmenes ejecutivos de inventario y flujo de caja.

## Sprint 64: Consciencia Temporal y Auditoría (IA DETECTIVE) - COMPLETADO
...
(el contenido anterior se mantiene)
...

## Sprint 65: Abastecimiento y Crecimiento (IA LOGÍSTICA) - COMPLETADO
...
(el contenido anterior se mantiene)
...

## Sprint 66: Reportería y Mantenimiento (IA ADMINISTRATIVA) - COMPLETADO

### Mejoras Implementadas:
- **Generación de Reportes PDF**: Integración con `IPdfService` para permitir que la IA genere fichas de liquidación de lotes a pedido del usuario.
- **Auditoría Financiera y de Stock**: Nuevas funciones para obtener resúmenes de flujo de caja y valoración de inventario en tiempo real.
- **Mantenimiento Avanzado de Catálogos**: Extensión de `GestionCatalogosPlugin` para incluir `Actualizar` y `Eliminar` con la **Regla de Seguridad de Confirmación**.
- **Seguridad en Acciones Destructivas**: La IA ahora detecta si una acción es de alto impacto y solicita confirmación explícita del usuario antes de proceder con el comando C#.

---
*Fin de la Fase de Implementación del Operador Maestro de Sistemas (OMS).*

# Documentación de Arquitectura: Operador Inteligente Omnicanal

Esta bitácora documenta las decisiones técnicas y cambios realizados durante la implementación de la Fase de Operador Inteligente Omnicanal (Sprints 67, 68 y 69).

---

## 1. Persistencia y Memoria Conversacional (Sprint 67)

### Objetivo
Transformar el `AgenteOrquestadorService` de un servicio *stateless* a uno *stateful*, permitiendo que la IA recuerde mensajes anteriores dentro de una misma sesión.

### Decisiones Técnicas
*   **Modelo de Datos:** Se crearon las entidades `Conversacion` (agrupador de mensajes por usuario) y `MensajeChat` (registro individual con roles: `user`, `assistant`).
*   **Gestión de Contexto:** Para evitar la saturación de tokens en el LLM, el orquestador recupera únicamente los últimos 10 mensajes de la conversación activa (`ObtenerHistorialChatQuery`) antes de procesar el nuevo prompt.
*   **CQRS:** Se implementaron comandos MediatR para separar la lógica de negocio del servicio de orquestación:
    *   `CrearConversacionCommand`: Inicializa un hilo de chat.
    *   `GuardarMensajeCommand`: Registra interacciones de forma asíncrona.

### Diagrama de Flujo (Lógica)
1. Usuario envía mensaje -> 2. Orquestador busca `conversacionId` -> 3. Carga historial (N=10) -> 4. Inyecta en `ChatHistory` de Semantic Kernel -> 5. Procesa con Plugins -> 6. Guarda respuesta -> 7. Retorna al cliente.

---

## 2. Integración Omnicanal con WhatsApp (Sprint 68)

### Objetivo
Permitir que los operadores en campo interactúen con el sistema mediante la API oficial de WhatsApp Business (Meta).

### Decisiones Técnicas
*   **Identidad de Usuario:** Se añadió el campo `Telefono` a la entidad `Usuario`. El sistema mapea el número remitente del webhook de WhatsApp al `UsuarioId` correspondiente.
*   **Seguridad y Auditoría:** Al recibir un mensaje de WhatsApp, el sistema establece manualmente el `ICurrentUserContext` con los datos del usuario encontrado, asegurando que cualquier acción ejecutada por la IA (ej. registrar mortalidad) quede correctamente auditada en el log del sistema.
*   **Webhook Resilience:** Se implementó `WhatsAppWebhookController` con validación de tokens de Meta y manejo de payloads tipo texto.

---

## 3. Procesamiento de Voz (STT y TTS) (Sprint 69)

### Objetivo
Habilitar comandos de voz tanto en WhatsApp como en la interfaz Web para agilizar la operación manos libres.

### Decisiones Técnicas
*   **Servicio de Voz (`IVoiceService`):** Integración con OpenAI Whisper para transcripción (STT) y OpenAI TTS para síntesis de voz.
*   **Flujo en WhatsApp:** 
    1. Recibe Audio (ID de Media) -> 2. Descarga binario de Meta -> 3. Whisper (STT) -> 4. Orquestador -> 5. Respuesta Texto por WhatsApp.
*   **Flujo en Web (`VoiceChatController`):**
    1. Recibe `FormData` con archivo de audio -> 2. Whisper (STT) -> 3. Orquestador -> 4. OpenAI TTS (Genera audio de respuesta) -> 5. Retorna JSON con texto y audio en Base64.

---

## 4. Configuración del Sistema (Nuevas Secciones)

Para el correcto funcionamiento, se deben configurar las siguientes secciones en el proveedor de configuración (ej. `appsettings.json` o Variables de Entorno):

### WhatsApp (Meta API)
```json
"WhatsApp": {
  "AccessToken": "tu_token_de_acceso",
  "PhoneNumberId": "id_del_numero",
  "VerifyToken": "token_de_verificacion_webhook",
  "ApiVersion": "v18.0"
}
```

### OpenAI (STT / TTS)
```json
"OpenAI": {
  "ApiKey": "tu_api_key_de_openai"
}
```

### CORS
Se habilitó una política CORS "AllowFrontend" en `Program.cs` para permitir peticiones desde `http://localhost:3000`.

---
**Nota:** Todas las acciones destructivas o críticas ejecutadas vía voz o chat siguen requiriendo confirmación explícita según las "Instrucciones del Sistema".

# Bitácora de Arquitectura - Operador Inteligente Omnicanal

## Sprint 70: Seguridad y Confirmación (Regla 10)

### Objetivo
Implementar un mecanismo de seguridad para acciones de alto impacto (Cerrar Lotes, Registrar Gastos > 500, etc.) mediante un flujo de confirmación en dos pasos.

### Decisiones Técnicas
1.  **Persistencia de Intenciones:** Se creó la entidad `IntencionPendiente` para almacenar temporalmente los parámetros de una función que requiere confirmación. Esto asegura que si el sistema se reinicia o el usuario tarda en responder, la intención no se pierda.
2.  **ConfirmacionPlugin:** Se implementó un nuevo plugin que permite al LLM confirmar o cancelar acciones pendientes.
3.  **Interceptación en Orquestador:** El `AgenteOrquestadorService` intercepta una etiqueta especial `[EJECUTAR_PENDIENTE:ID]` generada por el `ConfirmacionPlugin` tras la confirmación del usuario. Esto permite re-ejecutar la función original con los parámetros guardados y forzando `confirmar=true`.
4.  **Uso de System.Text.Json:** Para la serialización de parámetros de funciones en la base de datos, cumpliendo con la ligereza de la aplicación.

## Sprint 71: Resolución de Entidades y Búsqueda Difusa (Regla 8)

### Objetivo
Mejorar la robustez del sistema al identificar entidades (galpones, lotes, productos) mediante lenguaje natural, permitiendo errores tipográficos y ofreciendo sugerencias cuando la coincidencia no es exacta.

### Decisiones Técnicas
1.  **Algoritmo Levenshtein:** Se implementó una extensión de cadena (`StringExtensions.cs`) que calcula la distancia de Levenshtein para determinar la similitud entre dos textos.
2.  **EntityResolver centralizado:** Se creó una clase utilitaria `EntityResolver.cs` que estandariza la lógica de resolución en todos los plugins. Aplica las siguientes reglas en orden:
    *   **Inferencia (Regla 7):** Si solo hay una entidad disponible, se selecciona automáticamente.
    *   **Coincidencia Exacta/Parcial:** Busca coincidencias exactas o de tipo "contiene".
    *   **Búsqueda Difusa (Regla 8):** Si la similitud es > 0.8, selecciona automáticamente. Si es > 0.5, devuelve una lista de sugerencias al LLM.
3.  **Snapshot de Estado (Regla 5):** El `AgenteOrquestadorService` ahora inyecta un resumen de los lotes activos directamente en el System Prompt. Esto permite que la IA sepa qué está pasando en la granja sin tener que llamar a funciones de consulta explícitamente, mejorando la proactividad.

### Beneficios
*   **UX Superior:** El usuario puede escribir "galpon uno" o "galon 1" y el sistema lo entenderá.
*   **Menos Fallos:** Se reducen los errores por IDs inexistentes o nombres mal escritos.
*   **Contexto Inmediato:** La IA puede saludar diciendo: "Veo que tienes 2 lotes activos, ¿en cuál de ellos deseas trabajar hoy?".

## Sprint 72: Gestión de Memoria y Resúmenes (Token Management)

### Objetivo
Optimizar el uso de tokens y mantener la coherencia en conversaciones largas mediante una ventana deslizante de mensajes y resúmenes automáticos persistentes.

### Decisiones Técnicas
1.  **Resumen Persistente:** Se añadieron los campos `ResumenActual` y `UltimoIndiceMensajeResumido` a la entidad `Conversacion`. Esto permite que el asistente "recuerde" lo esencial sin necesidad de procesar cientos de mensajes previos.
2.  **Ventana Deslizante (Sliding Window):** El orquestador ahora solo inyecta los últimos 5 mensajes del historial en el `ChatHistory`, además del resumen acumulado. Esto mantiene el contexto inmediato fresco y el contexto histórico comprimido.
3.  **Resumen Automático via LLM:** Cuando la conversación alcanza un umbral (actualmente configurado al detectar 10 mensajes en la carga), se dispara un proceso en segundo plano que utiliza al propio LLM para generar un resumen conciso de los nuevos puntos clave, integrándolo con el resumen anterior si existía.
4.  **Actualización de Query y Command:** Se modificaron los Handlers de historial para devolver el objeto `HistorialChatResponse` que incluye el resumen, y se creó `ActualizarResumenCommand` para la persistencia.

### Beneficios
*   **Eficiencia:** Reduce drásticamente el consumo de tokens en sesiones largas.
*   **Velocidad:** El modelo responde más rápido al procesar prompts más cortos.
*   **Continuidad:** El sistema puede retomar una conversación días después basándose en el resumen guardado en la base de datos.

## Sprint 73: Vinculación Segura de WhatsApp (Handshake)

### Objetivo
Establecer un mecanismo de identidad seguro para que solo usuarios autorizados de GalponERP puedan operar el sistema vía WhatsApp, vinculando su número mediante un código de un solo uso.

### Decisiones Técnicas
1.  **Handshake de 6 Dígitos:** Se implementó un flujo de vinculación donde el usuario genera un código aleatorio desde el ERP (Web) que expira en 15 minutos.
2.  **Mapeo de Identidad:** El `WhatsAppWebhookController` ahora busca al usuario por el campo `WhatsAppNumero`. Si no lo encuentra, entra en "Modo Vinculación".
3.  **Proceso de Vinculación:** Si un mensaje de un número desconocido contiene exactamente 6 dígitos, el sistema busca un usuario con ese código activo. Al encontrarlo, guarda el número de WhatsApp en el perfil del usuario y completa la vinculación.
4.  **Seguridad por Diseño:** El sistema ignora cualquier comando de números no vinculados, respondiendo únicamente con instrucciones para realizar el handshake. Se añadió la capacidad de desvincular el número desde el perfil.

### Beneficios
*   **Seguridad:** Evita que cualquier persona con el número del bot pueda acceder a datos sensibles.
*   **Trazabilidad:** Todos los mensajes de WhatsApp se asocian correctamente al usuario de Firebase para auditoría y logs.
*   **Autogestión:** El usuario puede cambiar su número vinculado sin intervención del administrador.

## Sprint 74: Notificaciones Proactivas (Background Worker)

### Objetivo
Transformar el sistema de un asistente reactivo a un operador proactivo que monitorea el estado del negocio en tiempo real y notifica anomalías críticas automáticamente a los administradores vía WhatsApp.

### Decisiones Técnicas
1.  **AnalisisDatosJob:** Se implementó un nuevo servicio en segundo plano (`BackgroundService`) que se ejecuta cada 12 horas para analizar indicadores clave de rendimiento (KPIs).
2.  **Detección de Anomalías:** El Job integra múltiples consultas de MediatR para verificar:
    *   **Inventario:** Niveles críticos de alimento (menos de 3 días).
    *   **Mortalidad:** Picos de mortalidad superiores al 2% semanal por lote.
3.  **Generación de Mensajes via IA:** A diferencia de las alertas tradicionales estáticas, el Job utiliza el nuevo método `GenerarMensajeProactivoAsync` del Orquestador. Esto permite que la IA redacte un mensaje contextualizado, profesional y con sugerencias de acción, en lugar de un texto predefinido.
4.  **Omnicanalidad Proactiva:** Las alertas se envían directamente a los números de WhatsApp vinculados de los administradores, utilizando el handshake de seguridad establecido en el sprint anterior.

### Beneficios
*   **Prevención:** Permite actuar antes de que un problema de stock o sanidad se vuelva irreversible.
*   **Inteligencia de Negocio:** La IA actúa como un supervisor 24/7 que resume problemas complejos en mensajes de WhatsApp fáciles de digerir.
*   **Valor Agregado:** El ERP deja de ser una base de datos pasiva para convertirse en un socio activo en la gestión de la granja.

# Bitácora de Arquitectura - Pollos NicoLu

## Sprint 75: Sanidad y Bienestar Animal (Operación Completa)
### Decisiones Técnicas:
1. **Refactorización de SanidadPlugin:** Se integró `EntityResolver` para permitir búsquedas difusas de lotes y productos, cumpliendo con la Regla 6 (Cero Guids en el lenguaje natural).
2. **Registro de Aplicaciones Sanitarias:** Se mejoró `RegistrarAplicacionVacuna` para que pueda buscar actividades pendientes por nombre si no se proporciona un ID, mejorando la experiencia de usuario vía WhatsApp/Voz.
3. **Nueva Entidad RegistroBienestar:** Se creó la entidad `RegistroBienestar` para capturar parámetros ambientales (Temperatura, Humedad) y de consumo (Agua) de forma diaria. Esto permite un monitoreo más granular de la salud del lote más allá de solo la mortalidad.
4. **Persistencia de Bienestar:** Se implementó el patrón repositorio y comando (MediatR) para el manejo de registros de bienestar, asegurando la separación de responsabilidades.

### Nuevas Funcionalidades:
- `ProgramarActividadSanitaria`: Permite al usuario agendar vacunas o tratamientos manualmente desde el chat.
- `RegistrarParametrosBienestar`: Permite reportar condiciones ambientales y consumo de agua.

## Sprint 76: Abastecimiento, Compras y Cuentas por Pagar
### Decisiones Técnicas:
1. **Gestión de Órdenes de Compra (OC):** Se crearon las entidades `OrdenCompra` y `OrdenCompraItem` para separar la intención de compra de la recepción física. Esto permite un control administrativo más robusto.
2. **Flujo de Recepción Integrado:** Se implementó `RecibirOrdenCompraCommand` que realiza tres acciones en una transacción: marca la OC como recibida, crea el registro de `CompraInventario` y actualiza el stock/PPP de los productos.
3. **Control de Vencimientos:** Se añadió el campo `FechaVencimiento` a `CompraInventario` para permitir que la IA alerte sobre deudas próximas a vencer o ya vencidas.
4. **Búsqueda Difusa en Abastecimiento:** Se integró `EntityResolver` en `AbastecimientoPlugin` para resolver nombres de proveedores y productos desde el lenguaje natural.

### Nuevas Funcionalidades:
- `CrearOrdenCompra`: Permite generar pedidos multi-ítem a proveedores.
- `ConsultarOrdenesPendientes`: Lista las OCs que aún no llegan al almacén.
- `RecibirOrdenCompra`: Procesa la entrada de mercadería vinculada a una OC.
- `ConsultarCuentasPorPagar`: Ahora incluye alertas visuales (⚠️, ⏳) basadas en la fecha de vencimiento.

## Sprint 77: Auditoría y Seguridad Administrativa
### Decisiones Técnicas:
1. **Administración de Accesos (AdministracionPlugin):** Se creó el plugin para permitir la gestión de usuarios vía chat. Al ser acciones de alto impacto, tanto el cambio de roles como el bloqueo de usuarios requieren confirmación explícita mediante el sistema de intenciones pendientes.
2. **Reactivación de Entidades (Soft Delete):** Se modificó la entidad base `Entity` para incluir el método `Activar()`, complementando el `Desactivar()` existente, lo cual permite bloquear y desbloquear usuarios sin perder la integridad referencial ni violar la arquitectura.
3. **Consulta en Repositorio con Filtros Ignorados:** Se modificó `IUsuarioRepository` para incluir `ObtenerTodosConInactivosAsync()` y se ajustó `ObtenerPorIdAsync` usando `.IgnoreQueryFilters()` para garantizar que un administrador pueda encontrar y reactivar a un usuario previamente bloqueado (oculto por el filtro global de EF Core).
4. **Identidad del Creador de Gastos (AuditoriaPlugin):** Se añadió el método `ConsultarQuienRegistroGasto`, el cual utiliza la propiedad de auditoría interna `UsuarioId` (o `UsuarioCreacionId`) y cruza la información con la tabla de usuarios para responder de manera humana a preguntas como "¿Quién registró el recibo de luz?".
5. **Snapshot de Estado con Alertas (AgenteOrquestadorService):** Se refactorizó la inyección de contexto en el prompt del sistema. Ahora, la IA consulta proactivamente los últimos registros de `AuditoriaLog` (últimas 24 horas) y se entera si se han producido eventos críticos de seguridad (bloqueos, cambios de rol o eliminaciones), permitiendo que advierta al administrador al iniciar la conversación.

### Nuevas Funcionalidades:
- `ListarUsuarios`: Muestra el estado (Activo/Bloqueado) y los roles de todos los usuarios.
- `CambiarRolUsuario`: Actualiza el nivel de acceso (Admin, SubAdmin, Empleado).
- `CambiarEstadoBloqueoUsuario`: Suspende o reactiva el acceso al sistema para un usuario específico.
- `ConsultarQuienRegistroGasto`: Identifica al autor de un registro financiero utilizando búsqueda difusa sobre la descripción del gasto.

## Sprint 78: Análisis Avanzado y Correlación IA
### Decisiones Técnicas:
1. **AnalisisPlugin y Correlación Cruzada:** Se implementó un plugin de análisis que cruza datos de tres dominios: Inventario (Consumo de alimento), Mortalidad (Bajas acumuladas) y Producción (Lotes). Esto permite identificar qué marcas de alimento están asociadas a menores tasas de mortalidad.
2. **Modo Consultor Experto:** Se creó una función que simula el razonamiento de un consultor avícola, analizando desviaciones en los lotes activos y sugiriendo acciones correctivas proactivas en lugar de solo reaccionar a comandos del usuario.
3. **Optimización de Prompt de Sistema (100% Cobertura):** Se realizó la actualización final del prompt del sistema para consolidar el rol de "Operador Maestro". Ahora el modelo tiene conciencia total de sus capacidades en Sanidad, Abastecimiento, Auditoría, Finanzas y Análisis.
4. **Persistencia de Consultas de Lote:** Se creó `ObtenerMovimientosLoteQuery` para permitir que los plugins extraigan el historial detallado de insumos de un lote específico, facilitando análisis futuros (ej. costo de medicación por lote).

### Nuevas Funcionalidades:
- `AnalizarImpactoAlimentoEnMortalidad`: Reporte comparativo de eficiencia de marcas de alimento.
- `ModoConsultorSugiereMejoras`: Diagnóstico integral de la granja con recomendaciones estratégicas.
- **Prompt Maestro:** Nueva personalidad de la IA orientada a resultados y proactividad total.

# Bitácora de Arquitectura - Fase 3: Robustez de Grado Industrial

## Sprint 79: Estabilización Técnica y Refuerzo de Calidad

### Decisiones Técnicas:
1. **Dependencia de EntityFrameworkCore en Application:** Se agregó el paquete `Microsoft.EntityFrameworkCore` al proyecto `GalponERP.Application` para permitir que las interfaces y los handlers utilicen `DbSet` y métodos de extensión de EF Core sin depender de la implementación concreta de Infrastructure.
2. **Refuerzo de Entidades (Lote):** Se agregó la propiedad calculada `EdadSemanas` a la entidad `Lote` y se incluyó en `LoteResponse`. Esto permite que el Orquestador proporcione información de edad más precisa en el Snapshot de Estado.
3. **Corrección de Constructores (Usuario):** Se unificó el constructor de `Usuario` y el método `ActualizarPerfil` para incluir el campo `Telefono`, asegurando que el Seeder y los Handlers de comandos sean consistentes con la definición de la entidad.
4. **Validación de Nulos en Plugins:** Se implementaron verificaciones de nulidad después de llamadas a `_mediator.Send()` en plugins que recuperan detalles de entidades (ej. `ObtenerProductoPorIdQuery`), evitando excepciones de referencia nula en tiempo de ejecución.
5. **Estandarización de Excepciones en Plugins:** Se envolvieron todas las `[KernelFunction]` en bloques `try-catch` para asegurar que el LLM reciba una respuesta textual controlada ante fallos técnicos, en lugar de una excepción que rompa el flujo de la conversación.
6. **Ampliación de Pruebas Unitarias:** Se crearon suites de pruebas para `OrdenCompra` y `RegistroBienestar` cubriendo lógica de negocio crítica:
   - `OrdenCompra`: Validación de estados, cálculo automático de totales al agregar ítems, y restricciones de modificación.
   - `RegistroBienestar`: Validación de integridad de datos y actualización de parámetros ambientales.

## Sprint 80: Automatización de Alertas (Background Jobs)

### Decisiones Técnicas:
1. **Ampliación de IRegistroBienestarRepository:** Se agregó el método `ObtenerPorLoteAsync` para permitir consultas históricas y de tendencias de bienestar.
2. **Nueva Query de Bienestar:** Se implementó `ObtenerUltimoRegistroBienestarQuery` para facilitar al Orquestador y a los Background Jobs el acceso al estado ambiental más reciente de un lote.
3. **Robustez en AnalisisDatosJob:** Se transformó el job proactivo en un monitor integral que ahora audita:
   - **Finanzas:** Alertas de deudas vencidas o por vencer (ventana de 3 días).
   - **Logística:** Identificación de Órdenes de Compra con más de 7 días de retraso.
   - **Bienestar Animal:** Detección de anomalías térmicas y de consumo hídrico basadas en la edad fisiológica del lote (semanas de vida).
4. **Integración con IA Proactiva:** Todas las anomalías detectadas se consolidan y se envían al `IAgenteOrquestadorService` para que genere un mensaje natural y profesional antes de ser enviado por WhatsApp.

## Sprint 81: Inteligencia de Suministro y Optimización Final

### Decisiones Técnicas:
1. **Algoritmo de Predicción de Stock:** Se implementó `PredecirAgotamientoStockQuery` que utiliza una media móvil de los últimos 14 días para proyectar la fecha exacta de agotamiento de cualquier insumo, permitiendo una planificación logística anticipada.
2. **Poda Agresiva de Contexto:** Se refactorizó `AgenteOrquestadorService` para implementar una ventana deslizante inteligente. Cuando el historial supera los 15 mensajes, se genera automáticamente un resumen técnico y se reinicia la ventana de mensajes recientes (Top 5), optimizando el uso de tokens y manteniendo la relevancia del contexto.
3. **Validación de Consistencia (Cross-Domain):** Se creó un motor de auditoría financiera (`ValidarConsistenciaFinancieraQuery`) que reconcilia en tiempo real los registros de Compras con los movimientos de Inventario y los pagos en Finanzas, alertando sobre discrepancias o falta de documentos de respaldo.
4. **Unificación de Plugins:** Se consolidaron las funciones de inventario y se estandarizaron las respuestas de error en `InventarioPlugin` y `AuditoriaPlugin` para incluir las nuevas capacidades de predicción y auditoría integral.

### Estado Final de la Fase 3:
- El sistema ha alcanzado un nivel de robustez industrial.
- El backend es estable, tipado y cuenta con manejo de excepciones profesional.
- La IA es proactiva, auditable y capaz de auto-gestionar su memoria de largo plazo.
# Documentación de Excelencia Arquitectónica - Pollos NicoLu

## Sprint 82: Centralización de Inteligencia (Snapshot Único)

### Objetivo
Consolidar la "Verdad Única" del sistema en un snapshot dinámico inyectado al Agente Orquestador, eliminando la dispersión de lógica de cálculo y asegurando consistencia absoluta con el Dashboard Web.

### Cambios Realizados

#### 1. Implementación de `ObtenerDashboardSnapshotQuery`
- **Ubicación:** `GalponERP.Application/Dashboard/Queries/ObtenerDashboardSnapshotQuery.cs`
- **Funcionalidad:** Centraliza la recolección de datos de múltiples repositorios:
    - **Producción:** Lotes activos, cantidad de pollos vivos y mortalidad del mes.
    - **Inventario:** Stock de alimento, días restantes de autonomía y alertas de stock mínimo.
    - **Finanzas:** Saldos totales por cobrar (Ventas) y por pagar (Compras), e inversión total acumulada en lotes en curso.
    - **Sanidad:** Conteo de tareas pendientes para el día actual.
    - **Seguridad:** Filtrado de eventos críticos de auditoría en las últimas 24 horas (eliminaciones, cambios de rol, bloqueos).
- **Precisión:** Se utiliza redondeo a 2 decimales para montos financieros y pesos, y 1 decimal para días de stock, asegurando precisión industrial.

#### 2. Refactorización de `AgenteOrquestadorService`
- Se eliminó la lógica manual de construcción del snapshot que residía en el servicio.
- El agente ahora consume directamente el `DashboardSnapshotResponse` vía MediatR.
- Se mejoró el formateo del prompt del sistema con secciones claras (PRODUCCIÓN, INVENTARIO, FINANZAS, SEGURIDAD, SANIDAD) y emojis para resaltar alertas críticas.

### Validación de Consistencia
- El cálculo de `MortalidadMesActual` y `DiasAlimentoRestantes` utiliza exactamente la misma lógica que el Dashboard Web, garantizando que la IA y la interfaz de usuario nunca se contradigan.
- Se corrigió el valor por defecto de días de stock a `999` cuando no hay consumo registrado, manteniendo la paridad con la lógica web.

### Impacto en la UX
La IA ahora tiene visibilidad inmediata de:
- Deudas pendientes con proveedores (CxP).
- Cobros pendientes de clientes (CxC).
- Alertas de seguridad en tiempo real.
- Estado crítico de insumos.

---

## Sprint 84: Cierre del Ciclo Financiero y Auditoría Total

### Objetivo
Culminar la capacidad operativa del Agente para gestionar el ciclo completo de ingresos (Ventas/Cobros) y proporcionar una herramienta de auditoría infalible que garantice la integridad de los datos financieros.

### Cambios Realizados

#### 1. Cierre del Ciclo de Ingresos
- **Gestión de Cobros:** Se integró el comando `RegistrarPagoVenta` en el `VentasPlugin`, permitiendo a la IA registrar pagos recibidos de clientes.
- **Consultas de Cartera:** Se añadió `ConsultarVentasPendientes` para identificar rápidamente cuentas por cobrar (CxC) con alertas de atraso (7 y 15 días).

#### 2. Auditoría Consistente de 360 Grados
- **Expansión de `ValidarConsistenciaFinancieraQuery`:** Se amplió la lógica de validación para incluir el flujo de ventas y cobros, asegurando que:
    - Ninguna venta tenga cobros que excedan su total.
    - Los saldos pendientes coincidan con la diferencia entre el total vendido y los pagos registrados.
- **Interfaz del Agente:** El comando `AuditarConsistenciaIntegral` ahora presenta un reporte estructurado en dos bloques (Abastecimiento y Comercialización), proporcionando una visión clara de la deuda (CxP) y el saldo por cobrar (CxC).

#### 3. Estabilización y Calidad
- **Build de Producción:** Se ejecutó una compilación completa de la solución (`dotnet build`), resolviendo conflictos de nombres y accesos a propiedades en los DTOs del Dashboard.
- **Resolución de Conflictos:** Se refactorizaron los DTOs compartidos para evitar duplicidad de definiciones, asegurando un código limpio y mantenible.

### Resumen de Maestría Operativa (Fase 4 Completada)
El Backend de Pollos NicoLu está ahora **blindado**:
- **Consistencia Total:** La IA, el Dashboard y la base de datos comparten la misma lógica de cálculo.
- **Interacción Robusta:** El sistema es tolerante a errores de usuario gracias al `EntityResolver` mejorado.
- **Control Financiero:** El ciclo de caja está 100% cubierto, desde la compra de insumos hasta el cobro final de ventas.
- **Trazabilidad:** Cada acción es auditable y la IA puede justificar sus respuestas basándose en logs reales.

---
*Fin del Proyecto - Fase: Maestría Operativa*
