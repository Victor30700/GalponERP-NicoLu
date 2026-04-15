# Plan de Trabajo: Refactorización de Productos e Inventario (Control en Kg)

Este plan detalla los cambios necesarios para implementar el control exacto de inventario en Kilogramos y Unidades, permitiendo que los consumos se registren en Kg y se traduzcan correctamente a unidades físicas.

## 1. Backend (.NET - Clean Architecture)

### Cambios en Dominio
- **`GalponERP.Domain\Entities\Producto.cs`**:
    - Validar que `ActualizarStock` siga siendo compatible con la lógica de unidades.
    - Asegurar que el constructor y el método `Actualizar` manejen correctamente la relación entre peso y unidades.

### Cambios en Aplicación (CQRS)
- **`GalponERP.Application\Productos\Commands\CrearProducto\CrearProductoCommand.cs`**:
    - Actualizar el Record para incluir explícitamente `EquivalenciaEnKg` (Peso Total Inicial).
    - Modificar el `CrearProductoCommandHandler` para priorizar `EquivalenciaEnKg` si se proporciona, o calcularlo como `StockInicial * PesoUnitarioKg`.
- **`GalponERP.Application\Inventario\Commands\RegistrarConsumoAlimento\RegistrarConsumoAlimentoCommandHandler.cs`**:
    - **CRÍTICO**: Modificar el Handler para tratar la `request.Cantidad` como Kilogramos.
    - Calcular las unidades correspondientes: `decimal unidades = request.Cantidad / producto.PesoUnitarioKg;`.
    - Registrar el `MovimientoInventario` utilizando las `unidades` calculadas.
    - Llamar a `producto.ActualizarStock(unidades, TipoMovimiento.Salida)` para actualizar el peso total en la entidad.
- **Validadores**:
    - Actualizar `CrearProductoCommandValidator` y `ActualizarProductoCommandValidator` para incluir las nuevas reglas de negocio.

### Cambios en Infraestructura
- **`GalponERP.Infrastructure\Persistence\Configurations\ProductoConfiguration.cs`**:
    - Revisar precisiones de campos decimales para asegurar exactitud en cálculos de Kg (4 decimales recomendados).
- **Migraciones**:
    - Generar una migración de EF Core (ej: `AddPesoUnitarioToProducto`) si se detectan cambios estructurales, aunque la entidad actual ya parece tener los campos base. Se realizará un `dotnet ef migrations add` preventivo.

## 2. Frontend (Next.js / React)

### Módulo de Productos
- **`frontend\src\app\(dashboard)\productos\page.tsx`**:
    - **Esquema Zod**: Agregar `equivalenciaEnKg` al esquema de validación.
    - **Formulario**: 
        - Agregar input para `equivalenciaEnKg` (Peso Total Inicial).
        - Implementar `useEffect` o lógica reactiva en el formulario: `EquivalenciaEnKg = stockInicial * pesoUnitarioKg`.
        - Permitir que el usuario sobrescriba el cálculo si es necesario.

### Módulo de Lotes (Operaciones)
- **`frontend\src\components\production\QuickRecordModal.tsx`**:
    - Modificar el registro de tipo `feed` (Alimento).
    - Invertir o clarificar la prioridad de entrada: El usuario ingresará Kg consumidos como valor principal.
    - Mostrar el stock restante dinámico en Kg: "Disponibles: {stockActualKg} Kg".
    - Asegurar que el payload enviado al endpoint `/api/Inventario/consumo-diario` contenga la cantidad en Kilogramos en el campo esperado por el nuevo Handler del backend.

## 3. Base de Datos / SQL
- Scripts de actualización si fuera necesario para corregir stock acumulado existente basado en la nueva lógica de equivalencias.

## 4. Validación y Pruebas
- **Pruebas Unitarias**: Actualizar `GalponERP.Tests\ProductoTests.cs` para validar que el consumo en Kg descuenta correctamente el stock en unidades y peso.
- **Flujo E2E**: Crear un producto (10 sacos de 50kg = 500kg), registrar consumo de 25kg, verificar que quedan 9.5 sacos y 475kg.

---
**ESTADO: ESPERANDO APROBACIÓN PARA PROCEDER CON LA IMPLEMENTACIÓN.**
