# Plan de Trabajo - GalponERP Frontend

Este plan detalla la ejecución del desarrollo del frontend para GalponERP, siguiendo estrictamente la Guía Maestra y los fundamentos técnicos establecidos.

---

## FASE 0-1: FUNDACIÓN, SEGURIDAD Y UI CORE
**Objetivo:** Establecer la arquitectura base, el sistema de diseño "Deep Dark" y el flujo de autenticación seguro.

### Tareas:
- [x] **Configuración de Dependencias:**
  - Instalar `lucide-react`, `framer-motion`, `@tanstack/react-query`, `react-hook-form`, `zod`, `@hookform/resolvers`.
  - Configurar Shadcn/UI adaptado al tema oscuro profundo.
- [x] **Sistema de Diseño (Tailwind Config):**
  - Definición de paleta de colores: `slate-950` (fondo), `slate-900` (tarjetas), Esmeralda (acentos primarios), Ámbar (acentos secundarios).
  - Configurar fuentes y variables CSS para Glassmorphism.
- [x] **Login Cinemático:**
  - Implementar carrusel dinámico de fondos con crossfade.
  - Crear el panel central de login con efecto backdrop-blur (Glassmorphism).
  - Integrar validaciones con Zod y persistencia con Firebase.
- [x] **Layout Maestro Responsivo:**
  - **Desktop:** Sidebar lateral colapsable con navegación jerárquica.
  - **Mobile:** Tab Bar inferior con acciones rápidas y Drawer lateral para opciones secundarias.
  - Implementar Breadcrumbs dinámicos y Top Navbar.
- [x] **Seguridad y Rutas:**
  - Refinar el `AuthContext` para manejo de JWT.
  - Implementar Middleware de protección de rutas y RBAC (Role-Based Access Control).


---

## FASE 2: MAESTROS Y CATÁLOGOS
**Objetivo:** Implementar la gestión de entidades básicas con adaptabilidad Mobile-First.

### Tareas:
- [x] **Componentes de Datos Universales:**
  - Crear `UniversalGrid`: Tabla multi-columna (PC) que se transforma en Lista de Cards (Móvil).
  - Implementar filtros y búsqueda global en el frontend.
- [x] **Gestión de Entidades (CRUDs):**
  - Clientes, Proveedores, Productos y Galpones.
  - Formularios modales (PC) / Bottom Sheets (Móvil).
- [x] **Validación y Feedback:**
  - Toasts de notificación para acciones exitosas/fallidas.

---

## FASE 3: OPERACIONES DE PRODUCCIÓN (CENTRO DE MANDO)
**Objetivo:** Desarrollar el corazón operativo del sistema: el seguimiento de lotes.

### Tareas:
- [x] **Command Center del Lote:**
  - Dashboard 360° del lote activo (FCR, mortalidad, costos acumulados).
  - Gráficos interactivos de crecimiento y consumo (Recharts).
- [x] **Registro Diario Optimizado:**
  - Interfaz táctil para registro de bajas, consumo de alimento y agua.
  - Checklist de tareas sanitarias con acciones de un solo toque (`PATCH /api/calendario/{id}/aplicar`).
- [x] **Gestión de Lotes:**
  - Creación de nuevos lotes integrando plantillas sanitarias.

---

## FASE 4-5: LOGÍSTICA, FINANZAS E IA
**Objetivo:** Manejo de transacciones complejas e integración de asistencia inteligente.

### Tareas:
- [x] **Inventario y Kárdex:**
  - Registro de movimientos de stock (compras y consumos).
  - Alertas visuales de stock bajo.
- [x] **Módulo de Ventas y Pagos:**
  - Venta parcial/total de lotes con cálculo de peso promedio (Verdad Única del Backend).
  - Gestión de cuentas por cobrar y abonos de clientes.
- [x] **Chat IA e Interfaz de Voz:**
  - Interfaz de chat estilo burbuja/mensajería.
  - Integración de API de voz para dictado de registros en campo.

---

## FASE 6: AUDITORÍA, PWA Y PERFORMANCE
**Objetivo:** Finalización profesional, optimización y preparación para uso offline.

### Tareas:
- [x] **Visor de Auditoría:**
  - Línea de tiempo visual de cambios realizados por usuarios.
- [x] **Configuración PWA:**
  - Manifest, iconos y Service Worker para instalación en Android/iOS.
  - Estrategia de caché para operación estable con baja conectividad.
- [x] **Optimización Final:**
  - Lazy loading de componentes pesados y optimización de imágenes cinemáticas.

---

## PROTOCOLO DE PAUSAS OBLIGATORIAS
1. Al finalizar cada fase, se documentará el progreso en `docs.md`.
2. El Agente se detendrá y esperará aprobación explícita en el chat para continuar.
3. Se realizarán pruebas de adaptabilidad (PC/Mobile) antes de dar por cerrada cada tarea.
