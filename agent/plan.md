# PLAN DE DESARROLLO - FASE 2.2: PULIDO FINAL Y RENDIMIENTO (API FRONT-READY)

## SPRINT 39: Estandarización de Formularios (Lecturas Individuales)
*Objetivo: Proveer los endpoints `GET by ID` necesarios para que el Frontend pueda rellenar los formularios de edición.*
- [x] 1. **Galpones:** Crear `ObtenerGalponPorIdQuery` y exponer `GET /api/galpones/{id}`.
- [x] 2. **Gastos:** Crear `ObtenerGastoOperativoPorIdQuery` y exponer `GET /api/gastos/{id}`.
- [x] 3. **Operaciones:** Crear `ObtenerMortalidadPorIdQuery` y `ObtenerPesajePorIdQuery`. Exponer los endpoints respectivos (`GET /api/mortalidad/{id}` y `GET /api/pesajes/{id}`).

## SPRINT 40: Estandarización de Ciclo de Vida (Soft Deletes Faltantes)
*Objetivo: Permitir al Admin depurar la base de datos de errores sin romper la integridad referencial.*
- [x] 1. **Galpones:** Implementar `EliminarGalponCommand` (Soft Delete).
- [x] 2. **Galpones API:** Exponer `DELETE /api/galpones/{id}` (Restringido a Admin).
- [x] 3. **Plantillas:** Implementar `EliminarPlantillaSanitariaCommand` (Soft Delete).
- [x] 4. **Plantillas API:** Exponer `DELETE /api/plantillas/{id}` (Restringido a Admin/SubAdmin).

## SPRINT 41: Refactorización de Clean Code y Rendimiento (ICurrentUserContext)
*Objetivo: Eliminar consultas redundantes a la base de datos en los Controladores.*
- [x] 1. **Refactor de Controladores:** Auditar `GastosController`, `PesajesController`, `InventarioController`, `MortalidadController` y `VentasController`.
- [x] 2. **Optimización:** Eliminar métodos privados tipo `GetUsuarioIdActual()` que inyecten `IUsuarioRepository`. Reemplazarlos inyectando `ICurrentUserContext` y llamando directamente a `_currentUserContext.UsuarioId`.
- [x] 3. **Actualización de Documentación:** Revisar exhaustivamente `docs/endpoints.md` asegurando que los nuevos `GET {id}` y `DELETE` estén documentados con sus respuestas JSON.