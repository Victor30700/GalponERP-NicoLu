# ANÁLISIS INTEGRAL DEL SISTEMA - GALPON ERP

## 1. Visión General del Sistema
GalponERP es una plataforma SaaS (Software as a Service) de grado industrial diseñada para la gestión integral de granjas avícolas. El sistema trasciende un simple registro de datos, actuando como un **Ecosistema de Verdad** donde la inteligencia artificial, el monitoreo operativo y la gestión financiera convergen para garantizar la rentabilidad y trazabilidad absoluta.

## 2. Arquitectura Técnica
El sistema está construido sobre una arquitectura de **Clean Architecture** y **Domain-Driven Design (DDD)**, estructurada en cuatro capas:

### 2.1 Capa de Dominio (Core)
*   **Entidades Ricas:** Lógica de negocio encapsulada en las entidades (ej. `Lote.RegistrarVenta`, `Producto.RecalcularCostoPPP`).
*   **Value Objects:** Uso de objetos como `Moneda` para garantizar precisión matemática y evitar errores de redondeo.
*   **Inmutabilidad y Auditoría:** Cada entidad hereda de una base que gestiona `IsActive` (Soft Delete) y campos de auditoría (`UsuarioId`).

### 2.2 Capa de Aplicación (Casos de Uso)
*   **CQRS (MediatR):** Separación estricta entre comandos (escritura) y consultas (lectura).
*   **Validación Proactiva:** Uso de `FluentValidation` antes de ejecutar cualquier lógica de negocio.
*   **Orquestación de IA:** Integración de Semantic Kernel para traducir lenguaje natural en comandos técnicos.

### 2.3 Capa de Infraestructura (Persistencia y Servicios)
*   **Persistencia:** PostgreSQL con EF Core, utilizando filtros globales para Soft Delete.
*   **Identidad:** Firebase Auth integrado con una tabla local de usuarios para gestión de RBAC (Role-Based Access Control).
*   **Servicios Externos:** Firebase Admin (Notificaciones), WhatsApp Business API, OpenAI (Whisper/TTS) y QuestPDF (Reportes).

### 2.4 Capa de Presentación (API y Frontend)
*   **REST API:** Blindada con JWT y documentada mediante Swagger con contratos claros (`endpoints.md`).
*   **Frontend Moderno:** Next.js 14 con App Router y Tailwind CSS, ofreciendo una experiencia responsiva y rápida.

## 3. Pilares Operativos y Lógica de Negocio

### 3.1 Gestión de Lotes y Producción
El ciclo de vida del lote es el corazón del sistema.
*   **Ancla Matemática:** Los productos tienen una `EquivalenciaEnKg`, permitiendo que el sistema calcule el **FCR (Índice de Conversión Alimenticia)** de forma exacta: `FCR = Alimento Consumido (Kg) / Incremento de Biomasa (Kg)`.
*   **Sanidad SaaS:** Uso de plantillas inmutables que se clonan al crear un lote para asegurar que el plan sanitario se cumpla y sea auditable.

### 3.2 Motor Financiero Avanzado
*   **Costeo PPP:** El sistema calcula automáticamente el Precio Promedio Ponderado de los insumos con cada compra.
*   **Flujo de Caja:** Gestión de Cuentas por Pagar (CxP) a proveedores y Cuentas por Cobrar (CxC) de ventas a crédito.
*   **Snapshots de Cierre:** Al cerrar un lote, el sistema congela los KPIs (Utilidad, FCR, Mortalidad) para evitar alteraciones históricas.

### 3.3 Inventario y Kárdex Valorado
*   **Trazabilidad Total:** Cada gramo de alimento o dosis de vacuna está vinculado a un movimiento de inventario, un autor y, opcionalmente, un lote.
*   **Cero Negativos:** Política estricta que impide registrar consumos si no hay stock físico disponible.

## 4. Inteligencia Artificial y Omnicanalidad

### 4.1 Operador Maestro (IA)
La IA no es un chatbot, es un **Operador de Sistema**:
*   **Resolución Difusa:** Capacidad de entender nombres con errores (ej. "galpon uno") mediante el algoritmo de Levenshtein.
*   **Regla de Oro de UX:** Si hay ambigüedad, pregunta; si hay certeza, actúa proactivamente.
*   **Seguridad:** Acciones de alto impacto requieren confirmación explícita mediante un flujo de "Intenciones Pendientes".

### 4.2 Interacción Omnicanal
*   **WhatsApp:** Operación completa vía chat, incluyendo registro de mortalidad y consulta de saldos.
*   **Voz:** Procesamiento de audio para manos libres en galpones (STT/TTS).
*   **Handshake Seguro:** Vinculación de números telefónicos mediante códigos de 6 dígitos para garantizar que solo usuarios autorizados accedan vía WhatsApp.

## 5. Seguridad y Auditoría
*   **RBAC:** Roles definidos (Admin, SubAdmin, Empleado) con permisos granulares.
*   **Auditoría de queries:** Capacidad de la IA para auditar quién realizó una acción y cuándo.
*   **Consistencia de Datos:** El **Dashboard Snapshot** garantiza que la IA, la Web y el Backend siempre manejen los mismos números.

---
*Documento generado para el desarrollo del contexto completo del sistema GalponERP.*
