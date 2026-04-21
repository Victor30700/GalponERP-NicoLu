# Auditoría de Proyecto: Pollos NicoLu - "Blindaje del Sistema"

## 1. Introducción
Tras un análisis profundo de la base de código y los requerimientos de mejora detallados en `mejoras.md`, se identifican vulnerabilidades críticas en la lógica de negocio que podrían comprometer la seguridad alimentaria, la rentabilidad y la integridad de los datos.

## 2. Hallazgos Críticos: Riesgos de Blindaje

### 2.1. Riesgo de Seguridad Alimentaria: Periodo de Retiro (CRÍTICO)
*   **Situación Actual:** El sistema permite registrar la aplicación de medicamentos y vacunas, pero no existe trazabilidad del "Periodo de Retiro" (tiempo que debe pasar desde la última dosis hasta el sacrificio).
*   **Vulnerabilidad:** Se puede registrar la venta de un lote que aún tiene residuos químicos activos. El `VentasController` y `RegistrarVentaParcialCommandHandler` no realizan ninguna validación de estado sanitario.
*   **Impacto:** Riesgo legal, sanciones sanitarias y peligro para la salud pública.

### 2.2. Reactividad vs. Proactividad: Monitoreo de Agua
*   **Situación Actual:** La entidad `RegistroBienestar` contiene un campo decimal para `ConsumoAgua`. 
*   **Vulnerabilidad:** No se capturan datos base como "Lectura de Medidor" (entrada/salida), lo que impide detectar errores de registro o fugas. Tampoco existen alertas automáticas si el consumo cae un 10%, que es el indicador principal de enfermedad.
*   **Impacto:** Detección tardía de enfermedades, aumentando la tasa de mortalidad antes de que sea visible.

### 2.3. Integridad de Inventario en Fórmulas
*   **Situación Actual:** Existe un `RegistrarConsumoFormulaCommandHandler` que descuenta ingredientes. 
*   **Vulnerabilidad:** 
    *   La detección de medicamentos en la fórmula depende de comparaciones de cadenas de texto (`nombreCat.Contains("medicamento")`), lo cual es frágil.
    *   No se maneja el concepto de "Lote de Proveedor" ni "Fecha de Vencimiento" en el inventario. Se asume un stock genérico por producto.
*   **Impacto:** Imposibilidad de realizar trazabilidad ante fallos de vacunas (no se sabe qué lote de vacuna se aplicó).

### 2.4. Fragilidad en el Registro de "Unidades Abiertas"
*   **Situación Actual:** El registro de consumo permite ingresar kg o unidades, pero la conversión depende enteramente del `PesoUnitarioKg` estático del producto.
*   **Vulnerabilidad:** No hay tolerancia para mermas o variaciones de peso en sacos reales. El sistema no solicita justificación ante discrepancias significativas entre lo pesado y lo descontado.

---

## 3. Análisis de Arquitectura (Blindaje Técnico)

### 3.1. Capa de Dominio (Entidades)
*   **Lote:** Carece de propiedades como `FechaFinRetiro` o `EstadoSanitario`.
*   **Producto:** No tiene el campo `PeriodoRetiroDias`.
*   **Venta:** No tiene validación cruzada con el historial sanitario del lote.

### 3.2. Capa de Aplicación (Casos de Uso)
*   **Validaciones:** Las validaciones actuales son de tipo estructural (campos obligatorios) pero no de reglas de negocio complejas (reglas de seguridad alimentaria).
*   **Transaccionalidad:** El uso de `Unit of Work` es correcto, asegurando que si falla el descuento de un ingrediente en una fórmula, no se registre el consumo.

---

## 4. Hoja de Ruta para el Blindaje (Plan de Acción)

### Fase 1: Blindaje de Seguridad Alimentaria
1.  **Modificar `Producto`:** Añadir `PeriodoRetiroDias`.
2.  **Modificar `Lote`:** Añadir `FechaVencimientoRetiro` (calculada dinámicamente al aplicar medicamentos).
3.  **Blindar Ventas:** Modificar el Command de Venta para que lance una excepción si la fecha actual es menor a `FechaVencimientoRetiro`.

### Fase 2: Inteligencia Sanitaria (Agua y Alertas)
1.  **Evolucionar `RegistroBienestar`:** Añadir campos de `LecturaMedidorAnterior` y `LecturaMedidorActual`.
2.  **Automatización:** Implementar un servicio que compare el consumo de hoy contra el promedio de los últimos 3 días. Disparar notificaciones si la desviación es > 10%.

### Fase 3: Trazabilidad de Inventario
1.  **Gestión de Lotes:** Implementar la entidad `InventarioLote` para rastrear vencimientos de vacunas/medicamentos específicos.
2.  **Refactor de Fórmulas:** Mejorar la lógica de identificación de insumos críticos usando enums o flags en lugar de strings.

### Fase 4: Auditoría y Supervisor Virtual
1.  **Agente Orquestador:** Potenciar el agente para que realice auditorías nocturnas de FCR (Eficiencia Alimenticia) y alerte sobre lotes con bajo rendimiento.

---
**Documento de Auditoría elaborado por Gemini CLI - 20 de Abril, 2026**
