# ANÃLISIS INTEGRAL DEL SISTEMA - GALPON ERP

## 1. VisiÃ³n General del Sistema
GalponERP es una plataforma SaaS (Software as a Service) de grado industrial diseÃ±ada para la gestiÃ³n integral de granjas avÃ­colas. El sistema trasciende un simple registro de datos, actuando como un **Ecosistema de Verdad** donde la inteligencia artificial, el monitoreo operativo y la gestiÃ³n financiera convergen para garantizar la rentabilidad y trazabilidad absoluta.

## 2. Arquitectura TÃ©cnica
El sistema estÃ¡ construido sobre una arquitectura de **Clean Architecture** y **Domain-Driven Design (DDD)**, estructurada en cuatro capas:

### 2.1 Capa de Dominio (Core)
*   **Entidades Ricas:** LÃ³gica de negocio encapsulada en las entidades (ej. `Lote.RegistrarVenta`, `Producto.RecalcularCostoPPP`).
*   **Value Objects:** Uso de objetos como `Moneda` para garantizar precisiÃ³n matemÃ¡tica y evitar errores de redondeo.
*   **Inmutabilidad y AuditorÃ­a:** Cada entidad hereda de una base que gestiona `IsActive` (Soft Delete) y campos de auditorÃ­a (`UsuarioId`).

### 2.2 Capa de AplicaciÃ³n (Casos de Uso)
*   **CQRS (MediatR):** SeparaciÃ³n estricta entre comandos (escritura) y consultas (lectura).
*   **ValidaciÃ³n Proactiva:** Uso de `FluentValidation` antes de ejecutar cualquier lÃ³gica de negocio.
*   **OrquestaciÃ³n de IA:** IntegraciÃ³n de Semantic Kernel para traducir lenguaje natural en comandos tÃ©cnicos.

### 2.3 Capa de Infraestructura (Persistencia y Servicios)
*   **Persistencia:** PostgreSQL con EF Core, utilizando filtros globales para Soft Delete.
*   **Identidad:** Firebase Auth integrado con una tabla local de usuarios para gestiÃ³n de RBAC (Role-Based Access Control).
*   **Servicios Externos:** Firebase Admin (Notificaciones), WhatsApp Business API, OpenAI (Whisper/TTS) y QuestPDF (Reportes).

### 2.4 Capa de PresentaciÃ³n (API y Frontend)
*   **REST API:** Blindada con JWT y documentada mediante Swagger con contratos claros (`endpoints.md`).
*   **Frontend Moderno:** Next.js 14 con App Router y Tailwind CSS, ofreciendo una experiencia responsiva y rÃ¡pida.

## 3. Pilares Operativos y LÃ³gica de Negocio

### 3.1 GestiÃ³n de Lotes y ProducciÃ³n
El ciclo de vida del lote es el corazÃ³n del sistema.
*   **Ancla MatemÃ¡tica:** Los productos tienen una `PesoUnitarioKg`, permitiendo que el sistema calcule el **FCR (Ãndice de ConversiÃ³n Alimenticia)** de forma exacta: `FCR = Alimento Consumido (Kg) / Incremento de Biomasa (Kg)`.
*   **Sanidad SaaS:** Uso de plantillas inmutables que se clonan al crear un lote para asegurar que el plan sanitario se cumpla y sea auditable.

### 3.2 Motor Financiero Avanzado
*   **Costeo PPP:** El sistema calcula automÃ¡ticamente el Precio Promedio Ponderado de los insumos con cada compra.
*   **Flujo de Caja:** GestiÃ³n de Cuentas por Pagar (CxP) a proveedores y Cuentas por Cobrar (CxC) de ventas a crÃ©dito.
*   **Snapshots de Cierre:** Al cerrar un lote, el sistema congela los KPIs (Utilidad, FCR, Mortalidad) para evitar alteraciones histÃ³ricas.

### 3.3 Inventario y KÃ¡rdex Valorado
*   **Trazabilidad Total:** Cada gramo de alimento o dosis de vacuna estÃ¡ vinculado a un movimiento de inventario, un autor y, opcionalmente, un lote.
*   **Cero Negativos:** PolÃ­tica estricta que impide registrar consumos si no hay stock fÃ­sico disponible.

## 4. Inteligencia Artificial y Omnicanalidad

### 4.1 Operador Maestro (IA)
La IA no es un chatbot, es un **Operador de Sistema**:
*   **ResoluciÃ³n Difusa:** Capacidad de entender nombres con errores (ej. "galpon uno") mediante el algoritmo de Levenshtein.
*   **Regla de Oro de UX:** Si hay ambigÃ¼edad, pregunta; si hay certeza, actÃºa proactivamente.
*   **Seguridad:** Acciones de alto impacto requieren confirmaciÃ³n explÃ­cita mediante un flujo de "Intenciones Pendientes".

### 4.2 InteracciÃ³n Omnicanal
*   **WhatsApp:** OperaciÃ³n completa vÃ­a chat, incluyendo registro de mortalidad y consulta de saldos.
*   **Voz:** Procesamiento de audio para manos libres en galpones (STT/TTS).
*   **Handshake Seguro:** VinculaciÃ³n de nÃºmeros telefÃ³nicos mediante cÃ³digos de 6 dÃ­gitos para garantizar que solo usuarios autorizados accedan vÃ­a WhatsApp.

## 5. Seguridad y AuditorÃ­a
*   **RBAC:** Roles definidos (Admin, SubAdmin, Empleado) con permisos granulares.
*   **AuditorÃ­a de queries:** Capacidad de la IA para auditar quiÃ©n realizÃ³ una acciÃ³n y cuÃ¡ndo.
*   **Consistencia de Datos:** El **Dashboard Snapshot** garantiza que la IA, la Web y el Backend siempre manejen los mismos nÃºmeros.

---
*Documento generado para el desarrollo del contexto completo del sistema GalponERP.*
