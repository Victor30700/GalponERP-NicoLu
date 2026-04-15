# Documentación de Cambios: Refactorización de Productos e Inventario (Control en Kg)

## 1. Resumen de Cambios
Se ha modificado la lógica de gestión de productos y consumo de alimento para permitir un control preciso en Kilogramos, manteniendo la integridad del inventario en unidades físicas (sacos/bolsas).

## 2. Backend (API & Application)
- **`RegistrarConsumoAlimentoCommandHandler.cs`**: 
    - Ahora recibe la cantidad en **Kilogramos**.
    - Realiza la conversión automática a unidades: `unidades = kg / pesoUnitarioKg`.
    - Registra el `MovimientoInventario` en unidades físicas para el Kardex.
    - Actualiza el `StockActualKg` del producto basándose en las unidades resultantes.
- **`CrearProductoCommandHandler.cs`**:
    - Permite recibir `EquivalenciaEnKg` (Peso Total Inicial) directamente.
    - Si no se proporciona, se calcula como `StockInicial * PesoUnitarioKg`.

## 3. Frontend (Web Interface)
- **Módulo de Productos**:
    - El formulario de "Nuevo Producto" ahora calcula reactivamente el **Peso Total Inicial (Kg)** en tiempo real al ingresar el Stock Inicial y el Peso por Unidad.
- **Módulo de Lotes (Consumo de Alimento)**:
    - El modal de registro rápido de alimento ahora prioriza la entrada en **Kilogramos**.
    - Muestra el stock disponible tanto en unidades como en Kg totales para mayor claridad del operario.
    - Realiza el cálculo reactivo inverso (Kg -> Unidades) para que el operario sepa cuántos sacos/bolsas se están descontando.

## 4. Validación Realizada
- Se verificó la conversión matemática en el Backend: Consumir 25 Kg de un producto con 50 Kg/unidad resulta en un descuento de 0.5 unidades en el inventario.
- Se validó la reactividad del frontend en la creación de productos.
- Se aseguró que el stock no pueda ser negativo en kilogramos.

---
**Tarea finalizada exitosamente.**
