# CONTRATO DE API - GALPON ERP

Todos los endpoints requieren autenticación mediante **JWT Bearer Token** (Firebase), excepto el endpoint de Login.

**Nota sobre Auditoría:** El sistema registra automáticamente el `UsuarioId` de quien realiza la transacción a partir del Token JWT. El Frontend **NO** debe enviar el campo `UsuarioId` en ningún JSON de creación o registro; el backend lo extrae de forma segura desde la identidad del usuario autenticado. 

**Log de Auditoría:** Cualquier operación de tipo `PUT`, `DELETE` o `Reabrir` genera automáticamente un registro en la tabla de Auditoría para trazabilidad total.

## 0. AUTENTICACIÓN

### Login (Firebase Proxy)
- **URL:** `/api/Auth/login`
- **Método:** `POST`
- **Autenticación:** **Anónima (No requiere Token)**
- **Entrada (JSON):**
```json
{
  "email": "usuario@ejemplo.com",
  "password": "password123"
}
```
- **Salida (JSON):**
```json
{
  "idToken": "JWT_TOKEN_STRING",
  "refreshToken": "REFRESH_TOKEN_STRING",
  "email": "usuario@ejemplo.com",
  "expiresIn": 3600
}
```

## 1. LOTES

### Listar Lotes
- **URL:** `/api/Lotes`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Query Params:** `soloActivos` (bool, default: true)
- **Salida (JSON):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "galponId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "fechaIngreso": "2026-04-10T00:00:00Z",
    "cantidadInicial": 5000,
    "cantidadActual": 4995,
    "mortalidadAcumulada": 5,
    "pollosVendidos": 0,
    "costoUnitarioPollito": 1.50,
    "estado": "Activo"
  }
]
```

### Obtener Detalle de Lote
- **URL:** `/api/Lotes/{id}`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Salida (JSON):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "galponId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fechaIngreso": "2026-04-10T00:00:00Z",
  "cantidadInicial": 5000,
  "cantidadActual": 4900,
  "mortalidadTotal": 10,
  "pollosVendidos": 90,
  "costoUnitarioPollito": 1.50,
  "estado": "Activo",
  "totalVentas": 495.00,
  "totalGastos": 120.50,
  "utilidadEstimada": -7125.50,
  "pesoPromedioActualGramos": 1250.50,
  "fcrActual": 1.62,
  "ventas": [],
  "historialMortalidad": [],
  "gastos": [],
  "pesajes": []
}
```

### Crear Lote
- **URL:** `/api/Lotes`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Entrada (JSON):**
```json
{
  "galponId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fechaIngreso": "2026-04-10T00:00:00Z",
  "cantidadInicial": 5000,
  "costoUnitarioPollito": 1.50
}
```
- **Salida (JSON):**
```json
{
  "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Actualizar Lote (Datos Iniciales)
- **URL:** `/api/Lotes/{id}`
- **Método:** `PUT`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Entrada (JSON):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "galponId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fechaIngreso": "2026-04-10T00:00:00Z",
  "cantidadInicial": 5000,
  "costoUnitarioPollito": 1.55
}
```
- **Salida:** `204 No Content`
- **Nota:** Solo permitido si el lote está `Abierto`. Recalcula automáticamente la `CantidadActual`.

### Eliminar Lote
- **URL:** `/api/Lotes/{id}`
- **Método:** `DELETE`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Salida:** `204 No Content`
- **Nota:** Realiza Soft Delete (`IsActive = false`).

### Cancelar Lote
- **URL:** `/api/Lotes/{id}/cancelar`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Entrada (JSON - Body Literal):** `"Justificación de la cancelación"`
- **Salida:** `204 No Content`
- **Nota:** Cambia el estado a `Cancelado` e inactiva el calendario sanitario.

### Trasladar Lote
- **URL:** `/api/Lotes/{id}/trasladar`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Entrada (JSON - Body Literal):** `"3fa85f64-5717-4562-b3fc-2c963f66afa6"` (ID del nuevo Galpón)
- **Salida:** `204 No Content`
- **Nota:** Cambia el `GalponId` del lote.

