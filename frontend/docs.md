# DocumentaciÃ³n: RefactorizaciÃ³n del MÃ³dulo de Operaciones - Lote Detalle

## 1. Resumen de Cambios
Se ha transformado la secciÃ³n de **"OPERACIÃ“N"** de una interfaz de solo registro a un sistema de gestiÃ³n completo (CRUD) con historial filtrable y trazabilidad de inventario.

## 2. Backend (.NET Core)

### A. Nuevos Comandos y Handlers
- **Inventario:**
    - `EliminarMovimientoCommand`: Permite el borrado lÃ³gico (Soft Delete) de movimientos de inventario.
    - `ActualizarMovimientoCommand`: Permite corregir registros de consumo de alimento, recalculando el impacto en costo.
- **Sanidad:**
    - `EliminarBienestarCommand`: Permite eliminar registros de bienestar (agua/clima).
    - `ActualizarBienestarCommand`: Permite editar registros de bienestar existentes.

### B. Controladores
- **InventarioController:** AÃ±adidos endpoints `DELETE /api/Inventario/movimiento/{id}` y `PUT /api/Inventario/movimiento/{id}`.
- **SanidadController:** AÃ±adidos endpoints `DELETE /api/Sanidad/bienestar/{id}` y `PUT /api/Sanidad/bienestar/{id}`.

### C. Dominio
- **MovimientoInventario:** AÃ±adido mÃ©todo `Actualizar()` para permitir la modificaciÃ³n controlada de sus propiedades.

## 3. Frontend (Next.js / React)

### A. Componentes
- **QuickRecordModal:** 
    - Implementado selector dinÃ¡mico de productos para la categorÃ­a "Alimento".
    - Cambiada la lÃ³gica de entrada para Alimento de "Kg" a "Unidades" (ej. Sacos), realizando la conversiÃ³n automÃ¡tica a Kg basada en la propiedad `PesoUnitarioKg` del producto antes de enviar al backend.
    - Soporte completo para el modo ediciÃ³n (`isEditing`).
- **OperationFilters:** 
    - AÃ±adidos selectores de Mes y AÃ±o.
    - Implementada lÃ³gica de limpieza de filtros.
- **OperationHistoryList:** 
    - Mejorada la visualizaciÃ³n para mostrar el nombre del producto en registros de alimento.
    - Habilitadas acciones de Editar y Eliminar para todos los tipos de registros.

### B. Hooks
- **useInventario:** AÃ±adidas mutaciones `eliminarMovimiento` y `actualizarMovimiento`.
- **useSanidad:** AÃ±adidas mutaciones `eliminarBienestar` y `actualizarBienestar`.

## 4. LÃ³gica de Negocio Implementada
1. **Trazabilidad de Alimento:** Ahora el sistema obliga a seleccionar un producto de inventario para registrar el consumo, asegurando que el stock se descuente correctamente y se mantenga el costo promedio ponderado.
2. **Ciclo CRUD:** Todas las operaciones de campo (Bajas, Alimento, Agua, Pesajes) ahora pueden ser corregidas o eliminadas, manteniendo la integridad de los KPIs del lote mediante la invalidaciÃ³n selectiva de cachÃ© de React Query.
3. **Filtrado Temporal:** Por defecto, el historial muestra el mes actual, pero permite navegar por meses/aÃ±os anteriores o filtrar por un dÃ­a especÃ­fico.
