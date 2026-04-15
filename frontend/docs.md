# Documentación: Refactorización del Módulo de Operaciones - Lote Detalle

## 1. Resumen de Cambios
Se ha transformado la sección de **"OPERACIÓN"** de una interfaz de solo registro a un sistema de gestión completo (CRUD) con historial filtrable y trazabilidad de inventario.

## 2. Backend (.NET Core)

### A. Nuevos Comandos y Handlers
- **Inventario:**
    - `EliminarMovimientoCommand`: Permite el borrado lógico (Soft Delete) de movimientos de inventario.
    - `ActualizarMovimientoCommand`: Permite corregir registros de consumo de alimento, recalculando el impacto en costo.
- **Sanidad:**
    - `EliminarBienestarCommand`: Permite eliminar registros de bienestar (agua/clima).
    - `ActualizarBienestarCommand`: Permite editar registros de bienestar existentes.

### B. Controladores
- **InventarioController:** Añadidos endpoints `DELETE /api/Inventario/movimiento/{id}` y `PUT /api/Inventario/movimiento/{id}`.
- **SanidadController:** Añadidos endpoints `DELETE /api/Sanidad/bienestar/{id}` y `PUT /api/Sanidad/bienestar/{id}`.

### C. Dominio
- **MovimientoInventario:** Añadido método `Actualizar()` para permitir la modificación controlada de sus propiedades.

## 3. Frontend (Next.js / React)

### A. Componentes
- **QuickRecordModal:** 
    - Implementado selector dinámico de productos para la categoría "Alimento".
    - Cambiada la lógica de entrada para Alimento de "Kg" a "Unidades" (ej. Sacos), realizando la conversión automática a Kg basada en la propiedad `EquivalenciaEnKg` del producto antes de enviar al backend.
    - Soporte completo para el modo edición (`isEditing`).
- **OperationFilters:** 
    - Añadidos selectores de Mes y Año.
    - Implementada lógica de limpieza de filtros.
- **OperationHistoryList:** 
    - Mejorada la visualización para mostrar el nombre del producto en registros de alimento.
    - Habilitadas acciones de Editar y Eliminar para todos los tipos de registros.

### B. Hooks
- **useInventario:** Añadidas mutaciones `eliminarMovimiento` y `actualizarMovimiento`.
- **useSanidad:** Añadidas mutaciones `eliminarBienestar` y `actualizarBienestar`.

## 4. Lógica de Negocio Implementada
1. **Trazabilidad de Alimento:** Ahora el sistema obliga a seleccionar un producto de inventario para registrar el consumo, asegurando que el stock se descuente correctamente y se mantenga el costo promedio ponderado.
2. **Ciclo CRUD:** Todas las operaciones de campo (Bajas, Alimento, Agua, Pesajes) ahora pueden ser corregidas o eliminadas, manteniendo la integridad de los KPIs del lote mediante la invalidación selectiva de caché de React Query.
3. **Filtrado Temporal:** Por defecto, el historial muestra el mes actual, pero permite navegar por meses/años anteriores o filtrar por un día específico.
