# Plan de Trabajo: Implementación Frontend - Blindaje de Proyecto

Este plan detalla las modificaciones necesarias en el Frontend (Next.js) para soportar las nuevas funcionalidades de blindaje implementadas en el Backend (Fases 1-4).

## Fase 1: Blindaje de Seguridad Alimentaria [x]
1.  **Productos:** 
    *   Agregar campo `periodoRetiroDias` (int) en `src/hooks/useProductos.ts` y en el formulario de creación/edición en `src/app/(dashboard)/productos/page.tsx`. [x]
    *   Validar en el formulario que sea un número no negativo. [x]
2.  **Lotes:** 
    *   En la vista de detalle del lote (`src/app/(dashboard)/lotes/[id]/page.tsx`), mostrar un badge prominente de "BLOQUEO SANITARIO" si `fechaFinRetiro` es mayor a la fecha actual. [x]
3.  **Ventas:** 
    *   En el formulario de ventas (`src/app/(dashboard)/ventas/page.tsx`), deshabilitar la selección o mostrar un error si se intenta vender un lote con bloqueo sanitario activo. [x]

## Fase 2: Inteligencia Sanitaria [x]
1.  **Bienestar Animal:** 
    *   Actualizar el hook `useSanidad.ts` para incluir `lecturaMedidor` (decimal). [x]
    *   En el modal de registro rápido (`QuickRecordModal.tsx` o similar), agregar el campo "Lectura de Medidor" cuando el tipo sea 'Agua'. [x]

## Fase 3: Trazabilidad de Inventario [x]
1.  **Categorías:** 
    *   Actualizar `useCategorias.ts` para soportar el enum `TipoCategoria`. [x]
    *   Agregar selector de Tipo en el formulario de categorías (`src/app/(dashboard)/categorias/page.tsx`). [x]
2.  **Compras:** 
    *   Agregar campos `numeroLoteFabricante` y `fechaVencimiento` en el formulario de compras (`src/app/(dashboard)/inventario/page.tsx`). [x]

## Fase 4: Supervisor Virtual [x]
1.  **Dashboard:** 
    *   Actualizar `useDashboard.ts` para traer el `fcrPromedioEmpresa`. [x]
    *   Mostrar el FCR Global en una tarjeta de estadísticas en el Dashboard principal. [x]

## Tareas Transversales [x]
1.  **Tipos:** Sincronizar todas las interfaces de TypeScript con los nuevos DTOs del Backend. [x]
2.  **API:** Actualizar los métodos en `src/lib/api` para incluir los nuevos campos en los payloads de `POST` y `PUT`. [x]
3.  **Formatos:** Asegurar que las fechas de retiro se muestren en formato local (DD/MM/YYYY). [x]

**Estado de Implementación:** Finalizado. [x]
