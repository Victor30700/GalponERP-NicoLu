# Historial de Desarrollo - GalponERP

## Fase 1: Fundación y Dominio (Sprints 1 - 8)

### Sprint 1: Domain Layer
- **Value Object `Moneda`**: Encapsulación de montos en Bolivianos con redondeo a 2 decimales.
- **Entidades de Dominio**: Creación de `Entity.cs` (Guid, IsActive), `Galpon`, `Usuario`, `Lote` (lógica de bajas), `Producto`, `MovimientoInventario` y `MortalidadDiaria`.
- **Excepciones**: Implementación de `DomainException`, `LoteDomainException` e `InventarioDomainException`.
- **Interfaces de Repositorio**: Definición de `ILoteRepository` e `IInventarioRepository`.

### Sprint 2: Infrastructure Layer
- **EF Core & DbContext**: Configuración de `GalponDbContext` con PostgreSQL.
- **Fluent API & Soft Delete**: Configuración de tablas, conversiones de enums y filtro global `HasQueryFilter(e => e.IsActive)`.
- **Repositorios Concretos**: Implementación de `LoteRepository` e `InventarioRepository`.
- **Autenticación (Firebase)**: Interfaz `IAuthenticationService` e implementación `FirebaseAuthService` usando `IHttpContextAccessor`.
- **Inyección de Dependencias**: Registro de infraestructura en `DependencyInjection.cs`.

### Sprint 3: Application Layer
- **MediatR & FluentValidation**: Configuración y registro automático de handlers y validadores.
- **Registro Global**: Integración de `AddApplication()` y `AddInfrastructure()` en `Program.cs`.

### Sprint 4: API Layer
- **Global Exception Handler**: Middleware para transformar excepciones en respuestas RFC 7807 (400, 422, 500).
- **ValidationException**: Captura y formateo de errores de `FluentValidation`.

### Sprint 5: Motor Financiero y Costos
- **GastoOperativo**: Entidad y repositorio para registrar gastos asociados a galpones y lotes.
- **CalculadoraCostosLote**: Servicio de dominio para cálculo de FCR (Conversión Alimenticia) y Costo Total.

### Sprint 6: Módulo de Ventas y Cierre de Lote
- **Ventas**: Entidades `Cliente` y `Venta`. Lógica de ventas parciales y validación de stock biológico.
- **Cierre de Lote**: Comando `CerrarLoteCommand` que calcula utilidad neta (Ingresos - Costos) y bloquea futuras operaciones.
- **Patrón Unit of Work**: Implementación de `IUnitOfWork` para asegurar atomicidad.

### Sprint 7: Background Jobs y Testing
- **Notificaciones (Push)**: `INotificationService` implementado con Firebase Admin SDK.
- **Alertas de Inventario**: Query para verificar niveles críticos de alimento (menos de 3 días).
- **Background Job**: `AlertaInventarioJob` que se ejecuta cada 24 horas y notifica a los administradores.
- **Testing Unitario**: Creación de pruebas con xUnit para `Lote`, `CalculadoraCostosLote` y ciclo de vida operativo.

### Sprint 8: Planificación e Inteligencia
- **Calendario Sanitario**: Entidad `CalendarioSanitario` para gestión de vacunas.
- **SimuladorProyeccionLote**: Servicio puro para proyecciones "What-If" de consumo y utilidad.
- **Automatización**: Generación automática de calendario base al crear un nuevo lote (Newcastle, Gumboro).

---

## Fase 2: Exposición de API y Frontend (Sprints 9 - 13)

### Sprint 9: Exposición de API y Seguridad
- **Controladores**: Implementación de `LotesController`, `InventarioController`, `VentasController` y `PlanificacionController`.
- **Seguridad JWT**: Configuración de autenticación con Firebase Auth y soporte para Bearer tokens en Swagger.
- **Endpoints**: Creación de `docs/endpoints.md` para documentación de contratos.

### Sprint 9.3: Seeding Automático y Gestión Total
- **Seeding**: Inicialización automática del "Admin Maestro" en `Program.cs`.
- **CRUD Usuarios y Galpones**: Comandos de edición y Soft Delete. Implementación de `GalponesController`.

