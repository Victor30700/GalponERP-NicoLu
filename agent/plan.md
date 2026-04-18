# Plan de Implementación de Roles y Permisos (RBAC) - Frontend

Este plan detalla las restricciones de acceso y visibilidad para los roles de **Admin**, **Sub-Admin** y **Empleado** en el sistema GalponERP.

## 1. Definición de Roles (Basado en Backend/Hooks)
Los roles están definidos por valores numéricos en el perfil del usuario:
- **Empleado**: `0`
- **Sub-Admin**: `1`
- **Admin**: `2`

---

## 2. Matriz de Permisos por Vista

| Módulo | Vista / Ruta | Admin (2) | Sub-Admin (1) | Empleado (0) |
| :--- | :--- | :---: | :---: | :---: |
| **General** | Dashboard (`/`) | Full | Full | Solo lectura / Dashboard simplificado |
| | Chat con IA (`/chat`) | Sí | Sí | Sí |
| **Producción** | Lotes (`/lotes`) | Full | Full | Ver y registrar registros diarios |
| | Sanidad (`/sanidad`) | Full | Full | Ver y registrar aplicaciones |
| | Galpones (`/galpones`) | Full | Full | Ver solamente |
| | Planificación (`/planificacion`) | Full | Full | Ver solamente |
| **Inventario** | Inventario (`/inventario`) | Full | Full | Ver y registrar consumos |
| | Productos (`/productos`) | Full | Full | Ver solamente |
| | Categorías (`/categorias`) | Full | Full | Ver solamente |
| | Unidades (`/unidades-medida`) | Full | Full | Ver solamente |
| | Proveedores (`/proveedores`) | Full | Full | Ver solamente |
| **Finanzas** | Ventas (`/ventas`) | Full | Full | Ver y registrar |
| | Gastos (`/gastos`) | Full | Full | Solo lectura |
| | Finanzas (`/finanzas`) | Full | Full | **Oculto** |
| | Clientes (`/clientes`) | Full | Full | Ver y registrar |
| **Administración** | Usuarios (`/usuarios`) | Full | Full (Ver/Editar) | **Oculto** |
| | Auditoría (`/auditoria`) | Full | **Oculto** | **Oculto** |
| | Plantillas (`/plantillas`) | Full | Full | Ver solamente |
| | Configuración (`/configuracion`) | Full | **Oculto** | **Oculto** |
| **Soporte** | Ayuda (`/ayuda`) | Sí | Sí | Sí |

---

## 3. Estrategia de Implementación

### Fase 1: Filtrado de Menú (Sidebar)
Modificar `frontend/src/config/navigation.ts` para incluir un campo opcional de roles permitidos en la interfaz `NavigationItem`.
Actualizar `frontend/src/components/layout/Sidebar.tsx` para filtrar los elementos del menú antes de renderizarlos.

### Fase 2: Protección de Rutas (Middleware/Layout)
Implementar una verificación en `frontend/src/components/layout/DashboardLayout.tsx` o crear un componente `ProtectedRoute` que redirija al Dashboard si el usuario intenta acceder a una ruta para la cual no tiene el rol necesario.

### Fase 3: Restricciones en Vistas (UI)
En páginas como Inventario, Lotes y Usuarios:
- Ocultar botones de "Eliminar" para el rol **Empleado**.
- Deshabilitar formularios de edición o creación si el rol no tiene permisos de escritura.
- Ejemplo: En `usuarios/page.tsx` ya existe una verificación para `canManage` (Admin/SubAdmin). Replicar este patrón en otros módulos.

---

## 4. Pasos Detallados

1.  **Actualizar `navigation.ts`**:
    - Añadir `roles?: UserRole[]` a `NavigationItem`.
    - Asignar los roles correspondientes a cada ítem.
2.  **Actualizar `Sidebar.tsx`**:
    - Obtener el rol del `profile` de `useAuth`.
    - Filtrar `navigationSections` basándose en el rol.
3.  **Implementar Guardián de Rutas**:
    - Crear una función de utilidad `hasAccess(pathname, role)` para centralizar la lógica.
4.  **Refactorizar Vistas Críticas**:
    - **Finanzas**: Bloquear acceso total a Empleados.
    - **Configuración/Auditoría**: Bloquear acceso a Sub-Admin y Empleados.
    - **Inventario/Productos**: Ocultar acciones de modificación para Empleados.

---

## 5. Validación
- Iniciar sesión con una cuenta de cada rol.
- Verificar que el menú lateral solo muestre lo permitido.
- Intentar entrar por URL directa a rutas restringidas y confirmar la redirección/bloqueo.
- Verificar que las acciones de escritura (POST/PUT/DELETE) estén ocultas para roles no autorizados.
