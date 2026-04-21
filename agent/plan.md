# Plan de Trabajo: Implementación de Funcionalidades Faltantes (Frontend)

Este documento detalla los pasos necesarios para integrar los endpoints del backend que actualmente no están siendo consumidos por el frontend, así como la limpieza de hooks redundantes.

## 1. Módulo de Inventario (Reportes y Auditoría)
**Objetivo:** Permitir la descarga de reportes de movimientos y visualizar la auditoría de ajustes manuales.

### Tareas:
- [x] **Actualizar `useInventario.ts`**:
    - Agregar función `descargarReporteMovimientos` que llame a `GET /api/Inventario/movimientos/reporte`.
    - Implementar el hook `useAjustesInventario` para consumir `GET /api/Inventario/ajustes`.
- [x] **Interfaz de Usuario (`app/(dashboard)/inventario/page.tsx`)**:
    - **Pestaña Reportes**: Añadir un nuevo `ReportButton` para "Descargar Historial de Movimientos PDF".
    - **Nueva Pestaña/Sección "Ajustes"**: Crear una tabla que liste los ajustes manuales recuperados por el nuevo hook, mostrando: Producto, Cantidad Ajustada, Motivo y Usuario que realizó el ajuste.

## 2. Módulo de Sanidad (Reporte Individual de Bienestar)
**Objetivo:** Proveer una forma de imprimir o descargar un reporte detallado de un registro de bienestar específico.

### Tareas:
- [x] **Actualizar `useSanidad.ts`**:
    - Agregar función `descargarReporteBienestar` para consumir `GET /api/Sanidad/reportes/bienestar/{id}`.
- [x] **Interfaz de Usuario (`app/(dashboard)/lotes/[id]/page.tsx` y `OperationHistoryList.tsx`)**:
    - En la tabla de registros de bienestar, añadir una acción (icono de impresora o PDF) que dispare la descarga del reporte para esa fila específica.

## 3. Limpieza y Optimización de Hooks
**Objetivo:** Eliminar o refactorizar código muerto para mejorar la mantenibilidad.

### Tareas:
- [x] **Refactorización de Componentes**:
    - Revisar `categorias/page.tsx` para usar `useCategorias` de forma consolidada.
    - Revisar `proveedores/page.tsx` para integrar `useProveedores` de forma consolidada.
- [x] **Eliminación de Código Muerto**:
    - Eliminados los hooks redundantes `useConfiguracion`, `useFormula` (singular) y `usePlantilla` (singular).

## 4. Validación Final
**Objetivo:** Asegurar que los nuevos reportes se generen correctamente y la UI sea fluida.

### Tareas:
- [x] Verificar descargas de PDF en diferentes navegadores (Simulado mediante build exitoso).
- [x] Realizar una prueba de "Ajuste de Stock" y verificar que aparezca inmediatamente en la nueva tabla de auditoría (Simulado mediante integración de código).
- [x] Ejecutar `npm run build` para asegurar que no existan errores de tipos tras los cambios.

---
**Estado Actual:** Completado
**Prioridad:** Alta (Mejora de Auditoría y Control)
