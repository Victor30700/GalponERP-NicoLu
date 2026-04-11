# CONTRATO DE API - GALPON ERP

Todos los endpoints requieren autenticación mediante **JWT Bearer Token** (Firebase), excepto el endpoint de Login.

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
  "ventas": [],
  "historialMortalidad": [],
  "gastos": []
}
```

### Crear Lote
- **URL:** `/api/Lotes`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Entrada (JSON):**
```json
{
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
  "utilidadNeta": 5000.00
}
```

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

### Obtener Mortalidad por Lote
- **URL:** `/api/Mortalidad/lote/{loteId}`
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
- **Salida (JSON):** `"3fa85f64-5717-4562-b3fc-2c963f66afa6"` (ID del registro)

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
- **Salida (JSON):** `"3fa85f64-5717-4562-b3fc-2c963f66afa6"` (ID del pesaje)

### Obtener Pesajes por Lote
- **URL:** `/api/Pesajes/lote/{loteId}`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Salida (JSON):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "fecha": "2026-04-11T10:00:00Z",
    "pesoPromedioGramos": 1250.5,
    "cantidadMuestreada": 50
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
- **Salida (JSON):** `"3fa85f64-5717-4562-b3fc-2c963f66afa6"` (ID del gasto)

### Obtener Gastos Operativos
- **URL:** `/api/Gastos`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Query Params:** `galponId` (Guid, opcional), `loteId` (Guid, opcional)
- **Salida (JSON):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "galponId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "descripcion": "Pago Luz Abril",
    "monto": { "valor": 45.50, "codigo": "USD" },
    "fecha": "2026-04-10T00:00:00Z",
    "tipoGasto": "Servicios"
  }
]
```

### Obtener Calendario Sanitario por Lote
- **URL:** `/api/CalendarioSanitario/{loteId}`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Salida (JSON):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "diaDeAplicacion": 7,
    "descripcionTratamiento": "Vacuna Newcastle",
    "estado": 0
  }
]
```

### Marcar Vacuna/Tratamiento como Aplicado
- **URL:** `/api/CalendarioSanitario/{actividadId}/aplicar`
- **Método:** `PUT`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Salida:** `204 No Content`

## 3. INVENTARIO

### Registrar Movimiento de Inventario
- **URL:** `/api/Inventario/movimiento`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Entrada (JSON):**
```json
{
  "productoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "cantidad": 50.5,
  "tipo": 0,
  "fecha": "2026-04-10T10:00:00Z",
  "justificacion": "Compra inicial"
}
```
*(Nota: Tipo 0=Entrada, 1=Salida)*

- **Salida (JSON):**
```json
{
  "movimientoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Obtener Movimientos de Inventario (Kardex)
- **URL:** `/api/Inventario/movimientos`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Salida (JSON):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "productoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "nombreProducto": "Balanceado Inicio",
    "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "cantidad": 50.5,
    "tipo": "Entrada",
    "fecha": "2026-04-10T10:00:00Z"
  }
]
```

### Obtener Stock Actual (Global o por Producto)
- **URL:** `/api/Inventario/stock`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Query Params:** `productoId` (Guid, opcional)
- **Salida (JSON):**
```json
[
  {
    "productoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "nombreProducto": "Balanceado Inicio",
    "tipoProducto": "Alimento",
    "stockActual": 1250.75,
    "unidadMedida": "Kg"
  }
]
```

### Obtener Stock de un Producto Específico
- **URL:** `/api/Inventario/productos/{id}/stock`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Salida (JSON):**
```json
{
  "productoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "nombreProducto": "Balanceado Inicio",
  "tipoProducto": "Alimento",
  "stockActual": 1250.75,
  "unidadMedida": "Kg"
}
```

### Realizar Ajuste de Inventario
- **URL:** `/api/Inventario/ajuste`
- **Método:** `PUT`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Entrada (JSON):**
```json
{
  "productoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "cantidad": 5.0,
  "tipo": 1,
  "fecha": "2026-04-11T10:00:00Z",
  "justificacion": "Saco roto en bodega"
}
```
*(Nota: Aunque se envíe Tipo 0 o 1, el sistema lo registrará internamente como **AjusteEntrada** o **AjusteSalida** para no afectar los cálculos biológicos de FCR/Consumo).*

- **Salida (JSON):**
```json
{
  "ajusteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Verificar Niveles de Alimento
- **URL:** `/api/Inventario/niveles-alimento`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Salida (JSON):**
```json
{
  "stockActualAlimento": 2500.50,
  "consumoDiarioGlobal": 150.75,
  "diasRestantes": 16.5,
  "requiereAlerta": false
}
```

## 4. VENTAS

### Obtener Todas las Ventas
- **URL:** `/api/Ventas`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Salida (JSON):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "clienteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "clienteNombre": "Distribuidora Avícola",
    "fecha": "2026-04-10T15:00:00Z",
    "cantidadPollos": 100,
    "pesoTotalKg": 250.5,
    "precioPorKilo": 2.20,
    "total": 551.10
  }
]
```

### Obtener Detalle de una Venta
- **URL:** `/api/Ventas/{id}`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Salida (JSON):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "clienteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "clienteNombre": "Distribuidora Avícola",
  "fecha": "2026-04-10T15:00:00Z",
  "cantidadPollos": 100,
  "pesoTotalKg": 250.5,
  "precioPorKilo": 2.20,
  "total": 551.10
}
```

