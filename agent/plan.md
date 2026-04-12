# PLAN DE DESARROLLO - FASE 3.1: TESORERÍA, INTELIGENCIA Y REPORTES
## SPRINT 53: Tesorería - Gestión de Pagos a Proveedores
*Objetivo: Liquidar las cuentas por pagar y trackear la salida de efectivo.*
- [x] 1. **Dominio:** Crear entidad `PagoCompra` (Id, CompraId, Monto, FechaPago, MetodoPago, UsuarioId).
- [x] 2. **Dominio:** Agregar método `RegistrarPago` en `CompraInventario` para actualizar saldos y `EstadoPago` (Pagado, Parcial).
- [x] 3. **Application:** Implementar comando `RegistrarPagoCompraCommand` y query `ListarPagosCompraQuery`.
- [x] 4. **API:** Exponer `POST /api/inventario/compras/{id}/pagos` y `GET /api/inventario/compras/{id}/pagos`.

## SPRINT 54: Eficiencia Biológica Progresiva (FCR en Vivo)
*Objetivo: Monitorear el rendimiento sin esperar al cierre del lote.*
- [x] 1. **Application:** Crear query `ObtenerRendimientoVivoLoteQuery`. Debe cruzar el último pesaje con el costo de alimento acumulado para dar un FCR proyectado y costo por kilo vivo hoy.
- [x] 2. **Analytics:** Crear reporte de `Tendencias de Mortalidad` agrupado por semana de vida del ave.
- [x] 3. **API:** Exponer `GET /api/lotes/{id}/rendimiento-vivo` y `GET /api/mortalidad/lote/{id}/tendencias`.

## SPRINT 55: Cash Flow Proyectado (30/60 Días)
*Objetivo: Visualizar la liquidez futura de la empresa.*
- [x] 1. **Application:** Crear query `ObtenerFlujoCajaProyectadoQuery`. Debe consolidar:
    - (+) Cuentas por Cobrar (Ventas con saldo pendiente).
    - (-) Cuentas por Pagar (Compras con saldo pendiente).
    - (-) Proyección de consumo de alimento del stock actual.
- [x] 2. **API:** Exponer `GET /api/finanzas/flujo-proyectado`.

## SPRINT 56: Conciliación de Almacén y Reportabilidad SaaS
*Objetivo: Sincronizar stock físico vs sistema y generar documentos PDF.*
- [x] 1. **Application:** Implementar comando `RegistrarConciliacionStockCommand` para ajustes masivos tras inventario físico.
- [x] 2. **Infrastructure:** Integrar librería de generación de PDFs (QuestPDF o similar) para la "Ficha de Liquidación de Lote".
- [x] 3. **API:** Exponer `POST /api/inventario/conciliacion` y `GET /api/lotes/{id}/reporte-cierre-pdf`.
