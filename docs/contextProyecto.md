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
# Documentación de Cambios - Refactorización de Inventario y FCR

## Resumen
Se ha realizado una refactorización integral del manejo de unidades de medida y productos para permitir mayor flexibilidad en categorías no alimenticias y mejorar la precisión del cálculo de FCR (Food Conversion Ratio) mediante la sincronización bidireccional de Unidades y Kilogramos en el registro de consumo.

## Cambios en el Backend (.NET)

### Domain Layer
- **Nueva Entidad `TipoUnidad`**: Enum que clasifica las unidades en `Masa`, `Volumen` y `UnidadFisica`.
- **Entidad `UnidadMedida`**: Se agregó la propiedad `Tipo` para categorizar la unidad.
- **Entidad `Producto`**: 
    - Se flexibilizó el constructor para permitir `PesoUnitarioKg = 0`.
    - Ahora es posible crear productos que no requieren peso (ej. Medicinas, Servicios).

### Application Layer
- **Validadores de Producto**:
    - `CrearProductoCommandValidator` y `ActualizarProductoCommandValidator` ahora implementan validación condicional.
    - Solo se exige `PesoUnitarioKg > 0` si la categoría del producto es "Alimento".
- **Handler de Consumo Diario**:
    - `RegistrarConsumoAlimentoCommandHandler` ahora interpreta la cantidad recibida como **Kilogramos** (antes eran unidades).
    - Realiza la conversión interna a unidades para mantener la consistencia del stock, pero preservando la precisión decimal del pesaje ingresado.

### Infrastructure Layer
- **Migración de Base de Datos**: Se generó y aplicó la migración `AddTipoUnidadToUnidadMedida`.
- **Seeder**: Se actualizaron los datos iniciales de unidades de medida para incluir su tipo.

## Cambios en el Frontend (React)

### Registro de Productos (`ProductosPage.tsx`)
- Se implementó renderizado condicional en el formulario.
- Si el producto es "Alimento", se muestran los campos de Peso por Unidad y Peso Total Inicial.
- Si no es "Alimento", estos campos se ocultan y el sistema envía automáticamente `0` como peso.

### Registro de Consumo (`QuickRecordModal.tsx`)
- Se implementó **Sincronización Bidireccional**:
    - Al ingresar **Unidades** (Sacos), se calculan automáticamente los **Kg**.
    - Al ingresar **Kg** directamente, se calculan las **Unidades** equivalentes.
- Se eliminó el bloqueo del campo Kg, permitiendo ajustes manuales precisos.
- El payload enviado al backend ahora utiliza el valor en Kg como base principal.

## Fase 3 y 4: Integración Total y Pulido (Completado)

### Resumen de Integración
Se ha alcanzado la simetría total entre el backend (.NET) y el frontend (React/Next.js) mediante la centralización de la lógica en hooks de React Query, eliminando todas las llamadas directas a la API en los componentes y asegurando un flujo de datos consistente.

### Cambios en el Frontend (React)

#### Hooks Centralizados (`frontend/src/hooks`)
- **`useAgentes.ts`**: Implementado para gestionar conversaciones con la IA, incluyendo soporte para mensajes de voz (STT/TTS) y gestión de historial.
- **`useAuditoria.ts`**: Refactorizado para alinearse con la entidad del backend, permitiendo la visualización detallada de logs y la restauración de entidades eliminadas.
- **`useLotes.ts`**: Se integraron nuevas funciones para el cierre de lotes (`cerrarLote`), cancelación, reapertura y descarga de reportes PDF.
- **`useFinanzas.ts`**: Se agregó soporte para la consulta de **Cuentas por Pagar**, completando la visibilidad financiera del sistema.

#### Nuevas Funcionalidades y Componentes
- **Chat IA Refactorizado (`chat/page.tsx`)**: Ahora utiliza exclusivamente hooks para el envío de mensajes y carga de historial, mejorando la reactividad y el manejo de errores.
- **Panel de Auditoría (`auditoria/page.tsx`)**: Refactorizado para usar el hook centralizado, con visualización mejorada de detalles técnicos en formato JSON.
- **Liquidación de Lotes (`CerrarLoteModal.tsx`)**: 
    - Nuevo componente modal que captura datos críticos de liquidación (precio de venta promedio, fecha de cierre y observaciones).
    - Integrado en el dashboard de lotes para formalizar el fin del ciclo productivo.
