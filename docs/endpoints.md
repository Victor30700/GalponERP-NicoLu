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

## 0.1 GALPONES

### Listar Galpones
- **URL:** `/api/Galpones`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Salida (JSON):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "nombre": "Galpón A",
    "capacidad": 5000,
    "ubicacion": "Sector Norte",
    "isActive": true
  }
]
```

### Obtener Galpón por ID
- **URL:** `/api/Galpones/{id}`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Salida (JSON):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "nombre": "Galpón A",
  "capacidad": 5000,
  "ubicacion": "Sector Norte",
  "isActive": true
}
```

### Eliminar Galpón
- **URL:** `/api/Galpones/{id}`
- **Método:** `DELETE`
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)
- **Salida:** `204 No Content`
- **Nota:** Soft Delete (`IsActive = false`).

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
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)
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
- **Método:** `PUT`
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)
- **Salida:** `204 No Content`
- **Nota:** Limpia los campos de Snapshot contable y pone el lote en estado `Activo`. Restringido ESTRICTAMENTE a Admin.

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

### Obtener Mortalidad por ID
- **URL:** `/api/Mortalidad/{id}`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Salida (JSON):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fecha": "2026-04-10T00:00:00Z",
  "cantidadBajas": 5,
  "causa": "Calor excesivo",
  "usuarioId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
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
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)
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

### Obtener Pesaje por ID
- **URL:** `/api/Pesajes/{id}`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Salida (JSON):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fecha": "2026-04-11T10:00:00Z",
  "pesoPromedioGramos": 1250.5,
  "cantidadMuestreada": 50,
  "usuarioId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

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
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)
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

### Obtener Gasto por ID
- **URL:** `/api/Gastos/{id}`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Salida (JSON):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "galponId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "descripcion": "Pago Luz Abril",
  "monto": 45.50,
  "fecha": "2026-04-10T00:00:00Z",
  "tipoGasto": "Servicios",
  "usuarioId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
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

### Eliminar Gasto
- **URL:** `/api/Gastos/{id}`
- **Método:** `DELETE`
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)
- **Salida:** `204 No Content`
- **Nota:** Soft Delete.

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

### Eliminar Plantilla
- **URL:** `/api/Plantillas/{id}`
- **Método:** `DELETE`
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)
- **Salida:** `204 No Content`
- **Nota:** Soft Delete (`IsActive = false`).

## 2.3 CALENDARIO SANITARIO