### Registrar Venta Parcial
- **URL:** `/api/Ventas/parcial`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Entrada (JSON):**
```json
{
  "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "clienteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fecha": "2026-04-10T15:00:00Z",
  "cantidadPollos": 100,
  "pesoTotalVendido": 250.5,
  "precioPorKilo": 2.20
}
```
- **Salida (JSON):**
```json
{
  "ventaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

## 5. PLANIFICACIÓN

### Obtener Simulación de Rentabilidad
- **URL:** `/api/Planificacion/simulacion`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Entrada (Query Params):**
  - `cantidadPollos` (int)
  - `pesoEsperadoPorPolloKg` (decimal)
  - `precioAlimentoPorKg` (decimal)
  - `precioVentaPorKg` (decimal)
  - `fcrPersonalizado` (decimal, opcional)
- **Salida (JSON):**
```json
{
  "cantidadPollos": 5000,
  "mortalidadEstimada": 150,
  "kilosCarneProyectados": 12125.0,
  "totalIngresosVenta": 66687.5,
  "consumoAlimentoTotal": 20612.5,
  "costoAlimentoTotal": 41225.0,
  "otrosCostosVariables": 7500.0,
  "costoTotalEstimado": 48725.0,
  "utilidadEstimada": 17962.5,
  "roi": 36.8,
  "puntoEquilibrioKilos": 8859.1
}
```

## 6. USUARIOS

### Obtener Perfil del Usuario Actual (Me)
- **URL:** `/api/Usuarios/me`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Salida (JSON):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firebaseUid": "FIREBASE_UID_STRING",
  "email": "usuario@ejemplo.com",
  "nombre": "Juan Pérez",
  "rol": "Admin|SubAdmin|Empleado",
  "isActive": true
}
```

### Registrar Usuario
- **URL:** `/api/Usuarios`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)
- **Entrada (JSON):**
```json
{
  "firebaseUid": "FIREBASE_UID_STRING",
  "nombre": "Juan",
  "apellidos": "Pérez",
  "fechaNacimiento": "1990-05-15T00:00:00Z",
  "direccion": "Av. Principal 123",
  "profesion": "Ingeniero Agrónomo",
  "rol": "Empleado|SubAdmin|Admin"
}
```
- **Salida (JSON):**
```json
{
  "usuarioId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Obtener Todos los Usuarios
- **URL:** `/api/Usuarios`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)
- **Entrada:** N/A
- **Salida (JSON):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "firebaseUid": "FIREBASE_UID_STRING",
    "nombre": "Nombre del Usuario",
    "rol": "Admin|SubAdmin|Empleado"
  }
]
```

