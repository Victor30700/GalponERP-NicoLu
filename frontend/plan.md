# Plan de Trabajo: Refactorización del Registro de Consumo de Alimento (COMPLETADO)

Este plan aborda el problema de registro de equivalencia en Kg y el cálculo de consumo diario de alimento por unidades o por peso total.

## 1. Problemas Identificados
- [x] **Registro de Producto Confuso**: El campo "Equivalencia en Kg" se presta a confusión.
- [x] **Cálculo de Consumo Rígido**: El modal de registro rápido impedía ajustes precisos.
- [x] **Inconsistencia de Datos**: Los cálculos de consumo y FCR eran incorrectos por falta de peso por unidad individual.

## 2. Acciones en el Backend

### A. Clarificación de la Entidad `Producto`
- [x] Revisar comentarios y validaciones en `Producto.cs`.
- [x] Mejorar la respuesta de la API en `ObtenerMovimientosLoteQuery` agregando `CantidadKg`.

## 3. Acciones en el Frontend

### A. Refactorización de la Gestión de Productos (`productos/page.tsx`)
- [x] Cambiar etiqueta a **"Peso por Unidad (Kg)"**.
- [x] Agregar descripción explicativa sobre la importancia para el FCR.

### B. Mejora del Modal de Registro Rápido (`QuickRecordModal.tsx`)
- [x] **Cálculo Bidireccional**: Sincronización entre Unidades y Kg.
- [x] **Ajuste Manual**: Permitir sobrescribir Kg y recalcular unidades equivalentes.
- [x] **Visualización Clara**: Leyenda dinámica sobre el impacto en el inventario.

### C. Mejora del Historial de Operaciones (`OperationHistoryList.tsx`)
- [x] Mostrar el peso total en Kg (`cantidadKg`) en lugar de las unidades para el historial de alimento.

## 4. Pruebas y Validación
- [x] **Prueba de Registro**: Verificado que la nueva etiqueta es clara.
- [x] **Prueba de Consumo por Unidades**: Verificado el cálculo automático de Kg.
- [x] **Prueba de Consumo por Kg**: Verificado el recalculo de unidades para inventario.
- [x] **Prueba de Ajuste Mixto**: Verificado que al ajustar Kg manualmente se actualizan las unidades enviadas al backend.
