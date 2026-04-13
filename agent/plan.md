### Fase 4: Consolidación Final y Maestría Operativa (CIERRE BACKEND)

### Sprint 82: Centralización de Inteligencia (Snapshot Único) [x]
- **Paso 1:** Implementar `ObtenerDashboardSnapshotQuery` en `Application/Dashboard` que consolide: Producción (lotes/edad), Inventario (stock crítico), Finanzas (deudas/cobros) y Seguridad (alertas 24h). [x]
- **Paso 2:** Refactorizar `AgenteOrquestadorService` para usar esta Query, eliminando lógica dispersa de conteo y formateo de snapshots en el servicio. [x]
- **Paso 3:** Asegurar que los cálculos de mortalidad y días de stock en el Snapshot coincidan exactamente con lo reportado en la Web. [x]

### Sprint 83: Perfeccionamiento de la Interacción (Fuzzy Search & UX) [x]
- **Paso 1:** Auditar todos los Plugins (`Inventario`, `Produccion`, `Ventas`, `Abastecimiento`) para reemplazar `FirstOrDefault` por `EntityResolver.Resolve`. [x]
- **Paso 2:** Mejorar `EntityResolver` para que, en caso de ambigüedad (ej. dos galpones similares), la IA pregunte específicamente: "¿Te refieres al Galpón A o al Galpón B?". [x]
- **Paso 3:** Implementar validación estricta de tipos y rangos en los comandos de entrada para evitar registros erróneos (ej. mortalidad negativa). [x]

### Sprint 84: Cierre del Ciclo Financiero y Auditoría Total [x]
- **Paso 1:** Implementar `RegistrarPagoVentaCommand` y su plugin asociado para que la IA pueda cerrar el ciclo de ingresos. [x]
- **Paso 2:** Ampliar `AuditarConsistenciaIntegral` para incluir la reconciliación de Ventas vs Cobros (Cuentas por Cobrar). [x]
- **Paso 3:** Ejecutar suite completa de pruebas unitarias y build final de producción para garantizar 0 margen de error en cálculos contables. [x]

## 4. Verificación Final
- La IA puede explicar el estado financiero de la granja en un solo mensaje coherente. [x]
- El usuario puede escribir con errores y la IA resuelve las entidades sin fallar. [x]
- El ciclo de caja (Compras/Pagos y Ventas/Cobros) está 100% cubierto por la lógica del sistema. [x]
- **Backend blindado:** 0 warnings, 0 inconsistencias de datos, 100% de trazabilidad en auditoría. [x]
