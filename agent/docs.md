# BITÁCORA DE ARQUITECTURA - FASE 3.3 (GO-LIVE)

## SPRINT 60: Blindaje de Arquitectura y Data Inicial (Go-Live Ready)

### Decisiones Tomadas
1. **Filtros Globales de Query (Soft Delete):** 
   - Se inyectó dinámicamente un `HasQueryFilter(e => e.IsActive)` a TODAS las entidades del dominio que heredan de la clase base `Entity`.
   - **Mecanismo:** Se utilizó reflexión sobre el `ModelBuilder` en el método `OnModelCreating` de `GalponDbContext.cs` junto con un `ReplacingExpressionVisitor`.
   - **Beneficio:** Garantiza la exclusión automática de registros eliminados lógicamente (Lotes cancelados, facturas anuladas, usuarios desactivados, etc.) en todas las consultas de la aplicación, evitando "fugas de datos" en la contabilidad y reportes sin requerir sobre-esfuerzo manual.
   
2. **Data Inicial (Seeding):**
   - Se creó la clase `GalponDbSeeder` invocada automáticamente durante el inicio en `Program.cs`. 
   - **Seguridad:** El método verifica de forma asíncrona con `.AnyAsync()` si las tablas están vacías antes de inyectar los catálogos base (Unidades de medida comunes y Categorías de productos). Esto previene excepciones por duplicación de identificadores (Guids) al reiniciar o desplegar la aplicación.
   
3. **Consistencia Matemática (PPP):**
   - Se certificó que el Handler `RegistrarIngresoMercaderiaCommandHandler` invoca consistentemente la lógica de dominio `RecalcularCostoPPP` en la entidad `Producto` antes de guardarla. Este disparador es esencial para no corromper la valoración del inventario.
   
4. **Infraestructura de Reportes y Background Jobs:**
   - La **Ficha de Liquidación PDF** en `PdfService.cs` ya no imprime cadenas dinámicas genéricas; ahora renderiza una tabla limpia con columnas estructuradas ("Concepto" y "Valor") impulsada nativamente por **QuestPDF**, cruzando variables financieras como FCR, Costos y Utilidad neta formateadas con la moneda global.
   - Las alertas autónomas (`AlertaInventarioJob`, `AlertaSanitariaJob`) constan en `Program.cs` registradas como `HostedServices` listas para actuar en segundo plano protegiendo las cuotas de vida avícola.
