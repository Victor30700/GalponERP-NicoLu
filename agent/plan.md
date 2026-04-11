# PLAN DE DESARROLLO - FASE 1.8: REFACCIÓN SAAS Y CATÁLOGOS DINÁMICOS

## SPRINT 25: Dominio Dinámico y Ancla Matemática
*Objetivo: Reemplazar Enums rígidos por entidades escalables sin romper la matemática del negocio.*
- [x] 1. Dominio: Crear entidades `CategoriaProducto` (Nombre, Descripcion) y `UnidadMedida` (Nombre, Abreviatura).
- [x] 2. Dominio: Refactorizar `Producto`. Eliminar Enums `Tipo` y `UnidadMedida`. Añadir Foreign Keys `CategoriaProductoId` y `UnidadMedidaId` y sus propiedades de navegación.
- [x] 3. Dominio (CRÍTICO): Añadir propiedad `EquivalenciaEnKg` (decimal) a `Producto`. Esta será el multiplicador oficial para cualquier cálculo de FCR.
- [x] 4. Application: Implementar CRUD Completo (Commands/Queries) para `CategoriaProducto` y `UnidadMedida`.
- [x] 5. Application: Actualizar comandos de `Producto` (`Crear`, `Actualizar`) para requerir los nuevos IDs y la `EquivalenciaEnKg`.

## SPRINT 26: Migración de Datos y Reparación del Motor
*Objetivo: Migración sin pérdida de datos y actualización de cálculos en memoria.*
- [x] 1. Infraestructura: Crear la migración en EF Core. **REGLA CRÍTICA:** Modificar el método `Up()` manualmente para insertar las categorías por defecto (Alimento, Medicamento, Insumo) y unidades (Kg, Unidad, Litro, Saco) y actualizar los productos existentes apuntando a estos nuevos IDs ANTES de borrar las columnas de los Enums antiguos.
- [x] 2. Application: Refactorizar `ObtenerDetalleLoteQueryHandler` (o la calculadora de FCR). El Alimento Consumido ahora se calcula multiplicando `Cantidad` * `Producto.EquivalenciaEnKg`.
- [x] 3. API: Exponer `CategoriasController` y `UnidadesMedidaController` protegiéndolos con roles de Admin/SubAdmin.
- [x] 4. API: Actualizar los endpoints de `ProductosController`.
- [x] 5. Documentación: Actualizar exhaustivamente `endpoints.md` con las nuevas rutas y estructuras JSON.