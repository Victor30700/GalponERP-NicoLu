# Historial de Desarrollo - GalponERP

## Fase 1: FundaciÃ³n y Dominio (Sprints 1 - 8)

### Sprint 1: Domain Layer
- **Value Object `Moneda`**: EncapsulaciÃ³n de montos en Bolivianos con redondeo a 2 decimales.
- **Entidades de Dominio**: CreaciÃ³n de `Entity.cs` (Guid, IsActive), `Galpon`, `Usuario`, `Lote` (lÃ³gica de bajas), `Producto`, `MovimientoInventario` y `MortalidadDiaria`.
- **Excepciones**: ImplementaciÃ³n de `DomainException`, `LoteDomainException` e `InventarioDomainException`.
- **Interfaces de Repositorio**: DefiniciÃ³n de `ILoteRepository` e `IInventarioRepository`.

### Sprint 2: Infrastructure Layer
- **EF Core & DbContext**: ConfiguraciÃ³n de `GalponDbContext` con PostgreSQL.
- **Fluent API & Soft Delete**: ConfiguraciÃ³n de tablas, conversiones de enums y filtro global `HasQueryFilter(e => e.IsActive)`.
- **Repositorios Concretos**: ImplementaciÃ³n de `LoteRepository` e `InventarioRepository`.
- **AutenticaciÃ³n (Firebase)**: Interfaz `IAuthenticationService` e implementaciÃ³n `FirebaseAuthService` usando `IHttpContextAccessor`.
- **InyecciÃ³n de Dependencias**: Registro de infraestructura en `DependencyInjection.cs`.

### Sprint 3: Application Layer
- **MediatR & FluentValidation**: ConfiguraciÃ³n y registro automÃ¡tico de handlers y validadores.
- **Registro Global**: IntegraciÃ³n de `AddApplication()` y `AddInfrastructure()` en `Program.cs`.

### Sprint 4: API Layer
- **Global Exception Handler**: Middleware para transformar excepciones en respuestas RFC 7807 (400, 422, 500).
- **ValidationException**: Captura y formateo de errores de `FluentValidation`.

### Sprint 5: Motor Financiero y Costos
- **GastoOperativo**: Entidad y repositorio para registrar gastos asociados a galpones y lotes.
- **CalculadoraCostosLote**: Servicio de dominio para cÃ¡lculo de FCR (ConversiÃ³n Alimenticia) y Costo Total.

### Sprint 6: MÃ³dulo de Ventas y Cierre de Lote
- **Ventas**: Entidades `Cliente` y `Venta`. LÃ³gica de ventas parciales y validaciÃ³n de stock biolÃ³gico.
- **Cierre de Lote**: Comando `CerrarLoteCommand` que calcula utilidad neta (Ingresos - Costos) y bloquea futuras operaciones.
- **PatrÃ³n Unit of Work**: ImplementaciÃ³n de `IUnitOfWork` para asegurar atomicidad.

### Sprint 7: Background Jobs y Testing
- **Notificaciones (Push)**: `INotificationService` implementado con Firebase Admin SDK.
- **Alertas de Inventario**: Query para verificar niveles crÃ­ticos de alimento (menos de 3 dÃ­as).
- **Background Job**: `AlertaInventarioJob` que se ejecuta cada 24 horas y notifica a los administradores.
- **Testing Unitario**: CreaciÃ³n de pruebas con xUnit para `Lote`, `CalculadoraCostosLote` y ciclo de vida operativo.

### Sprint 8: PlanificaciÃ³n e Inteligencia
- **Calendario Sanitario**: Entidad `CalendarioSanitario` para gestiÃ³n de vacunas.
- **SimuladorProyeccionLote**: Servicio puro para proyecciones "What-If" de consumo y utilidad.
- **AutomatizaciÃ³n**: GeneraciÃ³n automÃ¡tica de calendario base al crear un nuevo lote (Newcastle, Gumboro).

---

## Fase 2: ExposiciÃ³n de API y Frontend (Sprints 9 - 13)

### Sprint 9: ExposiciÃ³n de API y Seguridad
- **Controladores**: ImplementaciÃ³n de `LotesController`, `InventarioController`, `VentasController` y `PlanificacionController`.
- **Seguridad JWT**: ConfiguraciÃ³n de autenticaciÃ³n con Firebase Auth y soporte para Bearer tokens en Swagger.
- **Endpoints**: CreaciÃ³n de `docs/endpoints.md` para documentaciÃ³n de contratos.

### Sprint 9.3: Seeding AutomÃ¡tico y GestiÃ³n Total
- **Seeding**: InicializaciÃ³n automÃ¡tica del "Admin Maestro" en `Program.cs`.
- **CRUD Usuarios y Galpones**: Comandos de ediciÃ³n y Soft Delete. ImplementaciÃ³n de `GalponesController`.

