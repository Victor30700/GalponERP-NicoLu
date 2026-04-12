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
