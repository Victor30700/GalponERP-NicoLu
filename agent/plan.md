# PLAN DE DESARROLLO - FASE 1.9: CORRECCIONES E INTELIGENCIA OPERATIVA

## SPRINT 29: Gestión de Correcciones (Flexibilidad Auditable)
*Objetivo: Permitir la edición y eliminación de registros diarios para corregir errores humanos.*
- [x] 1. **Mortalidad:** Implementar `ActualizarMortalidadCommand` y `EliminarMortalidadCommand` (Soft Delete).
- [x] 2. **Pesajes:** Implementar `ActualizarPesajeCommand` y `EliminarPesajeCommand` (Soft Delete).
- [x] 3. **Lotes:** Implementar `ActualizarLoteCommand` (Permitir editar FechaInicio, CantidadInicial y CostoInicial solo si el lote está Abierto).
- [x] 4. **API:** Exponer los endpoints `PUT` y `DELETE` en `MortalidadController`, `PesajesController` y `LotesController`. Asegurar protección por Roles (Admin/SubAdmin).

## SPRINT 30: Business Intelligence y Finanzas Consolidadas
*Objetivo: Reportes gerenciales que trascienden a un solo lote.*
- [x] 1. **Finanzas:** Crear `ObtenerFlujoCajaEmpresarialQuery` que consolide Ventas y Gastos de TODOS los lotes en un rango de fechas.
- [x] 2. **Producción:** Crear `ObtenerReporteMortalidadTransversalQuery` para analizar causas de muerte en toda la granja.
- [x] 3. **Benchmarking:** Crear `ObtenerComparativaEficienciaGalponesQuery` para medir cuál galpón es más rentable históricamente.
- [x] 4. **API:** Crear `FinanzasController` y añadir reportes avanzados al `DashboardController`.

## SPRINT 31: Gestión de Estado y Auditoría Log (Governance)
*Objetivo: Control total sobre el cierre de ciclos y rastro de actividad.*
- [x] 1. **Lotes:** Implementar `ReabrirLoteCommand` (Solo Admin). Debe limpiar los campos de Snapshot (`FCRFinal`, `UtilidadFinal`, etc.) y poner el lote en estado `Abierto`.
- [x] 2. **Auditoría:** Crear entidad `AuditoriaLog` (UsuarioId, Accion, Entidad, Fecha, DetallesJSON).
- [x] 3. **Middleware/Behavior:** Implementar un `AuditoriaBehavior` en MediatR que registre automáticamente en la tabla de Logs cualquier comando de tipo `PUT` o `DELETE`.
- [x] 4. **API:** Exponer `GET /api/auditoria/logs` (Solo Admin).
- [x] 5. **Documentación:** Actualizar `endpoints.md` con el contrato final de la Fase 1.9.

**FASE 1.9 COMPLETADA.** 🚀