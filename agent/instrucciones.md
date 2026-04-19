# ROL Y CONTEXTO
Eres un Arquitecto de Software Senior y Especialista en .NET/React, experto en Clean Architecture, Patrón CQRS (MediatR) y Domain-Driven Design (DDD). Tu misión es la IMPLEMENTACIÓN TOTAL del "Módulo de Nutrición y Fórmulas (Recetas)" para el ecosistema "GalponERP". Debes asegurar que el sistema soporte el consumo compuesto de insumos (Alimento + Medicinas) mediante proporciones dinámicas.

# OBJETIVO
Implementar de principio a fin la gestión de Fórmulas de Alimentación y su integración con el registro diario de consumo. Esto implica crear las nuevas entidades de dominio, el CRUD de Fórmulas mediante CQRS, y refactorizar el flujo de alimentación para que, al registrar el consumo total de una receta, el backend descuente automáticamente la proporción exacta de múltiples productos del inventario y registre automáticamente las aplicaciones médicas en el calendario sanitario.

# REGLAS DE FLUJO DE TRABAJO (ESTRICTO)
1. **Auditoría de Componentes:** Analiza la estructura actual de `GalponERP.Domain/Entities`, `GalponERP.Infrastructure/Persistence/GalponDbContext.cs`, y los comandos actuales en `GalponERP.Application/Inventario/Commands/RegistrarConsumoAlimento/`.
2. **Generación del Plan:** En `D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\agent\plan_formulas_nutricion.md`, crea un checklist técnico detallado separando la implementación por Fases (Dominio/Infra, CQRS, API, Frontend Next.js).
3. **ESPERA:** Una vez generado el plan, DETENTE y espera mi aprobación para proceder.
4. **Implementación Real:** Usa tus herramientas para CREAR y SOBREESCRIBIR los archivos `.cs`, `.ts` y `.tsx`. Asegúrate de generar la migración de Entity Framework Core al finalizar la fase de Infraestructura.
5. **Documentación:** Al finalizar, detalla en `D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\agent\docs_formulas.md` cómo funciona el algoritmo de deducción de inventario compuesto y cómo se integra con Sanidad.

# ALCANCE TÉCNICO POR CAPA

## 1. Capa de Dominio e Infraestructura
- **Nuevas Entidades:** - `Formula.cs`: Debe contener `Id`, `Nombre` (ej. Mezcla Inicio), `Etapa`, `CantidadBase` (ej. 100 Kg), `Activo`.
  - `FormulaDetalle.cs`: Relaciona la fórmula con los insumos. `Id`, `FormulaId`, `ProductoId`, `CantidadPorBase` (ej. 60 kg de maíz, 0.5 L de vacuna).
- **Persistencia:** Agregar los `DbSet` a `GalponDbContext.cs`, crear las configuraciones (`FormulaConfiguration.cs`) y sus respectivos Repositorios (`IFormulaRepository`, `FormulaRepository`).

## 2. Capa de Aplicación (CQRS)
- **CRUD de Fórmulas:** Crear comandos y queries estándar para gestionar las Fórmulas y sus Detalles (Crear, Editar, Listar).
- **Nuevo Comando de Consumo (`RegistrarConsumoFormulaCommand`):** - Recibe: `LoteId`, `FormulaId`, `CantidadTotalPreparada`, `Fecha`.
  - **Lógica Crítica en el Handler:**
    1. Buscar la Fórmula con sus Detalles.
    2. Calcular el Factor: `FactorMultiplicador = CantidadTotalPreparada / Formula.CantidadBase`.
    3. Iterar los Detalles: `CantidadARestar = Detalle.CantidadPorBase * FactorMultiplicador`.
    4. Ejecutar validación de stock y crear el `MovimientoInventario` (Salida) por cada ingrediente.
  - **Integración con Sanidad:** Dentro de la iteración, si el `Producto` asociado al detalle pertenece a la categoría "Medicamentos" o "Vacunas", se debe crear automáticamente un registro en `CalendarioSanitario` (o registrar el evento) indicando que se aplicó en el alimento.

## 3. Capa de Exposición (API)
- **`FormulasController.cs`:** Exponer los endpoints CRUD.
- **`InventarioController.cs` o `LotesController.cs`:** Exponer el nuevo endpoint `POST /api/lotes/{id}/consumo-formula`.

## 4. Capa Frontend (Next.js)
- **Vistas y Hooks:** Crear `useFormulas.ts` y una nueva página `app/(dashboard)/formulas/page.tsx` para administrar las recetas.
- **Refactorización de `QuickRecordModal.tsx` o Componente de Alimentación:** Modificar la UI de registro rápido de alimento en la vista del lote. Debe permitir al usuario alternar entre "Insumo Individual" (flujo antiguo) y "Por Fórmula" (nuevo flujo). En "Por Fórmula", solo selecciona del dropdown la receta y digita la cantidad total preparada.

# INSTRUCCIÓN DE RENDIMIENTO Y TRANSACCIONALIDAD
El `RegistrarConsumoFormulaCommandHandler` ejecutará múltiples operaciones de escritura (varios movimientos de inventario, actualizaciones de stock de productos y posibles registros sanitarios). Todo este flujo DEBE estar envuelto en una transacción utilizando `_unitOfWork.CommitAsync()` una única vez al final del proceso para asegurar la integridad referencial (si falla la deducción de la vacuna, no debe descontarse el maíz).

# INICIO
Comienza auditando el Dominio de GalponERP. Luego, genera el archivo `plan_formulas_nutricion.md` con los pasos exactos y los archivos afectados para mi revisión.