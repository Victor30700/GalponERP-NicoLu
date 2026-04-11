# PLAN DE DESARROLLO - FASE 1.8: CONSOLIDACIÓN TRANSACCIONAL Y AUDITORÍA

## SPRINT 27: Gestión de Gastos y Trazabilidad de Ventas
*Objetivo: Completar el ciclo de vida de los gastos y auditar ventas por lote.*
- [x] 1. **Gastos:** Implementar `ActualizarGastoOperativoCommand` y `EliminarGastoOperativoCommand` (Soft Delete).
- [x] 2. **Gastos API:** Exponer `PUT /api/gastos/{id}` y `DELETE /api/gastos/{id}`. Asegurar extracción de `UsuarioId` del JWT.
- [x] 3. **Ventas:** Crear query `ObtenerVentasPorLoteQuery` y exponer endpoint `GET /api/ventas/lote/{loteId}`.
- [x] 4. **Auditoría:** Revisar `AnularVentaCommandHandler` para garantizar que la devolución de pollos al lote sea atómica usando `IUnitOfWork`.

## SPRINT 28: Kardex Avanzado y Blindaje de Catálogos
*Objetivo: Reportes administrativos y estandarización de seguridad.*
- [x] 1. **Inventario:** Crear `ObtenerReporteMovimientosQuery` con filtros por `FechaInicio`, `FechaFin` y `CategoriaProductoId`.
- [x] 2. **Inventario API:** Exponer `GET /api/inventario/movimientos/reporte` para análisis de consumo de alimento vs medicinas.
- [x] 3. **Seguridad:** Auditar todos los controladores de Catálogos (`Categorias`, `UnidadesMedida`, `Productos`, `Clientes`) para restringir `POST`, `PUT` y `DELETE` estrictamente a roles `Admin` y `SubAdmin`.
- [x] 4. **Documentación:** Actualizar `docs/endpoints.md` con los nuevos reportes y métodos de gastos.