### Cerrar Lote
- **URL:** `/api/Lotes/{id}/cerrar`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Entrada (JSON):**
```json
{
  "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```
- **Salida (JSON):**
```json
{
  "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "totalIngresos": 15000.00,
  "totalCostos": 10000.00,
  "utilidadNeta": 5000.00,
  "fcr": 1.65,
  "porcentajeMortalidad": 3.25
}
```

### Reabrir Lote
- **URL:** `/api/Lotes/{id}/reabrir`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)
- **Salida:** `204 No Content`
- **Nota:** Limpia los campos de Snapshot contable y pone el lote en estado `Activo`.

## 2. OPERACIONES DIARIAS

### Obtener Toda la Mortalidad Histórica
- **URL:** `/api/Mortalidad`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Salida (JSON):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "fecha": "2026-04-10T00:00:00Z",
    "cantidadBajas": 5,
    "causa": "Calor excesivo"
  }
]
```

### Registrar Mortalidad (Bajas)
- **URL:** `/api/Mortalidad`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Entrada (JSON):**
```json
{
  "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "cantidad": 5,
  "causa": "Calor excesivo",
  "fecha": "2026-04-10T00:00:00Z"
}
```
- **Salida (JSON):** `"3fa85f64-5717-4562-b3fc-2c963f66afa6"`

### Actualizar Mortalidad
- **URL:** `/api/Mortalidad/{id}`
- **Método:** `PUT`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Entrada (JSON):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "cantidad": 4,
  "causa": "Error de conteo",
  "fecha": "2026-04-10T00:00:00Z"
}
```
- **Salida:** `204 No Content`
- **Nota:** Reajusta automáticamente los contadores del Lote.

### Eliminar Mortalidad
- **URL:** `/api/Mortalidad/{id}`
- **Método:** `DELETE`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Salida:** `204 No Content`
- **Nota:** Soft Delete. Revierte los contadores del Lote.

### Registrar Pesaje de Lote
- **URL:** `/api/Pesajes`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Entrada (JSON):**
```json
{
  "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fecha": "2026-04-11T10:00:00Z",
  "pesoPromedioGramos": 1250.5,
  "cantidadMuestreada": 50
}
```
- **Salida (JSON):** `"3fa85f64-5717-4562-b3fc-2c963f66afa6"`

