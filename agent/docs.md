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

## Documentación y Pruebas (Swagger UI)
- **Habilitación de Swagger**: Se instaló y configuró `Swashbuckle.AspNetCore` en el proyecto `GalponERP.Api`.
- **Endpoints Visibles**: La interfaz de Swagger UI está disponible en el entorno de desarrollo (`/swagger`) para facilitar la prueba de los endpoints de Planificación y Gestión de Lotes.
- **Configuración .NET 10**: Se integró Swagger con el nuevo soporte nativo de OpenAPI (`AddOpenApi`) asegurando que ambos esquemas estén disponibles.

# Configuración de Seguridad y Despliegue (Setup Crítico)

## Base de Datos PostgreSQL
- Se configuró la cadena de conexión `DefaultConnection` en `appsettings.Development.json`.
- El sistema está listo para ejecutar `dotnet ef database update` una vez que las credenciales locales sean validadas por el usuario.
- Se corrigieron problemas de mapeo en las configuraciones de EF Core para asegurar compatibilidad total con PostgreSQL.

## Seguridad de Credenciales (Firebase Admin)
- **Protección**: Se añadió `firebase-admin.json` al archivo `.gitignore` para prevenir fugas accidentales al repositorio.
- **Inicialización**: Se implementó el código de arranque en `Program.cs` para cargar el SDK de Firebase Admin usando las credenciales del archivo JSON.
- **Compilación**: Verificada la compatibilidad del paquete `FirebaseAdmin` con .NET 10 y el flujo de inyección de dependencias.
