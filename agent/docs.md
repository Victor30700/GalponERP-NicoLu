# Documentación Técnica: Fases 1 a 3 - Modernización, Event-Driven y Arquitectura Headless

Esta documentación detalla la evolución del sistema GalponERP desde un núcleo transaccional básico hacia una arquitectura orientada a eventos, preparada para el escalado SaaS y la integración con Inteligencia Artificial externa.

---

## 🛡️ Fase 1: Blindaje M2M y Seguridad Core (Enterprise Standard)

El objetivo de esta fase fue asegurar el núcleo del sistema y preparar la base para un entorno Multi-Tenant y de automatización externa.

### 1.1 Autenticación Machine-to-Machine (n8n / Integraciones)
- **Implementación:** Se desarrolló el `ApiKeyMiddleware.cs` en la capa de API.
- **Funcionamiento:** Las solicitudes que incluyen el header `x-api-key` son validadas contra una clave segura configurada en el servidor. Al ser exitosa, el middleware inyecta automáticamente un usuario de tipo **"ServiceAccount"** en el `CurrentUserContext`.
- **Beneficio:** Permite que servicios externos como n8n realicen operaciones de escritura manteniendo la trazabilidad completa en la auditoría, identificando claramente qué cambios fueron realizados por la IA.

### 1.2 Autorización Basada en Políticas (Decoupling de Roles)
- **Cambio:** Se migraron todos los controladores de `[Authorize(Roles = "...")]` a `[Authorize(Policy = "...")]`.
- **Razón Técnica:** Los roles hardcodeados limitan la flexibilidad. Las políticas permiten definir requisitos granulares (ej. `RequierePermisoProduccion`) que pueden ser mapeados dinámicamente a diferentes perfiles según la granja (Tenant).
- **Configuración:** Centralizada en `Program.cs`, facilitando auditorías de seguridad rápidas.

### 1.3 Integridad con Concurrencia Optimista Obligatoria
- **Validación:** Se refactorizaron los validadores de `FluentValidation` para exigir la propiedad `Version` (xmin de PostgreSQL) en todos los comandos de actualización (`UpdateLoteCommand`, `ActualizarStockCommand`, etc.).
- **Impacto:** Se garantiza que ninguna operación de edición se procese si los datos en pantalla están desactualizados, evitando la pérdida de información por "última escritura gana".

### 1.4 Soft Delete Global (Seguridad de Datos)
- **Implementación:** Configuración de **Global Query Filters** en el `GalponDbContext`.
- **Detalle:** Todas las consultas a la base de datos aplican automáticamente `WHERE IsActive = true`.
- **Beneficio:** Las eliminaciones lógicas son transparentes para los desarrolladores, eliminando el riesgo de incluir "datos zombis" en reportes financieros o cálculos de inventario por error humano.

---

## ⚡ Fase 2: Optimización de UX y Tiempo Real (Event-Driven Frontend)

Se eliminó la carga innecesaria del servidor y se mejoró la percepción de fluidez del sistema.

### 2.1 Erradicación del Polling Síncrono
- **Acción:** Se eliminaron los `refetchInterval` de los hooks de React Query.
- **Resultado:** Reducción drástica del tráfico de red y carga de CPU en el servidor de base de datos al eliminar peticiones redundantes cada 5 segundos.

### 2.2 Arquitectura SignalR Global
- **Componente:** `SignalRProvider.tsx` en `frontend/src/context/`.
- **Mecánica:** Se mantiene una única conexión persistente. El servidor emite eventos específicos (ej. `LoteActualizado`) y el frontend reacciona invalidando la caché de TanStack Query solo para los datos afectados.
- **Beneficio:** Los cambios realizados por un usuario (o por la IA vía n8n) se reflejan instantáneamente en las pantallas de todos los demás usuarios conectados.

### 2.3 Idempotencia en el Cliente
- **Implementación:** Interceptor en `api.ts`.
- **Detalle:** Inyección automática de un header `X-Idempotency-Key` (UUID v4) en peticiones `POST` y `PUT`.
- **Seguridad:** Previene duplicados accidentales en transacciones críticas (ventas, mortalidad, pagos) en caso de inestabilidad en la red o múltiples clics.

### 2.4 Actualizaciones Optimistas (Optimistic Updates)
- **Alcance:** Implementado en mutaciones de Mortalidad, Pesajes e Inventario.
- **UX:** La interfaz se actualiza asumiendo éxito en la operación. Si el servidor falla, la UI revierte automáticamente al estado anterior (Rollback), eliminando la latencia percibida por el usuario.

---

## 🧹 Fase 3: Arquitectura Headless e Inteligencia de Negocio

Esta fase simplificó el backend delegando la lógica de IA y proporcionó métricas reales de producción.

