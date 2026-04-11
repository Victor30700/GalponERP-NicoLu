# PLAN DE DESARROLLO - FASE 1.6: AUDITORÍA, HISTÓRICOS Y DASHBOARD

## SPRINT 17: Perfil y Gestión de Catálogos
*Objetivo: Identidad del frontend y gestión total de maestras.*
- [x] 1. API/Application: Crear `GET /api/usuarios/me` para devolver el perfil y `RolGalpon` del usuario autenticado (extrayendo FirebaseUid del Token).
- [x] 2. API/Application: Completar CRUD de `ProductosController` implementando Editar (PUT) y Soft Delete (DELETE).
- [x] 3. API/Application: Completar CRUD de `ClientesController` implementando Editar (PUT) y Soft Delete (DELETE).

## SPRINT 18: Auditoría y Ajustes de Inventario
*Objetivo: Control absoluto del almacén.*
- [x] 1. API/Application: Crear `GET /api/inventario/movimientos` (Kardex general histórico).
- [x] 2. API/Application: Crear `GET /api/inventario/productos/{id}/stock` (Stock de un ítem específico).
- [x] 3. API/Application: Crear `PUT /api/inventario/ajuste` (Para mermas, robos o sacos rotos, requiriendo justificación y ajustando el stock sin afectar costos de lotes).

## SPRINT 19: Históricos de Operación (Ventas y Mortalidad)
*Objetivo: Trazabilidad del día a día.*
- [x] 1. API/Application: Crear `GET /api/ventas` (Historial filtrable) y `GET /api/ventas/{id}` (Detalle/Recibo).
- [x] 2. API/Application: Crear `GET /api/mortalidad/lote/{loteId}` (Historial de bajas de un lote específico para análisis de brotes).

## SPRINT 20: Inteligencia Gerencial (Dashboard Avanzado)
*Objetivo: Proyecciones y comparativas financieras.*
- [x] 1. API/Application: Crear `GET /api/dashboard/proyeccion-sacrificio` (Días estimados para alcanzar peso de venta según FCR).
- [x] 2. API/Application: Crear `GET /api/dashboard/comparativa-lotes` (Comparar rentabilidad y FCR entre múltiples lotes).