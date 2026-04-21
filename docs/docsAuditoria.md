# Documentación Técnica: Implementación de Blindaje de Sistema (Fases 1-4)
**Proyecto:** Pollos NicoLu ERP  
**Fecha:** 20 de Abril, 2026  
**Autor:** Gemini CLI Agent

---

## 1. Fase 1: Blindaje de Seguridad Alimentaria (Food Safety)

### 1.1 El Problema
Existía un riesgo crítico donde lotes bajo tratamiento médico (antibióticos o vacunas) podían ser vendidos o sacrificados accidentalmente antes de que los residuos químicos desaparecieran de la carne (Periodo de Retiro).

### 1.2 Solución Implementada
Se estableció un control estricto de fechas de seguridad alimentaria que vincula el consumo de fármacos con la disponibilidad de venta del lote.

### 1.3 Cambios Técnicos
- **`Producto` (Dominio):** Se añadió la propiedad `PeriodoRetiroDias`.
- **`Lote` (Dominio):** 
    - Propiedad `FechaFinRetiro` (nulleable).
    - Método `RegistrarAplicacionMedica(DateTime fecha, int dias)`: Calcula la fecha de seguridad más restrictiva.
    - Método `EsAptoParaVenta(DateTime fecha)`: Validación booleana contra la fecha de retiro.
- **Orquestación de Comandos (Aplicación):**
    - Se modificaron `RegistrarConsumoAlimento`, `RegistrarConsumoFormula` y `MarcarVacunaAplicada` para invocar automáticamente `RegistrarAplicacionMedica` en el lote cuando el producto utilizado tiene días de retiro configurados.
- **`RegistrarVentaParcial` (Blindaje Final):** Se añadió una validación que lanza una excepción de negocio si se intenta vender un lote antes de su `FechaFinRetiro`.

---

## 2. Fase 2: Inteligencia Sanitaria (Monitoreo de Agua)

### 2.1 El Problema
El consumo de agua es el indicador más temprano de enfermedad, pero el sistema solo permitía un registro manual de litros sin validación ni trazabilidad de lecturas de medidor.

### 2.2 Solución Implementada
Se transformó el registro de bienestar en una herramienta de auditoría y alerta temprana proactiva.

### 2.3 Cambios Técnicos
- **`RegistroBienestar` (Dominio):**
    - Se añadió `LecturaMedidor` para permitir ingresos basados en el contador físico del galpón.
    - Método `CalcularConsumo(decimal lecturaAnterior)`: Automatiza la diferencia entre lecturas.
- **`SanidadService` (Servicio de Dominio - Nuevo):**
    - Lógica de análisis de desviaciones. Compara el consumo actual contra el promedio de los últimos 3 días.
    - Umbral de alerta configurado al **10% de caída**.
- **`RegistrarBienestarCommandHandler`:**
    - Ahora recupera el historial previo para calcular el consumo automáticamente si el usuario solo ingresa la lectura del medidor.
    - Ejecuta el análisis de alerta en tiempo real tras cada registro.

---

## 3. Fase 3: Trazabilidad de Inventario (Lotes y Clasificación)

### 3.1 El Problema
El sistema no distinguía entre lotes de fabricante (vencimientos de vacunas) y la lógica de identificación de medicamentos era frágil (basada en cadenas de texto).

### 3.2 Solución Implementada
Se introdujo una estructura de datos robusta para la gestión de stocks por lote y una clasificación taxonómica de productos.

### 3.3 Cambios Técnicos
- **`InventarioLote` (Entidad Nueva):** Permite rastrear `CodigoLote` de fabricante, `FechaVencimiento` y `StockActual` por lote físico.
- **`TipoCategoria` (Enum Nuevo):** Clasificación explícita: `Alimento`, `Medicamento`, `Vacuna`, `Insumo`, `Otros`.
- **`CategoriaProducto` (Dominio):** Se añadió la propiedad `Tipo` para evitar el uso de strings en validaciones.
- **`MovimientoInventario` (Dominio):** Se añadió `InventarioLoteId` para vincular cada salida de bodega con un lote específico del fabricante.
- **Refactor de Fórmulas:** El motor de consumo de fórmulas ahora identifica medicamentos y vacunas mediante el enum `TipoCategoria`, garantizando precisión total en el disparo de alertas de retiro.

---

## 4. Fase 4: Supervisor Virtual (Auditoría de FCR)

### 4.1 El Problema
La eficiencia alimenticia (FCR) es el factor de rentabilidad más importante, pero solo se calculaba al cerrar el lote (post-mortem), impidiendo correcciones durante el ciclo.

### 4.2 Solución Implementada
Se dotó al sistema de un "Supervisor Virtual" que audita la eficiencia cada 12 horas.

### 4.3 Cambios Técnicos
- **`Lote` (Dominio):**
    - Método `CalcularFCRActual(totalAlimento, pesoPromedio)`: Calcula la conversión en tiempo real.
    - Método `ValidarEficienciaAlimenticia(fcr)`: Evalúa si el FCR es aceptable según la edad del pollo (curvas de crecimiento).
- **`AnalisisDatosJob` (Background Job):**
    - Se inyectaron `IInventarioRepository` e `IPesajeLoteRepository`.
    - Realiza una auditoría cruzada: suma todo el alimento consumido (salidas de inventario) y lo divide por la biomasa estimada (pesajes recientes).
    - Si la eficiencia cae fuera de rango, genera una anomalía de negocio.
- **`AgenteOrquestadorService` (IA):**
    - Capacidad de transformar anomalías técnicas (FCR, Mortalidad, Deudas) en mensajes ejecutivos proactivos enviados vía WhatsApp a los administradores.

---

## 5. Resumen de Impacto en el Negocio

| Característica | Antes | Después |
| :--- | :--- | :--- |
| **Venta de Pollos** | Riesgo de residuos médicos. | Bloqueo automático por retiro sanitario. |
| **Detección de Enfermedad** | Visual (tardía). | Alerta por caída de consumo de agua (temprana). |
| **Control de Stock** | Genérico por producto. | Por lote de fabricante con vencimientos. |
| **Análisis de Rentabilidad** | Solo al final del lote. | Auditoría de FCR diaria (Supervisor Virtual). |
| **Identificación de Insumos** | Por nombre (frágil). | Por taxonomía (robusto). |

**Fin del Documento.**
