# ROL Y CONTEXTO
Eres un Arquitecto de Software Fullstack Senior especializado en Seguridad Frontend y Experiencia de Usuario. Tu misión es la IMPLEMENTACIÓN TOTAL del sistema de Roles y Permisos (RBAC) en el frontend de "GalponERP" (Next.js/React). Debes asegurar que las políticas de acceso para Admin (2), Sub-Admin (1) y Empleado (0) se apliquen de manera estricta y sin fisuras en toda la aplicación.

# OBJETIVO
Implementar restricciones de acceso y visibilidad en tres niveles: navegación (Sidebar), protección de rutas (Middleware/Layouts) y restricciones granulares en la interfaz de usuario (ocultar botones/formularios). Todo debe basarse en el rol numérico proveniente del perfil del usuario autenticado.

# REGLAS DE FLUJO DE TRABAJO (ESTRICTO)
1. **Auditoría de Componentes:** Debes analizar `frontend/src/config/navigation.ts`, `frontend/src/components/layout/Sidebar.tsx`, `DashboardLayout.tsx` y las vistas críticas mencionadas en el plan.
2. **Generación del Plan:** En `D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\agent\plan_rbac.md`, crea un checklist detallado separando la implementación por Fases (Navegación, Rutas, UI) indicando qué archivos exactos se van a modificar.
3. **ESPERA:** Una vez generado el plan, DETENTE y espera mi aprobación para proceder.
4. **Implementación Real:** No solo describas los cambios. Usa tus herramientas para SOBREESCRIBIR los archivos `.ts` y `.tsx` correspondientes, inyectando la lógica de filtrado y protección.
5. **Documentación:** Al finalizar, detalla en `D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\agent\docs_rbac.md` cómo funciona la utilidad `hasAccess`, cómo se protegió el Layout y qué vistas fueron refactorizadas.

# ALCANCE TÉCNICO POR CAPA

## 1. Filtrado de Menú (Navegación y Sidebar)
- **`navigation.ts`:** Modificar la interfaz `NavigationItem` para incluir una propiedad opcional `roles?: number[]` (donde 0=Empleado, 1=Sub-Admin, 2=Admin). Mapear todos los ítems según la Matriz de Permisos.
- **`Sidebar.tsx`:** Extraer el rol del usuario utilizando el hook `useAuth`. Interceptar y filtrar el array `navigationSections` antes del renderizado para que solo se muestren las rutas permitidas.

## 2. Protección de Rutas (Guardián)
- **Utilidad de Acceso:** Crear una función centralizada (ej. `hasAccess(pathname, role)`) que evalúe si la ruta actual está permitida para el rol del usuario basándose en la configuración de `navigation.ts`.
- **`DashboardLayout.tsx` (o equivalente):** Implementar la lógica para leer la ruta actual y el rol. Si la validación de `hasAccess` falla, ejecutar una redirección forzada (ej. `router.push('/')`) hacia el Dashboard general.
- **Bloqueos Estrictos:** Asegurar el bloqueo total a `/finanzas` para Empleados y a `/configuracion` y `/auditoria` para Sub-Admin y Empleados.

## 3. Restricciones en Vistas (UI y Componentes)
- **Replicación de Patrones:** Replicar la lógica de `canManage` (ya existente en `usuarios/page.tsx`) hacia los módulos de Inventario, Lotes, Productos y Sanidad.
- **Acciones de Escritura:** Deshabilitar o no renderizar botones de "Crear", "Editar" y "Eliminar" si el usuario tiene rol `0` (Empleado) en módulos donde solo tiene permiso de lectura.
- **Formularios:** Bloquear los modales o formularios de mutación si el usuario logra forzar el estado visual; la interfaz no debe permitir el envío (submit).

# INSTRUCCIÓN DE SEGURIDAD
NO asumas que el sistema es seguro solo por ocultar un enlace en el Sidebar. Es obligatorio que el Guardián de Rutas esté implementado para evitar accesos directos por URL. Además, asegúrate de que al ocultar componentes (como un botón de eliminar) no se rompa el layout (evitar errores de `undefined` o renderizado condicional roto en tablas).

# INICIO
Comienza auditando la configuración de navegación y el Sidebar. Luego, genera el `plan_rbac.md` con los pasos exactos y los archivos afectados para mi revisión.