### Sprint 10: InicializaciÃ³n del Frontend
- **Next.js 14**: ConfiguraciÃ³n con TypeScript, Tailwind CSS y App Router.
- **AuthContext**: GestiÃ³n global del estado de autenticaciÃ³n y tokens de Firebase en el cliente.
- **UI Base**: PÃ¡ginas de Login y Dashboard responsivo.

### Sprint 11: Operaciones Diarias y Auth Backend
- **Login Proxy**: Endpoint `POST /api/auth/login` para facilitar pruebas y Swagger.
- **Mortalidad Avanzada**: Registro de bajas con causas especÃ­ficas y actualizaciÃ³n de contadores de lote.
- **Gastos Operativos**: Endpoints para registro y consulta con filtros por galpÃ³n/lote.

### Sprint 12: Perfil de Usuario e Inventario Operativo
- **Perfil Enriquecido**: AdiciÃ³n de Apellidos, DirecciÃ³n, ProfesiÃ³n y Fecha de Nacimiento a la entidad `Usuario`.
- **SincronizaciÃ³n Firestore**: Persistencia de metadatos de usuario en la colecciÃ³n `users` de Firestore para disponibilidad omnicanal.
- **Inventario**: Registro de movimientos (KÃ¡rdex) y consulta de stock actual.

### Sprint 13: Dashboard y Reportes de Rentabilidad
- **Dashboard Query**: ConsolidaciÃ³n de aves vivas, mortalidad mensual e inventario crÃ­tico en una sola respuesta.
- **AuditorÃ­a de Seguridad**: VerificaciÃ³n de que todos los controladores operativos cuentan con `[Authorize]`.

---

## Fase 2.1: Decisiones de DiseÃ±o y Arquitectura (14 - 31)

### RBAC y Seguridad (Decisiones 14.1 - 14.3)
- **Enum `RolGalpon`**: MigraciÃ³n de roles basados en string a enums tipados (`Empleado=0`, `SubAdmin=1`, `Admin=2`).
- **MecÃ¡nica de InyecciÃ³n**: Los claims de rol se inyectan tras validar el JWT buscando al usuario en la BD local.

### MÃ³dulo de Pesajes y Ventas (Decisiones 15.1 - 16.1)
- **Pesajes**: Registro de peso promedio para cÃ¡lculo dinÃ¡mico de FCR: `FCR = Alimento / Incremento Biomasa`.
- **Venta por Peso**: TransiciÃ³n de precio unitario a precio por kilo y peso total, alineado con la industria real.

### GestiÃ³n de Identidad y AuditorÃ­a (Decisiones 17.1 - 22.2)
- **Endpoint `/me`**: Identidad atÃ³mica para el frontend.
- **AuditorÃ­a Transparente**: El `UsuarioId` se extrae del JWT y se inyecta en los comandos; el frontend no envÃ­a este ID.
- **Inmutabilidad**: Todos los registros operacionales capturan al autor permanentemente.

### Integridad Contable y Cierre (Decisiones 23.1 - 24.1)
- **Flexibilidad `Moneda`**: Permite montos negativos para representar pÃ©rdidas reales.
- **Snapshots de Cierre**: Al cerrar un lote se "congelan" los valores de FCR, Costo Total y Utilidad Final.
- **AnulaciÃ³n Segura**: La anulaciÃ³n de una venta devuelve automÃ¡ticamente los pollos al inventario del lote.
- **Background Jobs**: ImplementaciÃ³n de `AlertaSanitariaJob` para escaneo diario de tareas pendientes.

### Modelo SaaS DinÃ¡mico (Decisiones 25.1 - 27.2)
- **CatÃ¡logos DinÃ¡micos**: MigraciÃ³n de productos estÃ¡ticos a `CategoriaProducto` y `UnidadMedida` configurables.
- **Ancla MatemÃ¡tica**: Propiedad `PesoUnitarioKg` en productos para normalizar consumos (ej. Saco 40kg = 40.0).
- **KÃ¡rdex Normalizado**: Todos los cÃ¡lculos de stock y FCR se realizan en base a Kilogramos Reales.

---

## Fase 3: Operaciones Avanzadas y Robustez (Sprints 32 - 60)

### Sprint 32: Finanzas Reales (CxC)
- **Cuentas por Cobrar**: Entidad `PagoVenta` y gestiÃ³n de saldos pendientes calculados dinÃ¡micamente.

### Sprint 33: Sanidad SaaS
- **Plantillas Sanitarias**: DefiniciÃ³n de planes de vacunaciÃ³n que se clonan al crear un lote para asegurar trazabilidad inmutable.

### Sprint 34: Ciclo de Vida Avanzado
- **CancelaciÃ³n y Traslado**: Soporte para cancelar lotes con justificaciÃ³n y mover lotes entre galpones.

### Sprint 36 - 37: EjecuciÃ³n Integrada
- **AutomatizaciÃ³n de Stock**: El registro de vacunas y alimentaciÃ³n descuenta automÃ¡ticamente el stock del KÃ¡rdex.
- **GarantÃ­a de Stock**: PolÃ­tica de "Cero Negativos" en bodega.

