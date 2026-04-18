# Plan de Implementación RBAC - GalponERP Frontend

Este documento detalla los pasos para implementar el Control de Acceso Basado en Roles (RBAC) de manera estricta en el frontend.

## 1. Auditoría y Preparación
- [x] Analizar `AuthContext.tsx` y `useAuth` para identificar el manejo del rol.
- [x] Revisar `navigation.ts` para determinar la estructura de navegación.
- [x] Revisar `DashboardLayout.tsx` para la protección de rutas.
- [x] Identificar la matriz de permisos (Admin=2, Sub-Admin=1, Empleado=0).

## 2. Fase 1: Infraestructura y Utilidades
- [x] **Crear `frontend/src/lib/rbac.ts`**: Centralizar el enum `UserRole` y la función `hasAccess(pathname, role)`.
- [x] **Modificar `frontend/src/config/navigation.ts`**:
    - Importar `UserRole`.
    - Agregar campo `roles?: UserRole[]` a la interfaz `NavigationItem`.
    - Asignar roles permitidos a cada ítem según la matriz de permisos.

## 3. Fase 2: Filtrado de Interfaz Global
- [x] **Modificar `frontend/src/components/layout/Sidebar.tsx`**:
    - Usar `useAuth` para obtener el rol del perfil.
    - Filtrar `navigationSections` antes de renderizar los links.
- [x] **Modificar `frontend/src/components/layout/BottomNav.tsx`**:
    - Aplicar el mismo filtrado para la navegación móvil.

## 4. Fase 3: Protección de Rutas (Seguridad)
- [x] **Modificar `frontend/src/components/layout/DashboardLayout.tsx`**:
    - Implementar un "Guardián de Rutas" en el `useEffect`.
    - Si `hasAccess(pathname, profile.rol)` es falso, redirigir al `/` (Dashboard) y mostrar una alerta (opcional).

## 5. Fase 4: Restricciones Granulares en Vistas (UI)
Se aplicarán restricciones en las siguientes vistas críticas:
- [x] **Usuarios (`/usuarios`)**: Ya implementado (bloqueo total para Empleados).
- [x] **Inventario/Productos/Categorías**:
    - Empleados (0): Solo lectura. Ocultar botones `onAdd`, `onEdit`, `onDelete` en `UniversalGrid`.
- [x] **Lotes/Sanidad**:
    - Empleados (0): Pueden ver y registrar (Lectura/Escritura), pero no eliminar.
- [x] **Finanzas/Gastos**:
    - Empleados (0): Bloqueo total de `/finanzas`. Solo lectura en `/gastos`.
- [x] **Configuración/Auditoría**:
    - Empleados (0) y Sub-Admin (1): Bloqueo total.

## Archivos Modificados:
1. `frontend/src/lib/rbac.ts` (Nuevo)
2. `frontend/src/config/navigation.ts`
3. `frontend/src/components/layout/Sidebar.tsx`
4. `frontend/src/components/layout/BottomNav.tsx`
5. `frontend/src/components/layout/DashboardLayout.tsx`
6. `frontend/src/app/(dashboard)/inventario/page.tsx`
7. `frontend/src/app/(dashboard)/productos/page.tsx`
8. `frontend/src/app/(dashboard)/categorias/page.tsx`
9. `frontend/src/app/(dashboard)/sanidad/page.tsx`
10. `frontend/src/app/(dashboard)/gastos/page.tsx`

---
**ESTADO: FINALIZADO**
