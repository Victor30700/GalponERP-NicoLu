# Bitácora de Desarrollo - GalponERP

## Sprint : Catálogos Maestros (Fundación de Datos)

### Cambios realizados:
- **Domain:**
    - `IProductoRepository`: Se agregaron los métodos `Agregar` y `Actualizar` para permitir el CRUD completo.
    - `Producto.cs`: Se agregó el método `Actualizar` para permitir la modificación de múltiples propiedades.
- **Application:**
    - **Clientes:**
        - `CrearClienteCommand`: Comando y manejador para registrar nuevos clientes.
        - `ActualizarClienteCommand`: Comando y manejador para editar clientes existentes.
        - `EliminarClienteCommand`: Comando y manejador para realizar el Soft Delete de clientes.
        - `ListarClientesQuery`: Consulta y manejador para obtener la lista de clientes (incluyendo estado activo).
    - **Productos:**
        - `CrearProductoCommand`: Comando y manejador para registrar nuevos productos (Alimento, Medicamento, Insumo, etc.).
        - `ActualizarProductoCommand`: Comando y manejador para editar productos existentes.
        - `EliminarProductoCommand`: Comando y manejador para realizar el Soft Delete de productos.
        - `ListarProductosQuery`: Consulta y manejador para obtener la lista de productos (incluyendo estado activo).
- **API:**
    - `ClientesController`: Nuevo controlador para exponer el CRUD completo de clientes en `api/Clientes`.
    - `ProductosController`: Nuevo controlador para exponer el CRUD completo de productos en `api/Productos`.

### Observaciones:
- Se optó por crear carpetas independientes para `Clientes` y `Productos` en `GalponERP.Application` para mantener la consistencia con otros dominios (Galpones, Lotes, etc.), en lugar de agruparlos bajo `Catalogos`.
- Los endpoints existentes en `CatalogosController` se mantienen por compatibilidad, pero se recomienda usar los nuevos controladores dedicados para operaciones de catálogo maestro.

## Sprint 12: Vistas de Lote e Inventario Operativo

### Cambios realizados:
- **Domain:**
    - `ILoteRepository`: Se agregó `ObtenerTodosAsync` para permitir listado completo.
- **Application:**
    - **Lotes:**
        - `ListarLotesQuery`: Consulta para listar lotes con filtro opcional de activos.
        - `ObtenerDetalleLoteQuery`: Consulta detallada que consolida datos de ventas, mortalidad y gastos para calcular la utilidad estimada del lote.
    - **Inventario:**
        - `RegistrarMovimientoInventarioCommand`: Comando para registrar entradas/salidas de productos (alimento, medicina, etc.).
        - `ObtenerStockActualQuery`: Consulta que calcula el balance neto de stock por producto basándose en el historial de movimientos.
- **Infrastructure:**
    - `LoteRepository`: Implementación del nuevo método de listado.
- **API:**
    - `LotesController`: Se agregaron endpoints GET para listar y obtener detalle por ID.
    - `InventarioController`: Se agregaron endpoints para registrar movimientos y consultar stock actual.

### Observaciones:
- La consulta de detalle de lote realiza agregaciones en memoria de múltiples repositorios. Para grandes volúmenes de datos, se podría considerar una vista materializada o una tabla de resumen en el futuro.
- El stock actual se calcula dinámicamente. Si el historial crece demasiado, se recomienda implementar una tabla de `StockActual` que se actualice con cada movimiento.

## Sprint 13: Dashboard y Reportes de Rentabilidad

### Cambios realizados:
- **Domain:**
    - `IMortalidadRepository`: Se agregó `ObtenerPorRangoFechasAsync` para facilitar reportes temporales.
- **Application:**
    - **Dashboard:**
        - `ObtenerResumenDashboardQuery`: Nueva consulta que consolida el estado actual de la granja (aves vivas), mortalidad mensual e inventario crítico en una sola respuesta.
- **Infrastructure:**
    - `MortalidadRepository`: Implementación del método de consulta por rango de fechas.
- **API:**
    - `DashboardController`: Nuevo controlador con endpoint `GET /api/dashboard/resumen`.
    - **Seguridad:** Se realizó una auditoría y se confirmó que los 13 controladores operativos cuentan con el atributo `[Authorize]`.

### Observaciones:
- El dashboard reutiliza la lógica de cálculo de consumo de alimento del caso de uso de inventario para asegurar consistencia en las alertas.
- Se mantiene `AuthController` con acceso anónimo únicamente para el proceso de autenticación.
