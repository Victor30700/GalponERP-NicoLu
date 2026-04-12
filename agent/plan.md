# PLAN DE DESARROLLO - FASE 2.0: ERP COMERCIAL SAAS

## SPRINT 32: Finanzas Reales (Cuentas por Cobrar y Pagos)
*Objetivo: Permitir ventas a crédito y amortizaciones parciales para tener un Flujo de Caja verídico.*
- [x] 1. **Dominio:** Crear entidad `PagoVenta` (Id, VentaId, Monto, FechaPago, MetodoPago, UsuarioIdRegistro).
- [x] 2. **Dominio:** Actualizar la entidad `Venta`. Añadir la colección de `Pagos` y un método `RegistrarPago(monto)` que actualice dinámicamente el `EstadoPago` (Pendiente, Parcial, Pagado) y calcule el `SaldoPendiente`.
- [x] 3. **Application:** Crear `RegistrarPagoVentaCommand` y `ObtenerCuentasPorCobrarQuery`.
- [x] 4. **API:** Exponer `POST /api/ventas/{id}/pagos` y `GET /api/finanzas/cuentas-por-cobrar` en los controladores respectivos. Extraer `UsuarioId` del JWT para la auditoría del pago.
- [x] 5. **Infraestructura:** Crear y aplicar migración `AddPagosCuentasPorCobrar`.

## SPRINT 33: Sanidad SaaS (Plantillas Dinámicas)
*Objetivo: Eliminar la lógica hardcodeada de vacunas y permitir que cada granja configure sus programas.*
- [x] 1. **Dominio:** Crear `PlantillaSanitaria` (Nombre, Descripcion) y `ActividadPlantilla` (PlantillaId, TipoActividad, DiaDeAplicacion, ProductoIdRecomendado).
- [x] 2. **Application:** CRUD para Plantillas Sanitarias.
- [x] 3. **Application:** Refactorizar `CrearLoteCommandHandler` para que reciba un `PlantillaSanitariaId` opcional y construya el `CalendarioSanitario` dinámicamente basado en esa plantilla.

## SPRINT 34: Operaciones de Ciclo de Vida Avanzado
*Objetivo: Reflejar eventualidades del mundo real en los galpones.*
- [x] 1. **Application:** Implementar `CancelarLoteCommand` (Estado = Cancelado, requiere justificación, inactiva el calendario).
- [x] 2. **Application:** Implementar `TrasladarLoteCommand` (Cambia el `GalponId` del lote y deja registro en Auditoría).
- [x] 3. **API:** Exponer `POST /api/lotes/{id}/cancelar` and `POST /api/lotes/{id}/trasladar`.


## SPRINT 35: UX, Auditoría y Documentación
*Objetivo: Preparar la API para la integración final y auditoría administrativa.*
- [x] 1. **API:** Añadir filtros (Fecha, Usuario, Entidad) a `ObtenerAuditoriaLogsQuery`.
- [x] 2. **API:** Configurar comentarios XML en Swagger (`GalponERP.Api.csproj` y `Program.cs`) para que el frontend vea la descripción de cada campo.