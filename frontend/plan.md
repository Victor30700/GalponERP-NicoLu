# Plan de Ejecución: Refactorización del Módulo de Operaciones (Lotes)

Este plan detalla los pasos técnicos para transformar la sección de **"OPERACIÓN"** en la vista de detalle del lote en un sistema CRUD completo, con historial filtrable y trazabilidad de inventario.

## 1. Análisis y Preparación (Grounding)
- [x] Investigar entidades de Dominio (`Lote`, `Producto`, `MovimientoInventario`).
- [x] Revisar controladores de backend (`Mortalidad`, `Pesajes`, `Inventario`, `Sanidad`).
- [x] Identificar carencias en el backend (Falta DELETE/PUT para Movimientos de Inventario y Bienestar).
- [x] Revisar hooks existentes en el frontend.

## 2. Implementación en Backend (Cierre de Ciclo CRUD)
Para cumplir con el requerimiento de CRUD completo, se deben añadir los siguientes puntos de acceso:

### A. Inventario (Consumo de Alimento)
- [ ] Crear `EliminarMovimientoCommand` y `ActualizarMovimientoCommand` en `Application/Inventario`.
- [ ] Añadir `DELETE` y `PUT` en `InventarioController` para movimientos.
- [ ] Asegurar que al eliminar/actualizar un movimiento se revierta/actualice el impacto en el stock si es necesario (aunque para salidas/consumo es más simple).

### B. Sanidad (Consumo de Agua / Bienestar)
- [ ] Crear `EliminarBienestarCommand` y `ActualizarBienestarCommand` en `Application/Sanidad`.
- [ ] Añadir `DELETE` y `PUT` en `SanidadController`.

## 3. Refactorización del Frontend

### A. QuickRecordModal (Formularios de Registro)
- [ ] Cargar lista de productos de la categoría "Alimento" usando `useCatalogos`.
- [ ] Implementar selector de producto en el formulario de Alimento.
- [ ] Cambiar la unidad de entrada para Alimento a "Unidades" (ej. Sacos).
- [ ] Calcular `CantidadTotal = Unidades * EquivalenciaEnKg` antes de enviar al backend.
- [ ] Asegurar que el modal maneje correctamente el estado de edición (`isEditing`) para todos los tipos.

### B. OperationFilters (Filtros de Historial)
- [ ] Añadir selectores de Mes y Año.
- [ ] Implementar lógica para filtrar por Mes/Año actual por defecto.

### C. OperationHistoryList (Listado)
- [ ] Asegurar que todos los items muestren la información relevante (Producto en caso de alimento).
- [ ] Configurar las acciones de Editar y Eliminar para que disparen las mutaciones correctas.

### D. LoteDashboard (Página Principal)
- [ ] Implementar las funciones de eliminación para todos los tipos de registros.
- [ ] Refinar el filtrado de la lista basado en los nuevos estados de `OperationFilters`.

## 4. Pruebas y Validación
- [ ] Verificar registro de cada tipo.
- [ ] Verificar edición de cada tipo.
- [ ] Verificar eliminación con confirmación.
- [ ] Validar que el consumo de alimento descuenta correctamente el stock (o al menos registra el movimiento adecuado).
- [ ] Validar filtros temporales.

## 5. Documentación
- [ ] Actualizar `frontend/docs.md` con los cambios realizados.
