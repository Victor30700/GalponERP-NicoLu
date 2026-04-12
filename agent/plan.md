# PLAN MAESTRO: OPERADOR PROFESIONAL DE ÉLITE (FASE FINAL)

## VISIÓN GENERAL
Transformar la IA en un **Operador Maestro de Sistemas (OMS)** que no solo ejecute comandos, sino que razone sobre el estado del negocio, gestione el tiempo con precisión técnica y mantenga la integridad del sistema mediante auditoría y reportes.

---

## SPRINT 64: CONSCIENCIA TEMPORAL Y AUDITORÍA (IA DETECTIVE)
*Objetivo: Que la IA entienda el tiempo y el historial de acciones.*

- [ ] 1. **Time & Context Plugin (`ContextoPlugin.cs`):**
      - Crear funciones para que la IA resuelva rangos de fechas técnicos: `ObtenerRangoFecha(string descripcion)` (ej. "la semana pasada" -> {Inicio: 06/04, Fin: 12/04}).
      - Inyectar la hora y fecha local exacta en cada turno para evitar desfases.
- [ ] 2. **Plugin de Auditoría e Historial (`AuditoriaPlugin.cs`):**
      - Exponer `ObtenerAuditoriaLogsQuery`.
      - Permitir que la IA responda preguntas como: *"¿Quién cambió el precio del maíz ayer?"* o *"¿Cuándo se registró la última baja en el Galpón 1?"*.
- [ ] 3. **Plugin de Sanidad y Bienestar (`SanidadPlugin.cs`):**
      - Gestión de `CalendarioSanitario`: Listar tareas pendientes y registrar aplicaciones de vacunas/tratamientos.
      - **Inferencia Proactiva:** Si hay mortalidad alta, la IA debe sugerir revisar el calendario sanitario.

---

## SPRINT 65: ABASTECIMIENTO Y CRECIMIENTO (IA LOGÍSTICA)
*Objetivo: Cerrar el ciclo de inventario y monitorear el desarrollo de las aves.*

- [ ] 1. **Plugin de Abastecimiento (`AbastecimientoPlugin.cs`):**
      - Registrar Compras de Insumos (`RegistrarCompraInventarioCommand`).
      - Gestionar pagos a proveedores para mantener las finanzas al día.
- [ ] 2. **Plugin de Pesajes y Crecimiento (`PesajesPlugin.cs`):**
      - Registrar peso promedio por lote.
      - Comparar el crecimiento actual contra los estándares de la raza (Cobb 500/Ross 308) e informar desviaciones.
- [ ] 3. **Plugin de Configuración (`ConfiguracionPlugin.cs`):**
      - Permitir a la IA ajustar parámetros como el `UmbralMinimo` de stock o costos operativos base.

---

## SPRINT 66: REPORTERÍA Y MANTENIMIENTO (IA ADMINISTRATIVA)
*Objetivo: Generar documentos oficiales y mantener la base de datos limpia.*

- [ ] 1. **Plugin de Reportes Oficiales (`ReportesPlugin.cs`):**
      - Integrar con `IPdfService` para generar y enviar (vía respuesta de chat) reportes de cierre de lote, flujo de caja y estado de inventario.
- [ ] 2. **Plugin de Mantenimiento de Catálogos (Extensión `GestionCatalogosPlugin.cs`):**
      - Habilitar `Actualizar` y `Eliminar` (soft-delete) para Productos, Clientes, Proveedores y Categorías.
      - Implementar validaciones de seguridad: La IA debe confirmar antes de eliminar cualquier registro.

---

## REGLAS DE ORO DEL OPERADOR MAESTRO (ACTUALIZADAS)
1.  **Regla de Contexto Total:** La IA debe consultar la auditoría antes de dar por hecho una situación dudosa.
2.  **Regla de Resolución Temporal:** Nunca usar fechas hardcoded; siempre calcular el rango dinámicamente mediante el `ContextoPlugin`.
3.  **Regla de Profesionalismo:** Si una acción tiene impacto financiero alto (ej. cerrar un lote o eliminar un proveedor), la IA debe resumir el impacto antes de ejecutar.
4.  **Regla de Cero GUIDs:** En toda la interacción humana, se usan nombres. La conversión a GUID ocurre exclusivamente en la capa de C# del Plugin.
