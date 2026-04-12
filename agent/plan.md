# PLAN DE DESARROLLO - FASE 2.1: TRAZABILIDAD OPERATIVA E INVENTARIO

## SPRINT 36: Ejecución Sanitaria Integrada
*Objetivo: Permitir la aplicación de vacunas descontando automáticamente el inventario de la bodega.*
- [x] 1. **Application:** Refactorizar `MarcarVacunaAplicadaCommand` para que reciba `CantidadConsumida` (decimal) y extraiga el `UsuarioId` (del JWT).
- [x] 2. **Application:** En su Handler, usar `IUnitOfWork` para: 1) Cambiar el estado en `CalendarioSanitario` a Aplicado. 2) Generar un `MovimientoInventario` de tipo SALIDA por la dosis consumida.
- [x] 3. **Domain/Application:** Validar que exista stock suficiente antes de aplicar la vacuna. Si no hay, lanzar excepción de negocio.
- [x] 4. **API:** Exponer o actualizar `PATCH /api/calendario/{id}/aplicar`.

## SPRINT 37: Flujo Optimizado de Alimentación Diaria
*Objetivo: Un endpoint dedicado para registrar el consumo diario de alimento que afecte el stock y los costos del lote en una sola transacción.*
- [x] 1. **Application:** Crear `RegistrarConsumoAlimentoCommand` (LoteId, ProductoId, Cantidad, Justificacion, UsuarioId).
- [x] 2. **Application:** El Handler debe usar `IUnitOfWork` para registrar la SALIDA de inventario. El sistema internamente debe calcular los Kg reales (usando `Producto.EquivalenciaEnKg`) para los KPIs biológicos del Lote.
- [x] 3. **API:** Exponer `POST /api/inventario/consumo-diario`.

## SPRINT 38: Visibilidad de Kárdex y Dashboard Global
*Objetivo: Proveer las consultas (Lectura) finales que el Frontend necesita para renderizar la interfaz operativa.*
- [x] 1. **API:** Exponer `GET /api/inventario/producto/{id}/movimientos` para el Kárdex histórico detallado de un solo insumo.
- [x] 2. **API:** Exponer `GET /api/inventario/stock`. Esta consulta debe calcular el saldo neto (Entradas - Salidas) de cada producto y mostrar su `UnidadMedida` para el galponero.
- [x] 3. **API:** Completar `GET /api/dashboard/resumen`. Usar `.AsNoTracking()` para agrupar rápidamente: Aves Vivas Totales, Saldo Total por Cobrar (Ventas) y Tareas Sanitarias para el día actual.