### 3.1 Transición a Arquitectura Headless (Decoupling Total)
- **Limpieza:** Se eliminaron las dependencias de `SemanticKernel` y se borraron las carpetas de `Agentes` y `Plugins` del backend .NET.
- **Nueva Estrategia:** La orquestación de IA se trasladó formalmente a **n8n**. El backend ahora actúa como un motor de reglas de negocio puro y ligero.
- **Gateway de Integración:** Se creó `IntegrationController.cs` para recibir webhooks de Telegram y WhatsApp, actuando como puente seguro hacia n8n.

### 3.2 Auditoría de Estados Profunda (Entity Diff)
- **Mejora:** Extensión del `SaveChangesAsync` en `GalponDbContext` mediante el uso de `ChangeTracker`.
- **Capacidad:** El sistema ahora no solo guarda quién hizo el cambio, sino que registra un JSON con el **"Antes"** y **"Después"** de cada propiedad modificada.
- **Utilidad:** Auditoría forense completa y capacidad de reconstruir estados históricos de cualquier entidad.

### 3.3 Métricas Reales de Producción (Business Intelligence)
- **Refactorización:** `ListarLotesQueryHandler` pasó de usar placeholders a cálculos algorítmicos reales.
- **Nuevas Métricas Implementadas:**
    - **FCR (ICA):** Eficiencia alimenticia calculada cruzando consumos históricos y biomasa actual (pesajes).
    - **Viabilidad:** Porcentaje de aves sobrevivientes basado en mortalidad acumulada.
    - **Coste por Ave (Real-Time):** Sumatoria de: `Costo Inicial del Pollito` + `Gastos Operativos del Lote` + `Costo de Alimento Consumido` / `Población Actual`.

---

# Documentación Técnica: Fase 5 - Robustez, Persistencia y Pulido de UX

Esta fase elevó la calidad del sistema a un nivel listo para producción, resolviendo riesgos de pérdida de datos en procesos de fondo, asegurando la integridad transaccional y mejorando drásticamente la experiencia del usuario (UX) ante colisiones de datos.

## 1. Persistencia de Infraestructura (Hangfire con PostgreSQL)

Se garantizó que las tareas programadas no se pierdan ante reinicios del servidor o fallos inesperados.

- **Migración de Almacenamiento:** Se reemplazó el almacenamiento en memoria (`MemoryStorage`) por persistencia física en base de datos.
- **Implementación:**
    - Instalación del paquete `Hangfire.PostgreSql`.
    - Configuración en `Program.cs` vinculada a la cadena de conexión `DefaultConnection`.
    - Las tablas de gestión de jobs se crean automáticamente en el esquema de PostgreSQL, permitiendo auditoría de ejecuciones pasadas y reintentos de jobs fallidos.

## 2. Cierre del Ciclo de Concurrencia Optimista (E2E)

Se completó el flujo de protección de datos desde la base de datos hasta la interfaz de usuario.

- **Frontend Interceptor (Global):**
    - Se actualizó `api.ts` con un manejador de respuestas centralizado.
    - Captura automática del error **409 Conflict** y lanzamiento de una excepción `CONCURRENCY_ERROR`.
    - Integración con `sonner` para mostrar Toasts rojos informando: *"Conflicto de edición: Los datos han sido modificados por otro usuario"*.
- **Cobertura Total de Entidades:**
    - Se aplicó el manejo de campo `version` en los formularios y mutaciones de: **Lotes, Productos, Clientes, Proveedores y Categorías**.
    - **Ajuste de Proveedores:** Se corrigió una discrepancia en los campos del formulario (alineando `razonSocial` y `nitRuc` con el backend) y se integró el soporte de versión.
- **Backend Validation:** Se añadieron validaciones de versión en los Handlers de comandos `Actualizar` para las entidades maestras (Clientes, Proveedores, Categorías).

## 3. Idempotencia y Seguridad en Transacciones Sensibles

Se blindaron las operaciones financieras y de inventario contra duplicidad por errores de red o re-envíos accidentales del usuario.

- **Mecanismo `X-Idempotency-Key`:**
    - El Frontend genera un UUID único (`crypto.randomUUID()`) por cada intento de transacción.
    - Este identificador se envía en el header `X-Idempotency-Key`.
    - El `IdempotencyMiddleware` en el Backend intercepta la solicitud; si la llave ya fue procesada, retorna el resultado cacheado sin ejecutar la lógica de negocio nuevamente.
- **Módulos Protegidos:**
    - **Ventas:** Registro de ventas parciales/crédito.
    - **Pagos de Ventas:** Abonos realizados por clientes.
    - **Compras:** Registro de ingresos de mercadería.
    - **Pagos de Compras:** Abonos realizados a proveedores.

## 4. Refuerzo de UX y Feedback en Tiempo Real

Se mejoró la visibilidad de lo que sucede en el sistema mediante notificaciones proactivas.

- **Toasts Globales de SignalR:**
    - Se actualizó el hook `useSignalR.ts` para invocar a `toast()` de `sonner` automáticamente al recibir cualquier evento desde el `NotificationHub`.
    - Se activó el hook en `DashboardLayout.tsx`, asegurando que el socket esté conectado y escuchando desde el momento en que el usuario inicia sesión.
