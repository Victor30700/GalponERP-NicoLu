# PLAN DE DESARROLLO - FASE 1.7: PULIDO EMPRESARIAL Y AUDITORÍA CONTABLE

## SPRINT 21: Deuda Técnica y Clean Code
*Objetivo: Dejar el compilador en 0 warnings y asegurar la autenticación.*
- [x] 1. Solucionar todos los warnings de "posibles nulos" (Nullables) en comandos y entidades (ej. `RegistrarUsuarioCommandHandler`).
- [x] 2. Refactorizar `FirebaseAuthService` y `Program.cs` para reemplazar el método obsoleto `GoogleCredential.FromFile` por el estándar actual recomendado por Google Admin SDK (`GoogleCredential.GetApplicationDefault()` o carga vía JSON dinámico).

## SPRINT 22: Auditoría de Seguridad (Accountability)
*Objetivo: Rastrear exactamente qué empleado realizó cada transacción financiera o de inventario.*
- [x] 1. Dominio: Agregar la propiedad `UsuarioId` (Guid) a las entidades `MovimientoInventario`, `Venta` y `GastoOperativo`.
- [x] 2. Application: Actualizar los comandos (`RegistrarMovimientoInventarioCommand`, `RegistrarVentaParcialCommand`, `RegistrarGastoOperativoCommand`, `RegistrarPesajeCommand`, `RegistrarMortalidadCommand`) para recibir y guardar el `UsuarioId`.
- [x] 3. API: Modificar los Controladores para extraer el `UsuarioId` desde el JWT (`HttpContext.User`) e inyectarlo en los comandos antes de enviarlos a MediatR.
- [x] 4. Infraestructura: Crear la migración `AddAuditoriaUsuarios` y aplicarla.
- [x] 5. Documentación: Actualizar `endpoints.md` para reflejar que el Frontend NO necesita enviar el `UsuarioId`.

## Pendientes
## SPRINT 23: Integridad Contable y Anulaciones
*Objetivo: Congelar datos al cerrar lotes y permitir reversión segura de errores humanos.*
- [x] 1. Dominio: Agregar `EstadoPago` (Enum: `Pagado = 1`, `Pendiente = 2`, `Parcial = 3`) a la entidad `Venta`.
- [x] 2. Dominio: Agregar campos de Snapshot Contable (`FCRFinal`, `CostoTotalFinal`, `UtilidadNetaFinal`, `PorcentajeMortalidadFinal`) a la entidad `Lote`.
- [x] 3. Application: Modificar `CerrarLoteCommandHandler` para calcular y guardar permanentemente estos Snapshots en la BD.
- [x] 4. Application: Crear el caso de uso `AnularVentaCommand` (Marca la venta como `IsActive = false` y devuelve la `CantidadPollos` al lote utilizando `IUnitOfWork`).
- [x] 5. API: Exponer el endpoint `POST /api/ventas/{id}/anular` protegido por roles administrativos.
- [x] 6. Infraestructura: Crear y aplicar migración `AddIntegridadContableYPagos`.

## SPRINT 24: Rendimiento y Proactividad (Escalabilidad)
*Objetivo: Escalar la base de datos y automatizar operaciones operativas.*
- [x] 1. Infraestructura: Configurar Índices (`.HasIndex()`) en `IEntityTypeConfiguration` para las columnas más consultadas: `LoteId`, `ProductoId` y `Fecha` en las tablas de Inventario y Ventas. Crear migración `AddPerformanceIndexes`.
- [x] 2. API/BackgroundJobs: Crear un `IHostedService` o job en segundo plano (`AlertaSanitariaJob`) que se ejecute diariamente para revisar el `CalendarioSanitario` e imprimir en el Logger las vacunas pendientes del día.