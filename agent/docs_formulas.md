# Documentación Técnica: Módulo de Nutrición y Fórmulas (Recetas)

## Descripción General
El módulo de nutrición permite la creación de recetas compuestas (fórmulas) que agrupan múltiples insumos (alimento, vitaminas, medicamentos). Al registrar un consumo basado en una fórmula, el sistema calcula automáticamente las proporciones necesarias de cada ingrediente basándose en una "Cantidad Base" definida.

## Arquitectura de Datos

### Entidades Principales
- **Formula**: Define el encabezado de la receta (Nombre, Etapa, Cantidad Base).
- **FormulaDetalle**: Define los ingredientes y su cantidad requerida para la Cantidad Base de la fórmula.

### Relaciones
- Una `Formula` tiene muchos `FormulaDetalle`.
- Un `FormulaDetalle` está vinculado a un `Producto`.

## Lógica de Negocio: Consumo Compuesto

### Algoritmo de Cálculo
Cuando se prepara una cantidad $Q_{total}$ de una fórmula $F$, el sistema calcula un factor de escala $S$:
$$S = \frac{Q_{total}}{F.CantidadBase}$$

Para cada ingrediente $i$ en la fórmula, la cantidad a descontar del inventario $D_i$ es:
$$D_i = F.Detalle_i.CantidadPorBase \times S$$

### Integración Sanitaria
Si un ingrediente pertenece a una categoría cuyo nombre contiene "Medicamento", "Vacuna" o "Sanidad", el sistema:
1. Calcula el día de vida del lote: $Dia = FechaConsumo - FechaIngresoLote + 1$.
2. Crea automáticamente un registro en `CalendarioSanitario`.
3. Marca la actividad como `Aplicado`.
4. Vincula el producto y la cantidad consumida al registro sanitario.

### Integridad Transaccional
Todo el proceso se ejecuta dentro de un `Unit of Work`. Si el stock de cualquier ingrediente es insuficiente o ocurre un error en el registro sanitario, la transacción completa se revierte, garantizando la consistencia del inventario.

## Frontend
- **Administración**: `/formulas` permite el CRUD completo con edición dinámica de detalles.
- **Operación**: En el detalle de cada lote, el `QuickRecordModal` de Alimento permite alternar entre el registro de un insumo único o el uso de una fórmula predefinida.

## Endpoints API
- `GET /api/Formulas`: Listado de recetas.
- `POST /api/Formulas`: Creación de receta.
- `POST /api/Lotes/{id}/consumo-formula`: Registro de consumo compuesto para un lote específico.