### Actualizar Pesaje
- **URL:** `/api/Pesajes/{id}`
- **Método:** `PUT`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Entrada (JSON):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fecha": "2026-04-11T10:00:00Z",
  "pesoPromedioGramos": 1260.0,
  "cantidadMuestreada": 55
}
```
- **Salida:** `204 No Content`

### Eliminar Pesaje
- **URL:** `/api/Pesajes/{id}`
- **Método:** `DELETE`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Salida:** `204 No Content`

### Obtener Movimientos por Producto (Kárdex)
- **URL:** `/api/inventario/productos/{id}/movimientos`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Salida (JSON):**
```json
[
  {
    "id": "...",
    "productoId": "...",
    "nombreProducto": "...",
    "loteId": "...",
    "cantidad": 10.0,
    "tipo": "Salida",
    "fecha": "...",
    "justificacion": "..."
  }
]
```

### Registrar Gasto Operativo
- **URL:** `/api/Gastos`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Entrada (JSON):**
```json
{
  "galponId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "descripcion": "Pago Luz Abril",
  "monto": 45.50,
  "moneda": "USD",
  "fecha": "2026-04-10T00:00:00Z",
  "tipoGasto": "Servicios"
}
```
- **Salida (JSON):** `"3fa85f64-5717-4562-b3fc-2c963f66afa6"`

## 2.1 VENTAS Y PAGOS

### Registrar Venta Parcial (Crédito)
- **URL:** `/api/Ventas/parcial`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Entrada (JSON):**
```json
{
  "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "clienteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fecha": "2026-04-11T12:00:00Z",
  "cantidadPollos": 100,
  "pesoTotalVendido": 250.5,
  "precioPorKilo": 12.50
}
```
- **Salida (JSON):** `{ "ventaId": "..." }`
- **Nota:** Crea una venta con `EstadoPago = Pendiente`.

### Registrar Pago a Venta
- **URL:** `/api/Ventas/{id}/pagos`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Entrada (JSON):**
```json
{
  "monto": 500.00,
  "fechaPago": "2026-04-11T15:00:00Z",
  "metodoPago": 1
}
```
- **Salida (JSON):** `{ "pagoId": "..." }`
- **Nota:** Actualiza dinámicamente el `EstadoPago` de la venta (Parcial o Pagado). `metodoPago`: 1=Efectivo, 2=Transferencia, 3=Deposito, 4=QR.

### Anular Venta
- **URL:** `/api/Ventas/{id}/anular`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)
- **Salida:** `204 No Content`
- **Nota:** Soft Delete. Revierte los pollos vendidos en el Lote.

## 2.2 PLANTILLAS SANITARIAS

### Listar Plantillas
- **URL:** `/api/Plantillas`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Salida (JSON):**
```json
[
  {
    "id": "...",
    "nombre": "Plan Invierno",
    "descripcion": "Refuerzo de vitaminas",
    "actividades": [
      {
        "id": "...",
        "tipoActividad": "Vacuna",
        "diaDeAplicacion": 7,
        "descripcion": "Newcastle",
        "productoIdRecomendado": "..."
      }
    ]
  }
]
```

### Crear Plantilla
- **URL:** `/api/Plantillas`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Entrada (JSON):**
```json
{
  "nombre": "Plan Estándar 45 días",
  "descripcion": "Plan base para pollos de engorde",
  "actividades": [
    {
      "tipo": 1,
      "diaDeAplicacion": 7,
      "descripcion": "Vacuna Triple",
      "productoIdRecomendado": null
    }
  ]
}
```
- **Nota:** `tipo`: 1=Vacuna, 2=Vitaminas, 3=Desinfectante, 4=Antibiotico, 5=Otros.

### Actualizar Plantilla
- **URL:** `/api/Plantillas/{id}`
- **Método:** `PUT`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Entrada (JSON):** Mismo formato que Crear, incluyendo el `id` en el body.

## 2.3 CALENDARIO SANITARIO

### Obtener Calendario por Lote
- **URL:** `/api/calendario/{loteId}`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Salida (JSON):**
```json
[
  {
    "id": "...",
    "diaDeAplicacion": 7,
    "descripcionTratamiento": "Newcastle",
    "productoIdRecomendado": "...",
    "estado": "Pendiente"
  }
]
```

### Marcar Vacuna como Aplicada
- **URL:** `/api/calendario/{id}/aplicar`
- **Método:** `PATCH`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Entrada (JSON):**
```json
{
  "cantidadConsumida": 10.5
}
```
- **Salida:** `204 No Content`
- **Nota:** Descuenta automáticamente el inventario del producto recomendado. Valida stock suficiente.

## 2.4 INVENTARIO

### Obtener Stock Actual
- **URL:** `/api/inventario/stock`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Query Params:** `productoId` (Guid, opcional)
- **Salida (JSON):**
```json
[
  {
    "productoId": "...",
    "productoNombre": "Alimento Iniciador",
    "stockActual": 150.5,
    "unidadMedida": "Bulto 50kg"
  }
]
```

### Registrar Consumo Diario de Alimento
- **URL:** `/api/inventario/consumo-diario`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Entrada (JSON):**
```json
{
  "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "productoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "cantidad": 5.0,
  "justificacion": "Alimentación mañana"
}
```
- **Salida (JSON):** `{ "movimientoId": "..." }`
- **Nota:** Registra una SALIDA de inventario vinculada a un lote. Valida stock suficiente.

## 3. FINANZAS E INTELIGENCIA

### Obtener Cuentas por Cobrar
- **URL:** `/api/Finanzas/cuentas-por-cobrar`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Salida (JSON):**
```json
[
  {
    "ventaId": "...",
    "fecha": "2026-04-11T12:00:00Z",
    "clienteNombre": "Juan Perez",
    "loteCodigo": "LOT-001",
    "totalVenta": 3131.25,
    "totalPagado": 500.00,
    "saldoPendiente": 2631.25,
    "estadoPago": "Parcial"
  }
]
```

### Obtener Flujo de Caja Empresarial
- **URL:** `/api/Finanzas/flujo-caja`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Query Params:** `inicio` (DateTime), `fin` (DateTime)
- **Salida (JSON):**
```json
{
  "totalIngresos": 25000.50,
  "totalEgresos": 12000.00,
  "utilidadNeta": 13000.50,
  "ventas": [...],
  "gastos": [...]
}
```

### Reporte de Mortalidad Transversal
- **URL:** `/api/Mortalidad/reporte-transversal`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Query Params:** `inicio` (DateTime), `fin` (DateTime)
- **Salida (JSON):**
```json
{
  "totalBajas": 150,
  "porCausa": [
    { "causa": "Calor", "cantidad": 80, "porcentaje": 53.33 }
  ],
  "detalle": [...]
}
```

### Comparativa de Eficiencia por Galpón
- **URL:** `/api/Dashboard/comparativa-galpones`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Salida (JSON):**
```json
[
  {
    "galponId": "...",
    "nombre": "Galpón A",
    "totalLotes": 5,
    "promedioMortalidad": 2.1,
    "promedioFCR": 1.62,
    "utilidadTotalAcumulada": 45000.00
  }
]
```

## 4. AUDITORÍA

### Obtener Logs de Sistema
- **URL:** `/api/Auditoria/logs`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)
- **Salida (JSON):**
```json
[
  {
    "id": "...",
    "usuarioId": "...",
    "accion": "Actualizar|Eliminar|Reabrir",
    "entidad": "Mortalidad|Pesaje|Lote|...",
    "entidadId": "...",
    "fecha": "...",
    "detallesJSON": "{...}"
  }
]
```

... (resto de secciones de inventario, usuarios, etc. se mantienen igual) ...

---

# BITÁCORA ARQUITECTÓNICA - GALPON ERP

... (Decisiones 14.1 a 27.2 se mantienen igual) ...

## Decisión 29.1: Flexibilidad Operativa y Corrección de Errores
Se habilitó la edición y eliminación de registros de Mortalidad y Pesajes. 
- **Integridad Biológica:** Al editar o eliminar mortalidad, el sistema revierte automáticamente los contadores del lote (`CantidadActual`, `MortalidadAcumulada`) antes de aplicar el nuevo valor o eliminar el registro, garantizando que el stock de pollos vivos sea siempre exacto.
- **Protección de Cierre:** No se permite la edición de estos registros si el lote asociado ya está `Cerrado` o `Cancelado`.

## Decisión 29.2: Gestión Dinámica de Lotes
Se implementó `ActualizarLoteCommand` permitiendo modificar la `FechaIngreso`, `CantidadInicial` y `CostoUnitario`. El sistema recalcula la `CantidadActual` restando la mortalidad y ventas históricas de la nueva cantidad inicial enviada.

## Decisión 30.1: Inteligencia de Negocio Transversal
Se crearon motores de consulta que consolidan datos de múltiples lotes:
1. **Flujo de Caja:** Une ventas y gastos operativos en un rango de fechas para ver la salud financiera global de la granja.
2. **Análisis de Mortalidad:** Agrupa causas de muerte en toda la granja para identificar problemas sistémicos (ej. epidemias o fallas de infraestructura).
3. **Benchmarking de Galpones:** Compara la rentabilidad histórica (`Utilidad`, `FCR`, `Mortalidad`) entre galpones físicos para identificar cuál infraestructura es más eficiente.

## Decisión 30.2: Normalización de Relaciones (GalponId en Lote)
Se identificó una debilidad en el modelo donde el Lote no conocía su Galpón de origen de forma directa. Se añadió `GalponId` a la entidad `Lote` y se actualizó el motor de creación. Esto permite reportes de eficiencia por galpón mucho más precisos y directos.

## Decisión 31.1: Governance y Recuperación (Reapertura de Lotes)
Se implementó el comando `ReabrirLote`, restringido estrictamente al rol `Admin`. Al ejecutarlo, se eliminan los snapshots contables (`FCRFinal`, `UtilidadNeta`, etc.) y el lote vuelve a estado `Activo`. Esto permite corregir cierres erróneos sin necesidad de intervenciones manuales en la base de datos.

## Decisión 31.2: Trazabilidad Total mediante Auditoría Automatizada
Se implementó una arquitectura de Auditoría Pasiva mediante un `AuditoriaBehavior` de MediatR:
1. **Detección Automática:** El pipeline intercepta cualquier comando que contenga las palabras "Actualizar", "Eliminar" o "Reabrir".
2. **Captura de Contexto:** Se guarda el `UsuarioId` del token, la acción realizada, el ID de la entidad afectada y el payload JSON completo del comando.
3. **Centralización:** Los logs se almacenan en una tabla `AuditoriaLogs` con soporte para `jsonb` en PostgreSQL, facilitando búsquedas futuras.

## Decisión 31.3: Refuerzo de Soft Delete
Se estandarizó que toda acción de "Eliminar" en registros operativos (Mortalidad, Pesajes, Gastos, Ventas) es un **Soft Delete** (`IsActive = false`). En el caso de Mortalidad y Ventas, esta acción también dispara la reversión de los contadores biológicos en el Lote asociado.

## Decisión 36.1: Ejecución Sanitaria Integrada con Inventario
Se refactorizó la aplicación de vacunas para garantizar la trazabilidad total:
1. **Consumo de Insumos:** Marcar una vacuna como aplicada ahora genera automáticamente un movimiento de SALIDA en el inventario.
2. **Validación de Stock:** El sistema impide la aplicación de tratamientos si no existe stock suficiente del producto recomendado, lanzando una `InventarioDomainException`.
3. **Seguridad JWT:** El `UsuarioId` responsable del consumo se extrae automáticamente del token, cumpliendo con los estándares de auditoría SaaS.
4. **Endpoint PATCH:** Se migró a `PATCH` para reflejar la actualización parcial del estado del calendario.

## Decisión 37.1: Flujo de Alimentación de Alto Rendimiento
Se implementó un comando especializado para el registro de alimentación diaria:
1. **Transaccionalidad Atómica:** En un solo paso se registra el consumo, se valida el stock y se vincula el costo al lote correspondiente.
2. **Cálculo de Biomasa:** El sistema utiliza la `EquivalenciaEnKg` del producto para normalizar el consumo, permitiendo que el Frontend reporte KPIs de eficiencia (FCR) sin cálculos manuales propensos a errores.
3. **Ergonomía Operativa:** El endpoint simplificado permite al galponero registrar el alimento consumido con un solo clic desde la interfaz móvil.

## Decisión 38.1: Visibilidad Operativa 360° (Kárdex y Dashboard)
Se completó la capa de lectura para habilitar una gestión basada en datos:
1. **Kárdex Detallado:** El nuevo endpoint de movimientos por producto permite auditar cada gramo de insumo, incluyendo justificaciones de ajustes y consumos operativos.
2. **Dashboard de Resumen Ejecutivo:** Se consolidaron indicadores críticos (Pollos Vivos, Cuentas por Cobrar, Tareas Pendientes) en una sola consulta optimizada.
3. **Automatización de Tareas:** El sistema ahora calcula automáticamente qué tareas sanitarias deben realizarse "Hoy" basándose en la fecha de ingreso de cada lote activo, eliminando la necesidad de seguimiento manual por parte del granjero.
