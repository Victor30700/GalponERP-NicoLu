# PLAN DE DESARROLLO - GALPON ERP BACKEND

*Nota para el agente: Ejecuta solo un Sprint a la vez cuando el usuario te lo indique.*

## SPRINT 11: Catálogos Maestros (Fundación de Datos)
*Objetivo: Permitir la creación de entidades físicas necesarias para operar.*
- [x] 1. Crear CRUD completo en Application para `Cliente` (Crear, Actualizar, Eliminar - Soft Delete, Listar).
- [x] 2. Crear `ClientesController` exponiendo los endpoints.
- [x] 3. Crear CRUD completo en Application para `Producto` (Nombre, Categoria, UnidadMedida).
- [x] 4. Crear `ProductosController` exponiendo los endpoints.
- [x] 5. Documentar todos los endpoints en `docs/endpoints.md` y actualizar `agent/docs.md`.

## SPRINT 12: Vistas de Lote e Inventario Operativo
*Objetivo: Consultar el estado real de la granja y registrar entradas de stock.*
- [x] 1. Lotes: Crear `ListarLotesQuery` (resumen) y `ObtenerDetalleLoteQuery` (Data financiera e historial).
- [x] 2. Lotes: Exponer endpoints GET en `LotesController`.
- [x] 3. Inventario: Crear `RegistrarMovimientoInventarioCommand` (Entradas/Salidas de sacos, vacunas).
- [x] 4. Inventario: Crear `ObtenerStockActualQuery` (Suma de entradas menos salidas).
- [x] 5. Inventario: Exponer endpoints en `InventarioController`.
- [x] 6. Documentar nuevos endpoints en `docs/endpoints.md` y actualizar `agent/docs.md`.

## SPRINT 13: Dashboard y Reportes de Rentabilidad
*Objetivo: Proveer la data consolidada para la pantalla principal del Gerente (Admin).*
- [x] 1. Crear `ObtenerResumenDashboardQuery` que devuelva: Total pollos vivos, Mortalidad del mes, Alertas de alimento.
- [x] 2. Crear `DashboardController` con endpoint `GET /api/dashboard/resumen`.
- [x] 3. Validar seguridad: Asegurar que todos los controladores tengan `[Authorize]`.
- [x] 4. Documentar endpoint en `docs/endpoints.md` y actualizar `agent/docs.md`.