- **Impacto:** Las alertas de "Stock Bajo" o "Mortalidad Elevada" generadas por procesos de fondo ahora aparecen inmediatamente en la pantalla del usuario como notificaciones visuales elegantes.

## 5. Resumen de Cambios Técnicos

### Nuevos Paquetes y Dependencias
- **GalponERP.Api:** `Hangfire.PostgreSql` (v1.21.1)
- **Frontend:** `sonner` (v1.5.0) para notificaciones enriquecidas.

### Contrato de API Actualizado
- Las operaciones `PUT` y `PATCH` ahora requieren el campo `version` en el cuerpo del JSON.
- Las operaciones `POST` sensibles aceptan y procesan el header `X-Idempotency-Key`.

---
*Documentación técnica consolidada por el Agente de Desarrollo - Fases 1 a 5.*

---

# Documentación Técnica: Post-Auditoría (Fases 1 a 4) - Seguridad, Identidad y UX

Esta sección detalla las correcciones arquitectónicas y refinamientos implementados para elevar el sistema a un estándar de producción senior tras la última auditoría técnica.

## 🛡️ FASE 1: Seguridad del Gateway de Integración (Telegram/n8n)
Se implementó un mecanismo de autenticación robusto para el endpoint que recibe webhooks de Telegram antes de reenviarlos a la orquestación de IA en n8n.

- **Validación de Token Secreto:** El `IntegrationController.cs` ahora exige el encabezado `X-Telegram-Bot-Api-Secret-Token`.
- **Configuración Segura:** El token esperado se almacena en `appsettings.json` bajo la clave `Telegram:WebhookToken`, permitiendo su rotación sin cambios de código.
- **Defensa Proactiva:** Cualquier petición sin el token correcto o con uno inválido es rechazada con un `401 Unauthorized` y se registra un `LogWarning` que incluye la dirección IP del origen para detección de intrusos.

## 👤 FASE 2: Refinamiento de Identidad y Auditoría Dinámica
Se mejoró la trazabilidad de las acciones en el sistema para identificar claramente a los usuarios humanos detrás de cada cambio, eliminando la ambigüedad del valor "Sistema".

- **Extensión del Contexto de Usuario:** Se añadió la propiedad `NombreUsuario` a la interfaz `ICurrentUserContext`.
- **Mapeo de Identidad Multicanal:**
    - **Firebase Auth:** El evento `OnTokenValidated` en `Program.cs` ahora consulta el repositorio de usuarios para inyectar el nombre real del operario en el contexto.
    - **API Key (n8n):** El `ApiKeyMiddleware.cs` inyecta automáticamente el nombre de la Service Account para identificar acciones realizadas por la IA.
- **Auditoría Persistente:** Se modificó la lógica de `AuditEntry.cs` y el `SaveChangesAsync` del `GalponDbContext` para capturar este nombre dinámico y persistirlo en la columna `UsuarioNombre` de la tabla `AuditoriaLogs`.

## 🧹 FASE 3: Limpieza de Deuda Técnica (Filtros Globales)
Se aplicó el principio **DRY (Don't Repeat Yourself)** simplificando la capa de persistencia y confiando en las capacidades nativas de EF Core.

- **Remoción de Lógica Redundante:** Se eliminaron las cláusulas `.Where(x => x.IsActive)` manuales de los repositorios de `Mortalidad`, `Pesajes`, `Gastos`, `Ventas` y `Compras`.
- **Centralización de Reglas de Negocio:** El sistema ahora depende exclusivamente del **Global Query Filter** configurado en el `DbContext`. Esto reduce la probabilidad de errores al escribir nuevas consultas y asegura que el "Soft Delete" se respete en todo el sistema de forma uniforme.
- **Excepciones Controladas:** Se mantuvo el uso de `IgnoreQueryFilters()` solo en métodos específicos de recuperación de datos (como en `VentaRepository.ObtenerPorIdAsync`) donde es necesario acceder a registros desactivados.

## ⚡ FASE 4: Robustez de la Interfaz (Optimistic Updates)
Se optimizó la percepción de velocidad en el frontend para operaciones críticas de alta frecuencia.

- **Patrón de Actualización Optimista:** Implementado en el hook `useMortalidad.ts` para las mutaciones de **Actualizar** y **Eliminar**.
- **Mecánica de Ejecución (TanStack Query):**
    - **onMutate:** Cancela peticiones en curso, guarda un snapshot del estado actual y actualiza la caché local instantáneamente.
    - **onError:** En caso de fallo de red o error del servidor, el sistema realiza un **Rollback** automático devolviendo la UI al estado anterior al cambio.
    - **onSettled:** Invalida las consultas relacionadas para garantizar que, tras la respuesta del servidor, los datos locales estén 100% sincronizados con la base de datos real.
- **Resultado:** El usuario experimenta una respuesta inmediata (latencia cero percibida) al registrar o corregir bajas en los lotes.

---
*Documentación técnica de post-auditoría finalizada por el Agente de Desarrollo.*
