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

## 3. FINANZAS E INTELIGENCIA

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
