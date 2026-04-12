# PLAN DE DESARROLLO - FASE 3.3: GO-LIVE (PRODUCTION READY)

## SPRINT 60: Blindaje de Arquitectura y Data Inicial
*Objetivo: Conectar los cables sueltos de infraestructura, reportes y asegurar la consistencia del motor matemático para el lanzamiento.*
- [x] 1. **Persistencia (Global Filters):** Actualizar `GalponDbContext.cs`. Agregar `HasQueryFilter(e => e.IsActive)` a todas las entidades correspondientes en `OnModelCreating` para aislar los Soft Deletes del sistema.
- [x] 2. **Persistencia (Seeding):** Crear `GalponDbSeeder` (o lógica en EF Core) para inyectar Categorías base (Alimento, Medicina, Vacuna) y Unidades de Medida (Kg, Unidad, Dosis) si las tablas están vacías.
- [x] 3. **Application (PPP Trigger):** Auditar `RegistrarIngresoMercaderiaCommandHandler`. Garantizar que se invoque la lógica matemática que recalcula el `CostoUnitarioActual` en la entidad `Producto` usando el Costo Promedio Ponderado.
- [x] 4. **Infraestructura (Background Jobs):** Registrar formalmente `AlertaInventarioJob` y `AlertaSanitariaJob` en el contenedor de dependencias (`Program.cs` o `DependencyInjection.cs`) para que se ejecuten en segundo plano.
- [x] 5. **Infraestructura (PDF):** Refactorizar `PdfService.cs`. Sustituir los placeholders por una tabla estructurada que muestre los KPIs reales del lote (Pollos iniciales, Mortalidad, Consumo total de alimento, Ingresos, Egresos y Utilidad Neta).