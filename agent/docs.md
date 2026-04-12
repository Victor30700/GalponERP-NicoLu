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
