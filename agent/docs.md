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