### Sprint 10: Inicialización del Frontend
- **Next.js 14**: Configuración con TypeScript, Tailwind CSS y App Router.
- **AuthContext**: Gestión global del estado de autenticación y tokens de Firebase en el cliente.
- **UI Base**: Páginas de Login y Dashboard responsivo.

### Sprint 11: Operaciones Diarias y Auth Backend
- **Login Proxy**: Endpoint `POST /api/auth/login` para facilitar pruebas y Swagger.
- **Mortalidad Avanzada**: Registro de bajas con causas específicas y actualización de contadores de lote.
- **Gastos Operativos**: Endpoints para registro y consulta con filtros por galpón/lote.

### Sprint 12: Perfil de Usuario e Inventario Operativo
- **Perfil Enriquecido**: Adición de Apellidos, Dirección, Profesión y Fecha de Nacimiento a la entidad `Usuario`.
- **Sincronización Firestore**: Persistencia de metadatos de usuario en la colección `users` de Firestore para disponibilidad omnicanal.
- **Inventario**: Registro de movimientos (Kárdex) y consulta de stock actual.

### Sprint 13: Dashboard y Reportes de Rentabilidad
- **Dashboard Query**: Consolidación de aves vivas, mortalidad mensual e inventario crítico en una sola respuesta.
- **Auditoría de Seguridad**: Verificación de que todos los controladores operativos cuentan con `[Authorize]`.

---

## Fase 2.1: Decisiones de Diseño y Arquitectura (14 - 31)

### RBAC y Seguridad (Decisiones 14.1 - 14.3)
- **Enum `RolGalpon`**: Migración de roles basados en string a enums tipados (`Empleado=0`, `SubAdmin=1`, `Admin=2`).
- **Mecánica de Inyección**: Los claims de rol se inyectan tras validar el JWT buscando al usuario en la BD local.

### Módulo de Pesajes y Ventas (Decisiones 15.1 - 16.1)
- **Pesajes**: Registro de peso promedio para cálculo dinámico de FCR: `FCR = Alimento / Incremento Biomasa`.
- **Venta por Peso**: Transición de precio unitario a precio por kilo y peso total, alineado con la industria real.

### Gestión de Identidad y Auditoría (Decisiones 17.1 - 22.2)
- **Endpoint `/me`**: Identidad atómica para el frontend.
- **Auditoría Transparente**: El `UsuarioId` se extrae del JWT y se inyecta en los comandos; el frontend no envía este ID.
- **Inmutabilidad**: Todos los registros operacionales capturan al autor permanentemente.

### Integridad Contable y Cierre (Decisiones 23.1 - 24.1)
- **Flexibilidad `Moneda`**: Permite montos negativos para representar pérdidas reales.
- **Snapshots de Cierre**: Al cerrar un lote se "congelan" los valores de FCR, Costo Total y Utilidad Final.
- **Anulación Segura**: La anulación de una venta devuelve automáticamente los pollos al inventario del lote.
- **Background Jobs**: Implementación de `AlertaSanitariaJob` para escaneo diario de tareas pendientes.

### Modelo SaaS Dinámico (Decisiones 25.1 - 27.2)
- **Catálogos Dinámicos**: Migración de productos estáticos a `CategoriaProducto` y `UnidadMedida` configurables.
- **Ancla Matemática**: Propiedad `EquivalenciaEnKg` en productos para normalizar consumos (ej. Saco 40kg = 40.0).
- **Kárdex Normalizado**: Todos los cálculos de stock y FCR se realizan en base a Kilogramos Reales.

---

## Fase 3: Operaciones Avanzadas y Robustez (Sprints 32 - 60)

### Sprint 32: Finanzas Reales (CxC)
- **Cuentas por Cobrar**: Entidad `PagoVenta` y gestión de saldos pendientes calculados dinámicamente.

### Sprint 33: Sanidad SaaS
- **Plantillas Sanitarias**: Definición de planes de vacunación que se clonan al crear un lote para asegurar trazabilidad inmutable.

### Sprint 34: Ciclo de Vida Avanzado
- **Cancelación y Traslado**: Soporte para cancelar lotes con justificación y mover lotes entre galpones.