### Obtener Calendario por Lote
- **URL:** `/api/calendario/{loteId}`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Salida (JSON):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "diaDeAplicacion": 7,
    "fechaProgramada": "2026-04-17T00:00:00Z",
    "descripcionTratamiento": "Newcastle",
    "productoIdRecomendado": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "tipo": 1,
    "esManual": false,
    "justificacion": null,
    "estado": "Pendiente"
  }
]
```
- **Nota:** `estado` puede ser `Pendiente`, `Aplicado` o `Cancelado`. `tipo`: 1=Vacuna, 2=Vitaminas, 3=Desinfectante, 4=Antibiotico, 5=Otros.

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

### Agregar Actividad Sanitaria Manual
- **URL:** `/api/calendario/actividad-manual`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Descripción:** Agregar una actividad sanitaria manual (no programada en la plantilla).
- **Entrada (JSON):**
```json
{
  "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "tipo": 2,
  "fechaProgramada": "2026-04-20T00:00:00Z",
  "descripcion": "Refuerzo vitamínico extra por ola de calor",
  "productoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```
- **Salida (JSON):** `{ "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6" }`

### Reprogramar Actividad Sanitaria
- **URL:** `/api/calendario/{id}/reprogramar`
- **Método:** `PUT`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Descripción:** Reprogramar una actividad sanitaria pendiente.
- **Entrada (JSON):**
```json
{
  "actividadId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "nuevaFecha": "2026-04-22T00:00:00Z",
  "justificacion": "Retraso en entrega de insumos por proveedor"
}
```
- **Salida:** `204 No Content`


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

### Registrar Compra de Mercadería (Ingreso)
- **URL:** `/api/inventario/compras`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Descripción:** Registrar ingreso formal de mercadería con costo y proveedor.
- **Entrada (JSON):**
```json
{
  "productoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "cantidad": 100.0,
  "costoTotalCompra": 1500.50,
  "proveedor": "Distribuidora Avícola S.A.",
  "nota": "Compra de alimento iniciador para el mes de Mayo"
}
```
- **Salida (JSON):** `{ "movimientoId": "..." }`

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

### Obtener Resumen Ejecutivo (Dashboard)
- **URL:** `/api/Dashboard/resumen`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Salida (JSON):**
```json
{
  "totalPollosVivos": 12500,
  "inversionTotalEnCurso": 45600.50,
  "cuentasPorCobrarTotal": 8900.20,
  "tareasPendientesHoy": 5,
  "alertasStockMinimo": [
    {
      "productoNombre": "Alimento Iniciador",
      "stockActual": 10.5,
      "umbralMinimo": 20.0
    },
    {
      "productoNombre": "Vacuna Newcastle",
      "stockActual": 2.0,
      "umbralMinimo": 5.0
    }
  ]
}
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

## 5. PRODUCTOS

### Listar Productos
- **URL:** `/api/Productos`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Salida (JSON):** Listado de productos activos.

### Obtener Producto por ID
- **URL:** `/api/Productos/{id}`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Salida (JSON):** Detalle del producto.

### Crear Producto
- **URL:** `/api/Productos`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Entrada (JSON):** Incluye campos base y `umbralMinimo` (decimal) para alertas de stock.

### Actualizar Producto
- **URL:** `/api/Productos/{id}`
- **Método:** `PUT`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Entrada (JSON):** Incluye campos base y `umbralMinimo` (decimal).

### Eliminar Producto
- **URL:** `/api/Productos/{id}`
- **Método:** `DELETE`
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)
- **Nota:** Soft Delete.

## 6. CLIENTES

### Listar Clientes
- **URL:** `/api/Clientes`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)

### Obtener Cliente por ID
- **URL:** `/api/Clientes/{id}`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)

### Crear Cliente
- **URL:** `/api/Clientes`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)

### Actualizar Cliente
- **URL:** `/api/Clientes/{id}`
- **Método:** `PUT`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)

### Eliminar Cliente
- **URL:** `/api/Clientes/{id}`
- **Método:** `DELETE`
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)
- **Nota:** Soft Delete.

## 7. USUARIOS

### Obtener Perfil Actual (Me)
- **URL:** `/api/Usuarios/me`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Nota:** Utiliza `ICurrentUserContext` internamente para extraer la identidad del usuario de forma segura desde el token JWT.

### Listar Usuarios
- **URL:** `/api/Usuarios`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)

### Registrar Usuario
- **URL:** `/api/Usuarios`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)

### Actualizar Usuario
- **URL:** `/api/Usuarios/{id}`
- **Método:** `PUT`
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)

### Eliminar Usuario
- **URL:** `/api/Usuarios/{id}`
- **Método:** `DELETE`
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)

## 8. OTROS CATÁLOGOS

### Categorías de Producto
- `GET /api/Categorias` (Listar)
- `GET /api/Categorias/{id}` (Obtener)
- `POST /api/Categorias` (Crear - Admin/SubAdmin)
- `PUT /api/Categorias/{id}` (Actualizar - Admin/SubAdmin)
- `DELETE /api/Categorias/{id}` (Eliminar - **Admin**)

### Unidades de Medida
- `GET /api/UnidadesMedida` (Listar)
- `GET /api/UnidadesMedida/{id}` (Obtener)
- `POST /api/UnidadesMedida` (Crear - Admin/SubAdmin)
- `PUT /api/UnidadesMedida/{id}` (Actualizar - Admin/SubAdmin)
- `DELETE /api/UnidadesMedida/{id}` (Eliminar - **Admin**)

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

# BITÁCORA DE ARQUITECTURA - FASE 2.2

## SPRINT 39: Estandarización de Formularios (Lecturas Individuales)

### Decisiones Tomadas
1. **Creación de Queries GET by ID:** Se implementaron los queries `ObtenerGalponPorIdQuery`, `ObtenerGastoOperativoPorIdQuery`, `ObtenerMortalidadPorIdQuery` y `ObtenerPesajePorIdQuery` para permitir que el Frontend recupere la información de un solo registro. Esto es fundamental para las pantallas de edición de formularios.
2. **Uso de DTOs de Respuesta:** Aunque algunos endpoints existentes devolvían entidades de dominio directamente, para los nuevos endpoints se optó por definir registros (`record`) de respuesta específicos (e.g., `GalponDetalleResponse`, `GastoOperativoResponse`). Esto desacopla la API del modelo de dominio y permite una mayor flexibilidad en el futuro.
3. **Mapeo Manual:** Se realizó el mapeo manual de las entidades a los DTOs en los handlers para mantener el control total sobre los datos expuestos, siguiendo los principios de Clean Architecture.

### Endpoints Agregados
- `GET /api/galpones/{id}`
- `GET /api/gastos/{id}`
- `GET /api/mortalidad/{id}`
- `GET /api/pesajes/{id}`

## SPRINT 40: Estandarización de Ciclo de Vida (Soft Deletes Faltantes)

### Decisiones Tomadas
1. **Implementación de Soft Delete:** Se agregaron los comandos `EliminarGalponCommand` y `EliminarPlantillaSanitariaCommand`. Ambos utilizan el método `Eliminar()` heredado de la clase base `Entity`, el cual establece `IsActive = false`. No se eliminan registros físicos para preservar la integridad referencial y permitir auditorías históricas.
2. **Restricción de Roles:**
   - La eliminación de Galpones se restringió exclusivamente al rol `Admin` debido a que es una entidad estructural crítica.
   - La eliminación de Plantillas Sanitarias se permite tanto a `Admin` como a `SubAdmin`, facilitando la gestión operativa de planes sanitarios.

### Endpoints Agregados
- `DELETE /api/galpones/{id}` (Solo Admin)
- `DELETE /api/plantillas/{id}` (Admin y SubAdmin)

## SPRINT 41: Refactorización de Clean Code y Rendimiento (ICurrentUserContext)

### Decisiones Tomadas
1. **Eliminación de Consultas Redundantes:** Se eliminó la dependencia de `IUsuarioRepository` en los controladores operativos (`Gastos`, `Pesajes`, `Inventario`, `Mortalidad`, `Ventas`) para obtener el `UsuarioId`. Anteriormente, se realizaba una consulta a la base de datos basada en el `user_id` de Firebase en cada petición de escritura.
2. **Uso Obligatorio de ICurrentUserContext:** Se estandarizó el uso de `ICurrentUserContext` para extraer la identidad del usuario directamente del contexto de seguridad actual. Esto mejora el rendimiento al evitar viajes de ida y vuelta a la base de datos y simplifica el código de los controladores eliminando métodos privados repetitivos (`GetUsuarioIdActual`).
3. **Consistencia en la Identidad:** Al centralizar la obtención del ID del usuario, se garantiza que la auditoría automática y el registro de transacciones utilicen una fuente de verdad única y eficiente.

### Controladores Refactorizados
- `GastosController`
- `PesajesController`
- `InventarioController`
- `MortalidadController`
- `VentasController`

## SPRINT 43: Dashboard Global y Flujo de Compras

### Decisiones Tomadas
1. **Inteligencia Financiera (Inversión en Curso):** Se refactorizó el Dashboard para incluir la `InversionTotalEnCurso`. El cálculo ahora consolida el costo inicial de los pollitos, los gastos operativos registrados y el costo estimado de los insumos consumidos (alimento/medicina) basándose en el precio promedio de compra.
2. **Gestión de Stock Crítico:** Se añadió el campo `UmbralMinimo` a la entidad `Producto`. El Dashboard ahora reporta una lista de `AlertasStockMinimo` para cualquier producto cuya existencia física sea inferior a dicho umbral, permitiendo una reposición proactiva.
3. **Formalización de Compras:** Se implementó `RegistrarIngresoMercaderiaCommand`. A diferencia del movimiento de inventario genérico, este comando captura el `CostoTotalCompra` y el `Proveedor`, permitiendo alimentar el motor de costos promedio y mejorar la precisión de los reportes financieros.

### Endpoints Agregados/Actualizados
- `GET /api/Dashboard/resumen` (Actualizado con Inversión y Alertas)
- `POST /api/inventario/compras` (Nuevo: Registro formal de compras con costo)

# BITÁCORA DE ARQUITECTURA - FASE 2.3

## SPRINT 42: Maestros Completos y Sesión de Usuario

### Decisiones Tomadas
1. **Completitud CRUD para Maestros:** Se implementaron los queries `ObtenerClientePorIdQuery` y `ObtenerProductoPorIdQuery` para permitir la visualización y edición detallada de Clientes y Productos en el Frontend.
2. **Estandarización de Seguridad (Soft Delete por Admin):** Se ajustaron los controladores para cumplir con la regla de negocio que otorga exclusivamente al rol `Admin (2)` la capacidad de realizar borrados (Soft Delete). Se actualizaron:
   - `ClientesController`
   - `ProductosController`
   - `CategoriasController`
   - `LotesController`
   - `PesajesController`
   - `MortalidadController`
   - `UnidadesMedidaController`
   - `GastosController`
   - `PlantillasController`
3. **Refactorización de Sesión de Usuario:** Se migró el endpoint `/api/usuarios/me` para que utilice `ICurrentUserContext` e inyecte el ID del usuario directamente desde el token JWT en memoria, eliminando la consulta previa a `IUsuarioRepository` en el controlador. Esto optimiza el rendimiento y mejora la seguridad al centralizar la obtención de identidad.
4. **Simplificación de Controladores (DRY):** Se eliminaron métodos redundantes como `GetUsuarioIdActual` en `LotesController` y otros controladores operativos, delegando la responsabilidad de la identidad a la infraestructura de `ICurrentUserContext`.

### Endpoints Agregados/Actualizados
- `GET /api/clientes/{id}`
- `DELETE /api/clientes/{id}` (Restringido a Admin)
- `GET /api/productos/{id}`
- `DELETE /api/productos/{id}` (Restringido a Admin)
- `GET /api/usuarios/me` (Optimizado con `ICurrentUserContext`)

## SPRINT 43: Dashboard Global y Flujo de Compras

### Decisiones Tomadas
1. **Inteligencia Financiera (Inversión en Curso):** Se refactorizó el Dashboard para incluir la `InversionTotalEnCurso`. El cálculo ahora consolida el costo inicial de los pollitos, los gastos operativos registrados y el costo estimado de los insumos consumidos (alimento/medicina) basándose en el precio promedio de compra.
2. **Gestión de Stock Crítico:** Se añadió el campo `UmbralMinimo` a la entidad `Producto`. El Dashboard ahora reporta una lista de `AlertasStockMinimo` para cualquier producto cuya existencia física sea inferior a dicho umbral, permitiendo una reposición proactiva.
3. **Formalización de Compras:** Se implementó `RegistrarIngresoMercaderiaCommand`. A diferencia del movimiento de inventario genérico, este comando captura el `CostoTotalCompra` y el `Proveedor`, permitiendo alimentar el motor de costos promedio y mejorar la precisión de los reportes financieros.
4. **Evolución del Modelo de Datos:** Se actualizaron las entidades `Producto` y `MovimientoInventario` y sus configuraciones en EF Core para soportar los nuevos campos de auditoría financiera y operativa.

### Endpoints Agregados/Actualizados
- `GET /api/dashboard/resumen` (Ahora incluye Inversión y Alertas de Stock)
- `POST /api/inventario/compras` (Nuevo endpoint para registro formal de entradas con costo)

## SPRINT 44: Flexibilidad del Calendario Sanitario

### Decisiones Tomadas
1. **Adaptabilidad del Plan Sanitario:** Se evolucionó el calendario sanitario para permitir desviaciones del plan original. Se implementaron los comandos `AgregarActividadManualCommand` y `ReprogramarActividadCommand`, permitiendo a los granjeros reaccionar ante brotes o cambios climáticos sin romper la lógica del sistema.
2. **Cálculo Dinámico de Días:** Dado que el sistema utiliza `DiaDeAplicacion` (entero relativo a la fecha de ingreso) para la programación, los nuevos comandos aceptan una `DateTime` y realizan el cálculo automático del día correspondiente basado en el `Lote.FechaIngreso`, simplificando la experiencia del usuario final.
3. **Auditoría y Trazabilidad:** Se añadieron los campos `Tipo`, `EsManual` y `Justificacion` a la entidad `CalendarioSanitario`. Toda reprogramación o adición manual requiere ahora una justificación obligatoria, la cual queda registrada para auditorías posteriores, cumpliendo con la directiva de "dejar rastro" en operaciones que alteren la planificación.
4. **Estado de Actividades:** Se añadió el estado `Cancelado` al flujo del calendario, permitiendo descartar actividades programadas que ya no sean necesarias debido a cambios en la estrategia sanitaria del lote.

### Endpoints Agregados/Actualizados
- `POST /api/calendario/actividad-manual` (Añadir tareas fuera de plantilla)
- `PUT /api/calendario/{id}/reprogramar` (Mover fecha de tarea pendiente con justificación)

# BITÁCORA DE ARQUITECTURA - FASE 2.4: CIERRE FINANCIERO Y KARDEX AVANZADO

## SPRINT 45: Polímero de Ventas y CRM Básico

### Decisiones Tomadas
1. **Flexibilidad en Ventas (Actualización Atómica):** Se implementó `ActualizarVentaCommand` para permitir la corrección de errores en peso y precio. 
   - **Cálculo de Saldos:** El sistema recalcula automáticamente el `Total` de la venta. Dado que el `SaldoPendiente` es una propiedad calculada en la entidad `Venta` (`Total - PagosSum`), se garantiza que el saldo refleje siempre la realidad financiera sin redundancia de datos.
   - **Estado de Pago Dinámico:** Se ajustó la lógica de `ActualizarEstadoPagoSegunSaldos` para manejar pagos excedentes (saldos negativos), marcando la venta como `Pagada` si el saldo es menor o igual a cero.
   - **Integridad Biológica en Ventas:** Si la `CantidadPollos` es modificada en una venta existente, el sistema utiliza el nuevo método `Lote.CorregirVenta` para ajustar de forma atómica la `CantidadActual` y `PollosVendidos` del lote asociado.

2. **CRM e Historial de Cliente:** Se implementó `ObtenerHistorialClienteQuery` que devuelve todas las ventas de un cliente ordenadas cronológicamente. Se enriqueció el DTO `VentaResponse` para incluir el `SaldoPendiente` y el `EstadoPago`, permitiendo al frontend mostrar estados de cuenta claros para cobranzas.

### Endpoints Agregados/Actualizados
- `PUT /api/ventas/{id}` (Actualización de datos operativos)
- `GET /api/clientes/{id}/historial` (Historial transaccional del cliente)

## SPRINT 46: Auditoría de Inventario (Kárdex Real) y Cierres

### Decisiones Tomadas
1. **Algoritmo de Kárdex en Memoria:** Para garantizar la precisión contable del inventario, el `ObtenerKardexProductoQueryHandler` realiza el cálculo del `SaldoAcumulado` iterando cronológicamente sobre todos los movimientos del producto.
   - **Impacto por Tipo:** Se normalizó el impacto de cada movimiento: `Entrada`, `Compra` y `AjusteEntrada` suman al saldo; `Salida` y `AjusteSalida` restan.
   - **Trazabilidad:** Cada fila del Kárdex incluye la `Justificacion` y el `LoteId` opcional, permitiendo auditar el destino exacto de cada insumo.

2. **Agrupación Financiera de Gastos:** Se implementó `ObtenerGastosPorCategoriaQuery` para facilitar la generación de gráficos de torta/barras en el dashboard. El handler agrupa los gastos por `TipoGasto` en un rango de fechas, devolviendo el total acumulado por cada categoría.

3. **Refuerzo de Seguridad en Lotes:** Se migró el endpoint de reapertura de lotes a `PUT /api/lotes/{id}/reabrir` y se confirmó su restricción estricta al rol `Admin`. Esto asegura que solo usuarios con máxima autoridad puedan revertir cierres contables.

### Endpoints Agregados/Actualizados
- `GET /api/inventario/productos/{id}/kardex` (Trazabilidad con saldo acumulado)
- `GET /api/finanzas/gastos-por-categoria` (Reporte agrupado para analítica)
- `PUT /api/lotes/{id}/reabrir` (Estandarización de método y seguridad)
