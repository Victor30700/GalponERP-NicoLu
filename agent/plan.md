# PLAN DE DESARROLLO - FASE 2.3: UX Y FLEXIBILIDAD OPERATIVA (FRONT-READY V2)

## SPRINT 42: Maestros Completos y Sesión de Usuario
*Objetivo: Llenar los huecos CRUD para formularios Frontend y exponer el perfil de sesión.*
- [x] 1. **Clientes y Productos:** Crear `ObtenerClientePorIdQuery` y `ObtenerProductoPorIdQuery`.
- [x] 2. **API Catálogos:** Exponer `GET /api/clientes/{id}`, `DELETE /api/clientes/{id}`, `GET /api/productos/{id}` y `DELETE /api/productos/{id}`. (Asegurar protección por roles).
- [x] 3. **API Auth/Sesión:** Asegurar que existe y está expuesto el endpoint `GET /api/usuarios/me` (o `ObtenerUsuarioActualQuery`) que devuelva los datos del usuario logueado usando `ICurrentUserContext`.

## SPRINT 43: Dashboard Global y Flujo de Compras
*Objetivo: Pantalla de inicio gerencial y registro formal de entrada de mercadería.*
- [x] 1. **Dashboard:** Refactorizar `ObtenerResumenDashboardQuery` para que incluya: Total Pollos Vivos (toda la granja), Inversión Total en Curso (gastos de lotes abiertos), y Alertas de Stock Mínimo (productos por debajo del umbral).
- [x] 2. **Compras (Inventario):** Crear `RegistrarIngresoMercaderiaCommand` (ProductoId, Cantidad, CostoTotalCompra, Proveedor/Nota). Esto debe reemplazar o envolver el registro de movimiento genérico para asignar costos reales.
- [x] 3. **API:** Actualizar `/api/dashboard/resumen` y exponer `POST /api/inventario/compras`.

## SPRINT 44: Flexibilidad del Calendario Sanitario
*Objetivo: Permitir a los granjeros adaptar el calendario a la realidad del clima y los brotes.*
- [x] 1. **Calendario:** Implementar `AgregarActividadManualCommand` (LoteId, TipoActividad, FechaProgramada, ProductoId opcional).
- [x] 2. **Calendario:** Implementar `ReprogramarActividadCommand` (ActividadId, NuevaFecha, Justificacion).
- [x] 3. **API:** Exponer `POST /api/calendario/actividad-manual` y `PUT /api/calendario/{id}/reprogramar`.
- [x] 4. **Documentación:** Actualizar exhaustivamente `docs/endpoints.md` con los nuevos payloads.