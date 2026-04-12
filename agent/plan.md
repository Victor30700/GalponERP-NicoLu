# PLAN DE DESARROLLO - FASE 2.4: CIERRE FINANCIERO Y KARDEX AVANZADO

## SPRINT 45: Polímero de Ventas y CRM Básico
*Objetivo: Permitir la edición segura de ventas, exponer recibos individuales y el historial del cliente.*
- [x] 1. **Ventas (Queries):** Verificar que `ObtenerVentaPorIdQuery` esté correctamente expuesto en `GET /api/ventas/{id}`.
- [x] 2. **Ventas (Commands):** Implementar `ActualizarVentaCommand`. Permitir editar Peso y Precio. Si cambian, recalcular internamente el `Total` y `SaldoPendiente` usando `IUnitOfWork`.
- [x] 3. **Clientes (CRM):** Crear `ObtenerHistorialClienteQuery` (devuelve la lista de ventas asociadas a un `ClienteId` ordenadas por fecha) y exponer en `GET /api/clientes/{id}/historial`.
- [x] 4. **API:** Exponer `PUT /api/ventas/{id}` (protegido para Admin/SubAdmin).

## SPRINT 46: Auditoría de Inventario (Kárdex Real) y Cierres
*Objetivo: Proveer la hoja de vida matemática de los insumos y reportes financieros por categoría.*
- [x] 1. **Inventario (Kárdex):** Crear `ObtenerKardexProductoQuery`. A diferencia de los movimientos simples, este QueryHandler debe ordenar los movimientos cronológicamente y calcular un `SaldoAcumulado` fila por fila para devolver la trazabilidad exacta.
- [x] 2. **Finanzas:** Crear `ObtenerGastosPorCategoriaQuery`. Debe agrupar y sumar los gastos en un rango de fechas por tipo (Luz, Agua, Sueldos, etc.) para los gráficos del Dashboard. Exponer en `GET /api/finanzas/gastos-por-categoria`.
- [x] 3. **Lotes (Seguridad):** Asegurar que `ReabrirLoteCommand` esté expuesto en `PUT /api/lotes/{id}/reabrir` y restringido ESTRICTAMENTE a rol `Admin`.
- [x] 4. **Documentación:** Actualizar exhaustivamente `docs/endpoints.md` con los JSON de Kárdex, Historial de Cliente y Edición de Ventas.