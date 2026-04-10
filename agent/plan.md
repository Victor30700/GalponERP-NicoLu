## SPRINT 7: Background Jobs (Alertas) y Testing -> **COMPLETADO**
- [x] Tareas 7.1 a 7.5 completadas exitosamente. 11 pruebas unitarias en verde.

## SPRINT 8: Módulo de Planificación e Inteligencia -> **COMPLETADO**
**Objetivo:** Implementar la calculadora de rentabilidad proyectada (Simulador) y el generador automático del Calendario Sanitario por etapas de crianza.

- [x] **Tarea 8.1:** En `GalponERP.Domain/Entities/`, crear la entidad `CalendarioSanitario` (Id, LoteId, DiaDeAplicacion, DescripcionTratamiento, Estado [Pendiente, Aplicado]). Crear `ICalendarioSanitarioRepository` en las interfaces.
- [x] **Tarea 8.2:** En `GalponERP.Domain/Services/`, crear el servicio de dominio `SimuladorProyeccionLote`. Debe contener métodos para:
  - Calcular el consumo proyectado por etapas usando la regla: Inicio (1 al 14 días = 20% del total), Crecimiento (15 al 28 días = 35%), Engorde (29 al 45 días = 45%).
  - Proyectar la Utilidad Bruta recibiendo parámetros "What-If" (Precio del alimento, Precio de venta del pollo, Peso esperado y FCR base de 1.6).
- [x] **Tarea 8.4:** En `GalponERP.Infrastructure`, crear `CalendarioSanitarioRepository`, configurar el mapeo con Fluent API en `GalponDbContext` y registrarlo en la inyección de dependencias. Ejecutar la migración a PostgreSQL.
- [x] **Tarea 8.4:** En `GalponERP.Application`, MODIFICAR el handler de `CrearLoteCommand`. Al crear un lote, debe inyectar automáticamente los registros base en `CalendarioSanitario` (Ej: Día 7: Vacuna Newcastle, Día 14: Gumboro).
- [x] **Tarea 8.5:** En `GalponERP.Application`, crear `GetSimulacionRentabilidadQuery` que utilice el servicio `SimuladorProyeccionLote` y devuelva un DTO detallado. Exponer esto en un nuevo controlador `PlanificacionController` en la API.