### Sprint 47: Sello de AuditorÃ­a
- **AnulaciÃ³n de Pagos**: La anulaciÃ³n de un pago recalcula automÃ¡ticamente el `EstadoPago` de la venta.

### Sprint 49 - 52: Motor Financiero (Proveedores y PPP)
- **Proveedores y Compras**: GestiÃ³n de deudas (Cuentas por Pagar) y vinculaciÃ³n de compras al stock fÃ­sico.
- **Costeo PPP**: ImplementaciÃ³n del Precio Promedio Ponderado para valoraciÃ³n real de bodega y cierres contables exactos.
- **Inteligencia Predictiva**: Algoritmo de proyecciÃ³n de agotamiento de stock basado en consumo histÃ³rico.

### Sprint 56 - 58: Reportes y ConfiguraciÃ³n
- **QuestPDF**: GeneraciÃ³n de "Fichas de LiquidaciÃ³n de Lote" profesionales en PDF.
- **Tenant Settings**: ConfiguraciÃ³n global de la granja (Nombre, NIT, Logotipo) inyectada en reportes.

---

## Fase 4: IA, OrquestaciÃ³n y Omnicanalidad (Sprints 61 - 84)

### Sprint 61 - 63: Operador Maestro (Semantic Kernel)
- **IntegraciÃ³n de IA**: Uso de Semantic Kernel con Ollama (Gemma) para comandos en lenguaje natural.
- **Plugins de Dominio**: TraducciÃ³n de lenguaje humano a comandos MediatR para ProducciÃ³n, Inventario y Finanzas.
- **Inferencia e InyecciÃ³n Temporal**: La IA conoce la fecha actual y resuelve identidades (ej. "GalpÃ³n 1") automÃ¡ticamente.

### Sprint 64 - 66: Abastecimiento y AuditorÃ­a IA
- **IA LogÃ­stica**: Registro de compras y seguimiento de pesajes vÃ­a chat.
- **AuditorÃ­a IA**: La IA detecta acciones de alto impacto y solicita confirmaciÃ³n explÃ­cita.

### Sprint 67 - 69: Omnicanalidad (WhatsApp y Voz)
- **Memoria Conversacional**: Persistencia de historial de chat para mantener el contexto entre mensajes.
- **WhatsApp Business API**: IntegraciÃ³n con Webhooks para operaciÃ³n remota.
- **Voz (STT/TTS)**: IntegraciÃ³n con OpenAI Whisper y TTS para comandos manos libres en WhatsApp y Web.

### Sprint 70 - 74: Robustez del Operador Inteligente
- **Regla 10 (ConfirmaciÃ³n)**: Persistencia de "Intenciones Pendientes" para acciones crÃ­ticas.
- **Regla 8 (BÃºsqueda Difusa)**: ImplementaciÃ³n del algoritmo Levenshtein para resolver nombres con errores tipogrÃ¡ficos.
- **Poda de Contexto**: Ventana deslizante de mensajes y resÃºmenes automÃ¡ticos para ahorro de tokens.
- **Handshake WhatsApp**: VinculaciÃ³n segura de nÃºmeros telefÃ³nicos mediante cÃ³digos de 6 dÃ­gitos.
- **Notificaciones Proactivas**: Job de anÃ¡lisis que envÃ­a alertas de anomalÃ­as (mortalidad, stock) vÃ­a WhatsApp.

### Sprint 75 - 78: AnÃ¡lisis y Bienestar Animal
- **Registro de Bienestar**: Captura diaria de temperatura, humedad y consumo de agua.
- **Ã“rdenes de Compra**: SeparaciÃ³n de intenciÃ³n de compra y recepciÃ³n fÃ­sica.
- **Modo Consultor**: La IA analiza correlaciones (ej. marca de alimento vs mortalidad) y sugiere mejoras proactivas.

### Sprint 79 - 81: Robustez de Grado Industrial
- **EstabilizaciÃ³n**: Tipado fuerte, validaciÃ³n de nulos en plugins y manejo de excepciones profesional.
- **PredicciÃ³n de Agotamiento**: Algoritmo de media mÃ³vil para proyectar fechas de quiebre de stock.
- **AuditorÃ­a Transversal**: ReconciliaciÃ³n de pagos vs compras vs inventario.

### Sprint 82 - 84: Excelencia ArquitectÃ³nica y Cierre
- **Snapshot Ãšnico**: InyecciÃ³n de un resumen dinÃ¡mico (`DashboardSnapshot`) al Agente, asegurando que la IA y el Dashboard Web manejen exactamente los mismos datos.
- **Cierre Financiero**: GestiÃ³n completa de cobros y cuentas por cobrar vÃ­a chat.
- **MaestrÃ­a Operativa**: El sistema alcanza un estado blindado con consistencia total, trazabilidad absoluta y control financiero 100% cubierto.

---
*Fin del Historial de Desarrollo - Proyecto GalponERP*
