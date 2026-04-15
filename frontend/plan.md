# Plan de Trabajo: Refactorización Sección "Operación" (CRUD + Historial)

Este plan detalla los pasos para transformar la sección de **Operación** en la vista de detalle de lote, permitiendo la visualización histórica, filtrado y gestión completa (CRUD) de Bajas, Alimento, Agua y Pesajes.

## 1. Ajustes en el Backend (Infraestructura de API)

Para habilitar el CRUD completo, se deben asegurar/implementar los siguientes endpoints:

### 1.1 Mortalidad
*   **DELETE `/api/Mortalidad/{id}`**: Implementar el método en `MortalidadController` (el comando `EliminarMortalidadCommand` ya existe en Application).

### 1.2 Sanidad (Agua/Bienestar)
*   **GET `/api/Sanidad/lote/{loteId}`**: Crear endpoint para listar el historial de bienestar de un lote.
*   **DELETE `/api/Sanidad/bienestar/{id}`**: Implementar eliminación de registros de bienestar (opcional, pero recomendado para CRUD).
*   *Nota:* El `POST` actual ya maneja actualizaciones si la fecha coincide, pero se requiere un `PUT` explícito si se desea editar por ID.

### 1.3 Inventario (Alimento)
*   **GET `/api/Inventario/lote/{loteId}/movimientos`**: Crear query para filtrar movimientos de salida (consumo) de un lote específico.
*   **PUT `/api/Inventario/movimiento/{id}`**: Permitir corrección de cantidad/fecha.
*   **DELETE `/api/Inventario/movimiento/{id}`**: Permitir anulación de consumo accidental.

## 2. Servicios y Hooks (Frontend)

### 2.1 Crear `useMortalidad.ts`
*   `mortalidad`: Query `GET /api/Mortalidad/lote/{loteId}`.
*   `registrarMortalidad`: Mutation `POST /api/Mortalidad`.
*   `actualizarMortalidad`: Mutation `PUT /api/Mortalidad/{id}`.
*   `eliminarMortalidad`: Mutation `DELETE /api/Mortalidad/{id}`.

### 2.2 Expandir `useSanidad.ts`
*   `historialBienestar`: Query `GET /api/Sanidad/lote/{loteId}`.
*   `eliminarBienestar`: Mutation `DELETE /api/Sanidad/bienestar/{id}`.

### 2.3 Expandir `useInventario.ts`
*   `movimientosLote`: Query `GET /api/Inventario/lote/{loteId}/movimientos`.
*   `actualizarConsumo`: Mutation `PUT /api/Inventario/movimiento/{id}`.
*   `eliminarConsumo`: Mutation `DELETE /api/Inventario/movimiento/{id}`.

## 3. Componentes de Interfaz

### 3.1 `OperationHistoryList.tsx` (Nuevo)
*   Componente genérico o especializado para mostrar tablas/listas de registros.
*   Columnas: Fecha, Valor, Unidad, Autor, Acciones (Editar/Eliminar).
*   Integración con `Sonner` para feedback y `SweetAlert2` para confirmación de eliminación.

### 3.2 `OperationFilters.tsx` (Nuevo)
*   Selector de Fecha (Día/Mes/Año).
*   Manejo de estado local para filtrar las queries de React Query.

### 3.3 Refactorizar `QuickRecordModal.tsx`
*   Añadir prop `initialData?: any` para modo edición.
*   Cambiar lógica de `mutationFn` para usar `PUT` si `initialData` existe.
*   Soportar selección de fecha (actualmente usa `new Date()`).

## 4. Implementación en `lotes/[id]/page.tsx`

1.  **Manejo de Estado para Filtros:**
    *   `filterDate`: `Date | null` (por defecto hoy o nulo para mostrar todo).
2.  **Integración de Listas:**
    *   Reemplazar el listado simple de pesajes por una sección de "Historial de Operaciones".
    *   Implementar pestañas internas o un selector para conmutar entre Bajas, Alimento, Agua y Pesajes dentro del historial.
3.  **Lógica de Edición:**
    *   Al hacer clic en "Editar", abrir `QuickRecordModal` con los datos del registro seleccionado.

## 5. Cronograma de Tareas

1.  [x] **Backend:** Implementar endpoints faltantes (Mortalidad DELETE, Sanidad GET, Inventario Lote GET).
2.  [x] **Frontend Hooks:** Crear/Actualizar `useMortalidad`, `useSanidad`, `useInventario`.
3.  [x] **Frontend Components:** Crear `OperationFilters` y `OperationHistoryList`.
4.  [x] **Frontend Modal:** Adaptar `QuickRecordModal` para edición y fechas.
5.  [x] **Integración Final:** Refactorizar la pestaña "Operación" en el detalle del lote.
6.  [x] **Validación:** Pruebas de CRUD completo para cada tipo de registro.

---
**Nota sobre Tipado:** Se utilizarán las interfaces definidas en `endpoints.md` y se ajustarán los nombres de campos (ej. `cantidadBajas` vs `cantidad`) según corresponda en cada operación.