- **Reportes PDF**: Integración de la descarga de informes de cierre directamente desde la interfaz de gestión de lotes.

#### Validación y Feedback Visual
- **React Query**: Se auditó la invalidación de queries en todas las mutaciones. El uso de `queryClient.invalidateQueries` asegura que al realizar cambios (ej. eliminar un lote, registrar un pago), todas las vistas relacionadas se actualicen instantáneamente sin recargar la página.
- **Feedback de Usuario**: Se estandarizó el uso de `sonner` para notificaciones tipo toast y `SweetAlert2` para diálogos de confirmación, asegurando que cada acción (éxito o error) sea comunicada claramente al usuario.
- **Navegación**: Se implementaron redirecciones automáticas tras acciones destructivas (ej. volver al listado tras eliminar un lote).

### Cambios en el Backend (.NET)
- Se verificó la consistencia de los DTOs de Auditoría y Finanzas para asegurar la compatibilidad con los nuevos campos requeridos por el frontend.

## Verificación Final
- **Simetría**: El 100% de los controladores del backend ahora tienen un hook correspondiente en el frontend.
- **UX**: Flujo de navegación fluido con manejo de estados de carga (`isLoading`) y errores en todas las vistas críticas.
- **Persistencia**: Se confirmó que las acciones de restauración en el panel de auditoría funcionan correctamente reintegrando los datos eliminados.



# 📝 DOCUMENTACIÓN DE REPORTES SAVCO (GalponERP)

Este documento resume los reportes PDF implementados para el sistema de gestión avícola.

## 📋 Listado de Reportes

| ID | Nombre | Descripción | Endpoint API |
|---|---|---|---|
| **SAVCO-01** | Ingreso de Lote | Datos de recepción de aves, proveedor y costos iniciales. | `/api/lotes/{id}/reportes/ingreso` |
| **SAVCO-02** | Bitácora de Mortalidad | Historial de bajas agrupado por causa y fecha. | `/api/lotes/{id}/reportes/mortalidad` |
| **SAVCO-03** | Ficha de Desempeño Semanal | Resumen de pesos, mortalidad y consumo por semana de vida. | `/api/lotes/{id}/reportes/semanal` |
| **SAVCO-04** | Consumo de Alimento | Detalle cronológico de entregas de alimento al galpón. | `/api/lotes/{id}/reportes/consumo` |
| **SAVCO-05** | Calendario Sanitario | Historial de vacunación y tratamientos aplicados. | `/api/sanidad/reportes/calendario/{loteId}` |
| **SAVCO-06** | Bienestar Animal | Checklist de los 12 puntos de bienestar y parámetros ambientales. | `/api/sanidad/reportes/bienestar/{registroId}` |
| **SAVCO-07** | Control de Agua | Registro mensual de Cloro, pH y Temperatura del agua. | `/api/sanidad/reportes/agua/{loteId}` |
| **SAVCO-08** | Stock de Insumos | Inventario actual valorizado de todos los productos. | `/api/inventario/reportes/stock` |
| **SAVCO-09** | Liquidación de Lote | Informe final económico (Utilidad Neta) y biológico (FCR). | `/api/lotes/{id}/reportes/liquidacion` |

## 🛠️ Detalles Técnicos

- **Motor de PDF:** QuestPDF (.NET 8)
- **Frontend:** Next.js + Lucide Icons + React Hot Toast
- **Seguridad:** Los endpoints requieren rol `Admin`, `SubAdmin` o `Empleado` y utilizan el token JWT para la descarga.

## 🧮 Lógica de Cálculos

- **FCR (Conversión Alimenticia):** Calculado como `Total Alimento (kg) / Peso Ganado Total (kg)`.
- **Mortalidad %:** `(Total Bajas / Aves Ingresadas) * 100`.
- **Utilidad Neta:** `Ingresos por Ventas - (Costo Inicial Aves + Costo Insumos)`.
