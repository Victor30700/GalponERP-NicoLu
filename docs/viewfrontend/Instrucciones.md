# GUÍA MAESTRA DE INGENIERÍA FRONTEND Y SISTEMA DE DISEÑO - GALPON ERP

Este documento es la referencia oficial y estricta para el desarrollo del frontend de GalponERP. Define la arquitectura, el sistema de diseño visual (UI/UX), el contrato técnico de datos y el plan de ejecución para un sistema **100% adaptable, inmersivo y de alto rendimiento**.

---

## 1. SISTEMA DE DISEÑO Y UI/UX (DARK THEME & CINEMATIC)

El sistema debe transmitir modernidad, robustez y ser amable con la vista durante largas jornadas de uso. **Queda estrictamente prohibido el uso de fondos blancos puros.**

### 1.1 Paleta de Colores (Deep Dark UI)
- **Backgrounds (Fondos):** Tonos oscuros profundos para evitar la fatiga visual. Uso de `slate-950` (#020617) para el fondo principal y `slate-900` (#0f172a) para tarjetas y modales.
- **Acentos y Primarios:** Tonos esmeralda (salud/crecimiento) y ámbar (granos/producción) para botones de acción principal, gráficos y alertas.
- **Textos:** Tonos gris claro (`gray-300` a `gray-100`) para excelente contraste sin deslumbrar.
- **Superficies:** Paneles con sutiles bordes translúcidos (`border-white/10`) y efectos de desenfoque (*Glassmorphism*) en elementos flotantes.

### 1.2 Experiencia de Inicio de Sesión (Login Impactante)
- **Fondo Dinámico:** Carrusel progresivo (*crossfade* suave) de imágenes fotográficas de alta calidad de galpones y aves de corral.
- **Overlay Cinemático:** Una capa de gradiente oscuro sobre las imágenes (`bg-black/60` a `bg-slate-900/90`) para garantizar la total legibilidad del formulario sin perder la textura visual del fondo.
- **Componente Central:** Un panel *Glassmorphism* (desenfoque de fondo tipo `backdrop-blur-md`) centrado, con tipografía limpia y animaciones fluidas de validación.

### 1.3 Navegación y Panel de Administración (Admin Dashboard)
- **Top Navbar:** Minimalista, contiene el breadcrumb dinámico, notificaciones, buscador global y menú de perfil.
- **Sidebar Profesional (PC):** Panel lateral izquierdo colapsable. Íconos limpios (Lucide/Heroicons), indicadores de estado sutiles y transiciones suaves al expandir/contraer.
- **Bottom Navigation (Móvil):** En pantallas pequeñas, el Sidebar desaparece y se reemplaza por una barra de navegación inferior tipo aplicación nativa (Tab Bar) con las acciones principales, apoyada por un menú hamburguesa para opciones secundarias.

---

## 2. FUNDAMENTOS TÉCNICOS Y ARQUITECTURA

### 2.1 Stack Tecnológico
- **Core:** Next.js 14+ (App Router) con TypeScript (Tipado estricto).
- **Estilos:** Tailwind CSS (Mobile-First) + Shadcn/UI (Componentes preconfigurados en modo oscuro por defecto).
- **Animaciones:** Framer Motion (para transiciones de vistas, modales y el fondo del login).
- **Gestión de Datos:** React Query (Sincronización de estado de servidor y caché).
- **Formularios:** React Hook Form + Zod (Validación cliente/servidor paritaria).
- **Autenticación:** Firebase SDK + `AuthContext` (JWT Persistence).

### 2.2 Reglas de Oro del Frontend
1. **Pasividad Matemática:** El frontend **NUNCA** realiza cálculos de negocio (FCR, Utilidad, PPP). Consume datos procesados del Backend para garantizar la **Verdad Única**.
2. **Auditoría Transparente:** No enviar `UsuarioId` en los Requests. El Backend lo extrae de forma segura del JWT.
3. **Manejo de Fechas:** Usar formato estricto ISO 8601 (`YYYY-MM-DDTHH:mm:ssZ`).
4. **Roles (RBAC):** Renderizado condicional estricto. Ocultar/Deshabilitar acciones y rutas según el rol del usuario (`Admin`, `SubAdmin`, `Empleado`).

---

## 3. ESTRATEGIA DE ADAPTABILIDAD (MOBILE-FIRST ABSOLUTO)

El diseño debe verse y sentirse como una aplicación nativa en dispositivos móviles, y expandirse fluidamente en resoluciones de escritorio.

| Elemento | Comportamiento en PC (Desktop) | Comportamiento en Móvil (Smartphones) |
| :--- | :--- | :--- |
| **Tablas de Datos** | DataGrids completos multi-columna con paginación y filtros en cabecera. | Lista de Tarjetas (Cards) detalladas. Swipe actions si es necesario. |
| **Menú Principal** | Sidebar lateral colapsable a la izquierda. | Tab Bar inferior estático + Menú emergente (Drawer). |
| **Modales / Formularios** | Ventanas emergentes (Dialogs) centradas en pantalla. | Bottom Sheets (Bandejas que suben desde abajo) con scroll interno. |
| **Áreas de Toque** | Cursores y hovers precisos. | Zonas táctiles amplias (mínimo 44x44px) para dedos. |

---

## 4. ESPECIFICACIÓN TÉCNICA DE ENDPOINTS (CONTRATO I/O)

*(El Agente debe adherirse estrictamente a estas estructuras para el Data Fetching. Para detalles exhaustivos, consultar la documentación en `endpoints.md`)*

**Módulo: Autenticación y Perfil**
- `POST /api/Auth/login` | **IN:** `{ email, password }` | **OUT:** `{ idToken, refreshToken, expiresIn }`
- `GET /api/Usuarios/me` | **OUT:** `{ id, nombre, apellidos, email, rol, telefono, isActive }`

**Módulo: Producción y Lotes**
- `GET /api/Dashboard/resumen` | **OUT:** `{ totalAvesVivas, totalCxC, totalCxP, inversionLotesActivos, alertasStock, tareasSanitariasHoy }`
- `POST /api/Lotes` | **IN:** `{ galponId, nombreLote, cantidadInicial, costoUnitarioPollito, plantillaId? }`
- `GET /api/Lotes/{id}` | **OUT:** `{ id, fcrActual, mortalidadPorcentaje, costoTotalAcumulado, avesVivas, estado... }`
- `PATCH /api/calendario/{id}/aplicar` (Sin Body).

**Módulo: Inventario y Compras**
- `GET /api/inventario/stock` | **OUT:** `Array<{ productoId, nombre, stockActualUnidades, stockActualKg, unidadMedida }>`
- `POST /api/inventario/consumo-diario` | **IN:** `{ loteId, productoId, cantidad }`
- `POST /api/inventario/compras` | **IN:** `{ proveedorId, productoId, cantidad, precioCompraUnitario, nroFactura, fechaVencimiento? }`

**Módulo: Ventas y Finanzas**
- `POST /api/Ventas/parcial` | **IN:** `{ loteId, clienteId, cantidadPollos, pesoTotalKg, precioPorKg, nota }`
- `GET /api/Finanzas/cuentas-por-cobrar` | **OUT:** `Array<{ clienteNombre, saldoPendiente, ventasVencidas }>`
- `POST /api/Ventas/{id}/pagos` | **IN:** `{ monto, fecha, metodoPago }`

---

## 5. PLAN DE DESARROLLO POR FASES

### Fase 0-1: Fundación, Seguridad y UI Core (Días 1-10)
- Configuración de Next.js, Tailwind, Shadcn/UI (Theme config para tonos oscuros).
- Implementación del sistema dinámico de fondos cinemáticos para el acceso.
- Construcción del Layout Maestro: Sidebar (PC) / Bottom Tab Bar (Móvil).
- Vistas de Login, enrutamiento protegido y Perfil.

### Fase 2: Maestros y Catálogos (Días 11-20)
- CRUDs de Clientes, Proveedores, Productos y Galpones.
- Transformación condicional de UI (Tables en PC -> Cards en Móvil).

### Fase 3: Operaciones de Producción (Días 21-35)
- Construcción del **Centro de Mando del Lote** (Dashboard detallado 360°).
- Tabs responsivos optimizados para uso táctil en el galpón: Registro de bajas, consumo, check-lists sanitarios.
- Integración de gráficos responsivos nativos (Recharts) en esquemas oscuros.

### Fase 4-5: Logística, Finanzas e IA (Días 36-50)
- Formularios multi-paso complejos para Kárdex, Compras y Ventas.
- **Chat IA:** Interfaz estilo mensajería optimizada para dispositivos móviles (entrada de texto y controles de voz).
- Implementación de Modales/Drawers para confirmación de intenciones seguras.

### Fase 6: Auditoría, PWA y Performance (Días 51-60)
- Logs de auditoría visualmente estructurados.
- Configuración de PWA (Manifest, Service Workers) para instalación en pantalla de inicio.
- Optimización de carga de imágenes (fondos dinámicos) y animaciones.

---

## 6. CONTEXTO DEL ENTORNO Y FLUJO DE TRABAJO DEL AGENTE

El Agente IA debe operar respetando estrictamente las siguientes rutas y reglas de ejecución:

### 6.1 Rutas y Accesos
- **Directorio de Trabajo Frontend (Modificable):** `D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\frontend` (Todo el desarrollo de código debe ocurrir aquí).
- **Directorio Raíz del Proyecto (Lectura):** `D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu` (El agente tiene acceso para analizar todo el backend y entender la lógica, **PERO TIENE ESTRICTAMENTE PROHIBIDO MODIFICAR EL CÓDIGO BACKEND**).
- **Contexto del Sistema:** Para entender el propósito global del sistema, el agente puede consultar: `D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\docs\contextProyecto.md`.
- **Detalle de Endpoints:** Para conectar el frontend correctamente sin inventar datos ni rutas, el agente DEBE consultar: `D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\docs\endpoints.md`.

### 6.2 Archivos de Gestión del Agente
- **Plan de Trabajo:** `D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\docs\viewfrontend\plan.md`
- **Documentación de Avances:** `D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\docs\viewfrontend\docs.md`

### 6.3 Protocolo de Ejecución (Pausas Obligatorias)
1. **Inicio:** Al recibir estas instrucciones, el Agente debe generar un plan de trabajo detallado y guardarlo en `plan.md`.
2. **Ejecución por Fases:** El Agente trabajará fase por fase según el plan.
3. **Documentación:** Al finalizar cada fase, el Agente **debe** documentar los cambios realizados, componentes creados y lógica implementada en `docs.md`.
4. **Pausa Obligatoria:** Tras documentar la fase en `docs.md`, el Agente se detendrá por completo y solicitará confirmación explícita del usuario antes de proceder a la siguiente fase o escribir más código.

---
**Instrucción Final para la IA:** Inicia confirmando el entendimiento de estas reglas, rutas y el protocolo de pausas. Inmediatamente después, genera el archivo `plan.md` en la ruta especificada y espera mi aprobación para comenzar con la Fase 0-1.