### Sprint 36 - 37: Ejecución Integrada
- **Automatización de Stock**: El registro de vacunas y alimentación descuenta automáticamente el stock del Kárdex.
- **Garantía de Stock**: Política de "Cero Negativos" en bodega.

### Sprint 47: Sello de Auditoría
- **Anulación de Pagos**: La anulación de un pago recalcula automáticamente el `EstadoPago` de la venta.

### Sprint 49 - 52: Motor Financiero (Proveedores y PPP)
- **Proveedores y Compras**: Gestión de deudas (Cuentas por Pagar) y vinculación de compras al stock físico.
- **Costeo PPP**: Implementación del Precio Promedio Ponderado para valoración real de bodega y cierres contables exactos.
- **Inteligencia Predictiva**: Algoritmo de proyección de agotamiento de stock basado en consumo histórico.

### Sprint 56 - 58: Reportes y Configuración
- **QuestPDF**: Generación de "Fichas de Liquidación de Lote" profesionales en PDF.
- **Tenant Settings**: Configuración global de la granja (Nombre, NIT, Logotipo) inyectada en reportes.

---

## Fase 4: IA, Orquestación y Omnicanalidad (Sprints 61 - 84)

### Sprint 61 - 63: Operador Maestro (Semantic Kernel)
- **Integración de IA**: Uso de Semantic Kernel con Ollama (Gemma) para comandos en lenguaje natural.
- **Plugins de Dominio**: Traducción de lenguaje humano a comandos MediatR para Producción, Inventario y Finanzas.
- **Inferencia e Inyección Temporal**: La IA conoce la fecha actual y resuelve identidades (ej. "Galpón 1") automáticamente.

### Sprint 64 - 66: Abastecimiento y Auditoría IA
- **IA Logística**: Registro de compras y seguimiento de pesajes vía chat.
- **Auditoría IA**: La IA detecta acciones de alto impacto y solicita confirmación explícita.

### Sprint 67 - 69: Omnicanalidad (WhatsApp y Voz)
- **Memoria Conversacional**: Persistencia de historial de chat para mantener el contexto entre mensajes.
- **WhatsApp Business API**: Integración con Webhooks para operación remota.
- **Voz (STT/TTS)**: Integración con OpenAI Whisper y TTS para comandos manos libres en WhatsApp y Web.

### Sprint 70 - 74: Robustez del Operador Inteligente
- **Regla 10 (Confirmación)**: Persistencia de "Intenciones Pendientes" para acciones críticas.
- **Regla 8 (Búsqueda Difusa)**: Implementación del algoritmo Levenshtein para resolver nombres con errores tipográficos.
- **Poda de Contexto**: Ventana deslizante de mensajes y resúmenes automáticos para ahorro de tokens.
- **Handshake WhatsApp**: Vinculación segura de números telefónicos mediante códigos de 6 dígitos.
- **Notificaciones Proactivas**: Job de análisis que envía alertas de anomalías (mortalidad, stock) vía WhatsApp.

### Sprint 75 - 78: Análisis y Bienestar Animal
- **Registro de Bienestar**: Captura diaria de temperatura, humedad y consumo de agua.
- **Órdenes de Compra**: Separación de intención de compra y recepción física.
- **Modo Consultor**: La IA analiza correlaciones (ej. marca de alimento vs mortalidad) y sugiere mejoras proactivas.

### Sprint 79 - 81: Robustez de Grado Industrial
- **Estabilización**: Tipado fuerte, validación de nulos en plugins y manejo de excepciones profesional.
- **Predicción de Agotamiento**: Algoritmo de media móvil para proyectar fechas de quiebre de stock.
- **Auditoría Transversal**: Reconciliación de pagos vs compras vs inventario.

### Sprint 82 - 84: Excelencia Arquitectónica y Cierre
- **Snapshot Único**: Inyección de un resumen dinámico (`DashboardSnapshot`) al Agente, asegurando que la IA y el Dashboard Web manejen exactamente los mismos datos.
- **Cierre Financiero**: Gestión completa de cobros y cuentas por cobrar vía chat.
- **Maestría Operativa**: El sistema alcanza un estado blindado con consistencia total, trazabilidad absoluta y control financiero 100% cubierto.

---
*Fin del Historial de Desarrollo - Proyecto GalponERP*
