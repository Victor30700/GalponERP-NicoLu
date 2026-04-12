# PLAN DE DESARROLLO - FASE 2.5: SELLO DE AUDITORÍA Y CONSISTENCIA FINAL

## SPRINT 47: Gestión Completa de Pagos y Consistencia de Stock
*Objetivo: Permitir la auditoría de abonos, la anulación segura de los mismos y garantizar que el Kárdex y el Stock coincidan matemáticamente.*
- [x] 1. **Finanzas (Queries):** Crear `ObtenerPagosPorVentaQuery` y exponer en `GET /api/ventas/{id}/pagos`.
- [x] 2. **Finanzas (Commands):** Implementar `AnularPagoVentaCommand`. Esta acción (Soft Delete del pago) DEBE usar `IUnitOfWork` para recalcular y actualizar la entidad `Venta` asociada (devolviendo el monto al `SaldoPendiente` y ajustando el `EstadoPago`). Exponer en `DELETE /api/ventas/{ventaId}/pagos/{pagoId}` (Solo Admin).
- [x] 3. **Inventario (Refactor):** Auditar y refactorizar `ObtenerStockActualQueryHandler` para asegurar que utiliza la misma fórmula exacta de sumas y restas con `EquivalenciaEnKg` que el Kárdex del Sprint 46.

## SPRINT 48: Blindaje Contable y Reportes Administrativos
*Objetivo: Evitar cierres de lotes con deudas pendientes y proveer reportes globales de gastos y ajustes.*
- [x] 1. **Lotes (Cierre Estricto):** Modificar `CerrarLoteCommandHandler`. Añadir validación: Si alguna venta del lote tiene `SaldoPendiente > 0`, lanzar `LoteDomainException` ("No se puede cerrar el lote porque existen cuentas por cobrar pendientes").
- [x] 2. **Inventario (Auditoría):** Crear `ObtenerReporteAjustesInventarioQuery` para listar específicamente los movimientos manuales filtrados por `Justificacion`. Exponer en `GET /api/inventario/ajustes`.
- [x] 3. **Finanzas:** Crear un endpoint global de gastos `GET /api/finanzas/gastos` con filtros opcionales (FechaInicio, FechaFin, Categoria).
- [x] 4. **Documentación:** Actualización final y exhaustiva de `docs/endpoints.md`.