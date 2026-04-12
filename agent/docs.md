# BITÁCORA DE ARQUITECTURA - FASE 4: IA Y ORQUESTACIÓN

## Sprint 61: Cerebro Base y Orquestador

### 1. Registro del Kernel en el Contenedor DI
Configuración de Semantic Kernel en `GalponERP.Infrastructure/DependencyInjection.cs`.
- **Endpoint:** `http://localhost:11434`
- **Modelo:** `gemma4:e4b`

### 2. Implementación de Plugins (Skills)
Primer plugin: `ProduccionPlugin.cs` en `GalponERP.Application/Agentes/Plugins`.
- Inyecta `IMediator` e `ICurrentUserContext`.
- No accede directamente a la BD (Patrón CQRS).

## Sprint 61.5 y 61.6: Resolución Contextual

### 1. Inyección de Realidad Temporal
El `AgenteOrquestadorService` inyecta `DateTime.Now` en el System Message para que la IA conozca la fecha actual.

### 2. Resolución de Identidades
- Los plugins ahora reciben nombres legibles (ej. "Galpón 1") en lugar de GUIDs.
- Se enriquecieron los DTOs de Lotes con `NombreGalpon`.
- **Inferencia Automática:** Si solo hay 1 lote activo, el plugin lo selecciona por defecto para reducir fricción.

### 3. Hotfix: Serialización Ollama (Fechas)
Se eliminaron los parámetros `DateTime` de los métodos `[KernelFunction]` para evitar errores de serialización JSON en el conector de Ollama.
- La fecha de registro se genera internamente en el plugin usando `DateTime.UtcNow`.
- Los parámetros del plugin se limitan a tipos primitivos (`string`, `int`).

## Sprint 62: Cerebros de Catálogo, Inventario y Finanzas

### 1. Expansión de Plugins (Skills)
Se crearon tres nuevos plugins en `GalponERP.Application/Agentes/Plugins`:
- **CatalogosPlugin.cs:** Proporciona un "Directorio" mental a la IA con productos, proveedores, clientes y categorías.
- **InventarioPlugin.cs:** Gestión de stock, registro de consumo de alimento y proyecciones de niveles críticos.
- **FinanzasPlugin.cs:** Resumen financiero, flujo de caja y registro rápido de gastos operativos.

### 2. Implementación de Regla 8 (Fallback Conversacional)
Todos los nuevos plugins aplican estrictamente la Regla 8:
- Si no se encuentra una coincidencia exacta por nombre (Producto, Galpón, Categoría), el sistema retorna un listado de opciones válidas al LLM para que este consulte al usuario.
- **Inferencia Inteligente:** Si existe un único registro activo, se selecciona automáticamente.

### 3. Registro Dinámico en el Orquestador
Se actualizaron las dependencias en `AgenteOrquestadorService` para registrar dinámicamente los nuevos plugins en el Kernel de Semantic Kernel.

## Sprint 62.5: Hotfix - Toma de Control en C# (Inferencia y Fallback)

### 1. Resolución de Bloqueo en Semantic Kernel
Se detectó que Semantic Kernel interceptaba la falta de parámetros antes de ejecutar el código C#. 
- **Solución:** Se refactorizaron las firmas de los métodos `[KernelFunction]` para que los parámetros de nombres (Galpón, Producto, Categoría) sean opcionales (`string? = null`).
- **Impacto:** Ahora el control pasa siempre al código C#, permitiendo que nuestras reglas de negocio se ejecuten sin interferencia del orquestador.

### 2. Refuerzo de Regla 7 (Inferencia) y Regla 8 (Fallback)
- Se trasladó la lógica de validación al cuerpo de los métodos.
- **Inferencia (Regla 7):** Si el parámetro es nulo o no coincide, pero solo hay una opción válida en el sistema, el plugin la selecciona automáticamente.
- **Fallback (Regla 8):** Si hay múltiples opciones o ninguna coincide, el plugin retorna un string estructurado listando las opciones reales de la base de datos para que el LLM consulte al usuario.
- **Análisis en Cascada:** Los plugins resuelven dependencias de forma secuencial (ej. primero Galpón, luego Lote, luego Producto), deteniendo la ejecución y solicitando aclaración en el punto exacto de ambigüedad.

## Sprint 63: El Operador Maestro (Flujos Complejos y Escritura)

### 1. Nuevo Plugin de Gestión de Catálogos (`GestionCatalogosPlugin.cs`)
Se habilitó la capacidad de **escritura** para la IA, permitiendo flujos de creación dinámica:
- **Creación de Productos:** La IA ahora puede intentar crear productos. Si la categoría o unidad de medida no existe o es ambigua, el código C# devuelve las opciones disponibles para que la IA guíe al usuario.
- **Creación de Categorías, Clientes y Proveedores:** Comandos directos para expandir los catálogos desde lenguaje natural.

### 2. Evolución del System Prompt (Operador Maestro)
Se redefinió la identidad de la IA en `AgenteOrquestadorService`:
- **Persona:** "Operador Maestro de GalponERP", un asistente de élite proactivo.
- **Directiva de No-Detención:** Se instruye a la IA a ejecutar funciones incluso con datos parciales para que la lógica de C# (Reglas 7 y 8) tome el control y proporcione las opciones reales del sistema.
- **Enfoque en Flujos:** Capacidad para encadenar acciones (ej. crear categoría -> crear producto) de forma conversacional.

### 3. Refuerzo de la Regla de Oro de UX
El sistema ahora garantiza que el usuario nunca llegue a un callejón sin salida técnico, siempre recibiendo alternativas válidas extraídas directamente de los Queries del sistema.

## Sprint 64: El Operador Total (Brechas Críticas)

### 1. Módulo de Ventas (`VentasPlugin.cs`)
- Se habilitó la capacidad de registrar ventas parciales directamente desde el chat.
- La IA resuelve automáticamente el cliente y el lote activo, aplicando la Regla 8 en caso de ambigüedad.
- Consulta de ventas recientes para seguimiento financiero.

### 2. Gestión del Ciclo de Vida del Lote (`GestionLotesPlugin.cs`)
- **Apertura de Lotes:** Capacidad para iniciar nuevos ciclos de producción en galpones vacíos, vinculando plantillas sanitarias de forma inteligente.
- **Cierre de Lotes:** Proceso de liquidación que devuelve un resumen detallado de rentabilidad (Utilidad, FCR, Mortalidad).

### 3. Identificación de Brechas Pendientes
Se realizó un escaneo profundo del sistema identificando los siguientes plugins a desarrollar para completar la cobertura:
- **SanidadPlugin:** Control de calendario y aplicación de vacunas.
- **PesajesPlugin:** Seguimiento del crecimiento de las aves.
- **AbastecimientoPlugin:** Carga de stock mediante registro de compras.
- **AuditoriaPlugin:** Consulta de historial de acciones por usuario.