### Actualizar Usuario
- **URL:** `/api/Usuarios/{id}`
- **Método:** `PUT`
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)
- **Entrada (JSON):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "nombre": "Nuevo Nombre",
  "rol": "Empleado|SubAdmin|Admin"
}
```
- **Salida:** `204 No Content`

### Eliminar Usuario (Soft Delete)
- **URL:** `/api/Usuarios/{id}`
- **Método:** `DELETE`
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)
- **Salida:** `204 No Content`

## 7. GALPONES

### Crear Galpón
- **URL:** `/api/Galpones`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Entrada (JSON):**
```json
{
  "nombre": "Galpón 01",
  "capacidad": 5000,
  "ubicacion": "Zona Norte"
}
```
- **Salida (JSON):**
```json
{
  "galponId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Listar Galpones
- **URL:** `/api/Galpones`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Salida (JSON):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "nombre": "Galpón 01",
    "capacidad": 5000,
    "ubicacion": "Zona Norte",
    "isActive": true
  }
]
```

### Editar Galpón
- **URL:** `/api/Galpones/{id}`
- **Método:** `PUT`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Entrada (JSON):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "nombre": "Galpón 01 Modificado",
  "capacidad": 5500,
  "ubicacion": "Zona Norte"
}
```
- **Salida:** `204 No Content`

## 8. CATÁLOGOS (DEPRECADO)

### Obtener Clientes
- **URL:** `/api/Catalogos/clientes`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)

### Obtener Productos
- **URL:** `/api/Catalogos/productos`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)

## 9. CLIENTES

### Crear Cliente
- **URL:** `/api/Clientes`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Entrada (JSON):**
```json
{
  "nombre": "Distribuidora Avícola",
  "telefono": "123456789",
  "direccion": "Calle Falsa 123"
}
```
- **Salida (JSON):**
```json
{
  "clienteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Listar Clientes
- **URL:** `/api/Clientes`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Salida (JSON):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "nombre": "Distribuidora Avícola",
    "telefono": "123456789",
    "isActive": true
  }
]
```

### Actualizar Cliente
- **URL:** `/api/Clientes/{id}`
- **Método:** `PUT`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Entrada (JSON):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "nombre": "Nombre Actualizado",
  "telefono": "987654321",
  "direccion": "Nueva Dirección 456"
}
```
- **Salida:** `204 No Content`

### Eliminar Cliente (Soft Delete)
- **URL:** `/api/Clientes/{id}`
- **Método:** `DELETE`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Salida:** `204 No Content`

## 10. PRODUCTOS

### Crear Producto
- **URL:** `/api/Productos`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Entrada (JSON):**
```json
{
  "nombre": "Balanceado Inicio",
  "tipo": "Alimento",
  "unidadMedida": "Kg"
}
```
- **Salida (JSON):**
```json
{
  "productoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Listar Productos
- **URL:** `/api/Productos`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Salida (JSON):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "nombre": "Balanceado Inicio",
    "tipo": "Alimento",
    "unidadMedida": "Kg",
    "isActive": true
  }
]
```

### Actualizar Producto
- **URL:** `/api/Productos/{id}`
- **Método:** `PUT`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Entrada (JSON):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "nombre": "Nombre Actualizado",
  "tipo": "Alimento",
  "unidadMedida": "Libras"
}
```
- **Salida:** `204 No Content`

### Eliminar Producto (Soft Delete)
- **URL:** `/api/Productos/{id}`
- **Método:** `DELETE`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Salida:** `204 No Content`

## 11. DASHBOARD

### Obtener Resumen Dashboard
- **URL:** `/api/Dashboard/resumen`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Salida (JSON):**
```json
{
  "totalPollosActivos": 15000,
  "mortalidadPromedio": 2.5,
  "fcrPromedio": 1.65,
  "utilidadProyectada": 25000.00
}
```

### Obtener Proyección de Sacrificio
- **URL:** `/api/Dashboard/proyeccion-sacrificio/{loteId}`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Salida (JSON):**
```json
{
  "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "pesoActualGramos": 1250.5,
  "fcrActual": 1.62,
  "diasDeVida": 25,
  "gananciaDiariaEstimadaGramos": 48.5,
  "pesoObjetivoGramos": 2500.0,
  "diasRestantes": 26,
  "fechaSacrificio": "2026-05-07T10:00:00Z"
}
```

### Obtener Comparativa de Lotes
- **URL:** `/api/Dashboard/comparativa-lotes`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Salida (JSON):**
```json
[
  {
    "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "fechaIngreso": "2026-04-10T00:00:00Z",
    "cantidadInicial": 5000,
    "mortalidadTotal": 150,
    "fcrFinal": 1.65,
    "totalVentas": 15000.00,
    "totalGastos": 8000.00,
    "utilidadNeta": 7000.00
  }
]
```
