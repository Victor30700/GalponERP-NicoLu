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
