# CONTRATO DE API - GALPON ERP

Todos los endpoints requieren autenticación mediante **JWT Bearer Token** (Firebase), excepto el endpoint de Login.

**Nota sobre Auditoría:** El sistema registra automáticamente el `UsuarioId` de quien realiza la transacción a partir del Token JWT. El Frontend **NO** debe enviar el campo `UsuarioId` en ningún JSON de creación o registro; el backend lo extrae de forma segura desde la identidad del usuario autenticado.

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
  "utilidadNeta": 5000.00,
  "fcr": 1.65,
  "porcentajeMortalidad": 3.25
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
    "categoriaNombre": "Alimento",
    "stockActual": 1250.75,
    "unidadMedida": "Kilogramos",
    "unidadMedidaNombre": "Kilogramos"
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
  "categoriaNombre": "Alimento",
  "stockActual": 1250.75,
  "unidadMedida": "Kilogramos",
  "unidadMedidaNombre": "Kilogramos"
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

### Anular Venta
- **URL:** `/api/Ventas/{id}/anular`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)
- **Salida:** `204 No Content`
- **Nota:** Esta operación marca la venta como inactiva (Soft Delete) y devuelve la cantidad de pollos vendidos al inventario del lote, siempre que el lote no esté cerrado.

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
  "categoriaProductoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "unidadMedidaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "equivalenciaEnKg": 1.0
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
    "categoriaProductoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "categoriaNombre": "Alimento",
    "unidadMedidaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "unidadMedidaNombre": "Kilogramo",
    "equivalenciaEnKg": 1.0,
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
  "categoriaProductoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "unidadMedidaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "equivalenciaEnKg": 40.0
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

## 12. CATEGORÍAS (ADMIN)

### Listar Categorías
- **URL:** `/api/Categorias`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Salida (JSON):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "nombre": "Alimento",
    "descripcion": "Balanceados y forraje"
  }
]
```

### Crear Categoría
- **URL:** `/api/Categorias`
- **Método:** `POST`
- **Entrada (JSON):**
```json
{
  "nombre": "Nueva Categoría",
  "descripcion": "Descripción opcional"
}
```
- **Salida (JSON):** `"3fa85f64-5717-4562-b3fc-2c963f66afa6"` (ID)

## 13. UNIDADES DE MEDIDA (ADMIN)

### Listar Unidades
- **URL:** `/api/UnidadesMedida`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Salida (JSON):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "nombre": "Kilogramo",
    "abreviatura": "Kg"
  }
]
```

### Crear Unidad
- **URL:** `/api/UnidadesMedida`
- **Método:** `POST`
- **Entrada (JSON):**
```json
{
  "nombre": "Litro",
  "abreviatura": "L"
}
```
- **Salida (JSON):** `"3fa85f64-5717-4562-b3fc-2c963f66afa6"` (ID)


# BITÁCORA ARQUITECTÓNICA - GALPON ERP

## Decisión 14.1: Implementación de RBAC (Role-Based Access Control) con Enums
Se ha refactorizado el sistema de roles de un esquema basado en strings constantes a un `enum RolGalpon` numérico para mayor control y tipado fuerte.

**Jerarquía de Roles:**
*   `Empleado = 0`: Acceso operativo (Lotes, Mortalidad, Galpones, Inventario, Calendario).
*   `SubAdmin = 1`: Acceso de gestión (Ventas, Gastos, Productos, Clientes, Dashboard, Planificación).
*   `Admin = 2`: Acceso total (Gestión de Usuarios, Configuración del Sistema).

**Mecánica de Inyección:**
El `Rol` se almacena como un entero en la base de datos PostgreSQL mediante EF Core. Al validar el JWT de Firebase, se intercepta el evento `OnTokenValidated`, se busca al usuario por su `FirebaseUid` en la BD local y se inyecta un `ClaimTypes.Role` con el nombre del enum (Admin, SubAdmin, Empleado).

## Decisión 14.2: Migración RefactorRolesEnum
Se generó una migración no destructiva para convertir la columna `Rol` de `character varying(50)` a `integer`. Se recomienda que antes de aplicar la migración en producción, se limpien o mapeen los valores de texto existentes a sus equivalentes numéricos (Admin -> 2, etc.).

## Decisión 14.3: Blindaje de Controladores
Se aplicó el atributo `[Authorize(Roles = "Admin,SubAdmin,Empleado")]` según la criticidad del endpoint, asegurando que el principio de menor privilegio se cumpla.

## Decisión 15.1: Módulo de Pesajes y Cálculo de FCR
Se implementó la entidad `PesajeLote` para registrar el peso promedio de muestras de pollos. El Índice de Conversión Alimenticia (FCR) se calcula dinámicamente en la consulta de detalle del lote utilizando la fórmula:
`FCR = Alimento Consumido (Kg) / Incremento de Biomasa (Kg)`.

**Consideraciones Técnicas:**
- El peso inicial del pollito se estima en 40g para el cálculo del incremento.
- Se filtran los movimientos de inventario de tipo "Salida" para productos de tipo "Alimento" vinculados al lote.
- Se requiere al menos un registro de pesaje para obtener el FCR actual.

## Decisión 16.1: Venta basada en Peso
Se refactorizó la entidad `Venta` para alejarse de un modelo de "precio unitario por pollo" hacia uno de "peso total vendido y precio por kilo", alineándose con la realidad comercial de la industria avícola.

## Decisión 17.1: Identidad y Gestión de Catálogos
Se implementó el endpoint `/api/usuarios/me` para permitir al frontend obtener los datos del usuario logueado y su rol (RBAC) de forma atómica. Se completó la trazabilidad de catálogos permitiendo `Update` y `Soft Delete` en Clientes y Productos.

## Decisión 18.1: Trazabilidad de Inventario (Kardex) y Ajustes
Se habilitó la consulta histórica de movimientos para auditoría. Para los ajustes manuales (mermas/robos), se extendió la entidad `MovimientoInventario` con un campo `Justificacion` obligatorio para mantener la integridad de la bitácora de almacén.

## Decisión 19.1: Desacoplamiento de Históricos
Se crearon endpoints específicos para listar Ventas y Mortalidad fuera del contexto de un solo lote, permitiendo análisis transversales de la operación.

## Decisión 21.1: Limpieza de warnings y GoogleCredential
Se refactorizó la carga de credenciales de Firebase en `FirebaseAuthService` y `Program.cs` para utilizar `GoogleCredential.GetApplicationDefault()`. Para compatibilidad en desarrollo local con archivos JSON, se inyecta la ruta del archivo en la variable de entorno `GOOGLE_APPLICATION_CREDENTIALS` dinámicamente. Se eliminaron todos los warnings de compilación (posibles nulos y métodos obsoletos).

## Decisión 22.1: Accountability y Auditoría de Transacciones
Se implementó un sistema de auditoría obligatorio para todas las transacciones financieras (Ventas, Gastos) y operativas (Movimientos de Inventario, Pesajes, Mortalidad). 
1. **Identidad Segura:** El `UsuarioId` local (Guid) se extrae del JWT a través del `FirebaseUid` buscando en `IUsuarioRepository` en cada controlador.
2. **Inmutabilidad:** El `UsuarioId` se inyecta en los comandos de MediatR y se guarda permanentemente en la base de datos para saber exactamente quién registró cada acción.
3. **Migración Segura:** Se corrigió un error de casting automático en PostgreSQL para la columna `Rol` mediante una cláusula `USING (CASE ...)` en la migración `RefactorRolesEnum`, asegurando la integridad de los datos existentes.

## Decisión 22.2: Actualización de Contratos API
Se actualizó `endpoints.md` para notificar al Frontend que la auditoría es transparente; el cliente no debe (ni puede) enviar el `UsuarioId` en los payloads JSON, delegando la responsabilidad de identidad totalmente al Backend.

## Decisión 23.1: Flexibilidad Financiera en Objeto Moneda
Se eliminó la restricción de montos no negativos en el Value Object `Moneda`. Esto es necesario para representar correctamente conceptos de **Pérdida Neta** o **Utilidad Negativa** al cerrar un lote donde los costos superan los ingresos, evitando excepciones de negocio fatales durante el cierre contable.

## Decisión 23.2: Corrección de Filtros en Listado de Lotes
Se corrigió un error en `LoteRepository` donde el parámetro `soloActivos` no funcionaba correctamente debido a los filtros globales de EF Core. Se implementó `.IgnoreQueryFilters()` en la consulta de "todos los lotes" para asegurar que el sistema pueda distinguir correctamente entre lotes activos, cerrados y eliminados.

## Decisión 23.3: Integridad Contable y Snapshots de Cierre
Se implementó un sistema de "congelación" de datos al cerrar un lote para asegurar la inmutabilidad de los reportes históricos.
1. **Snapshots en Lote:** Se agregaron los campos `FCRFinal`, `CostoTotalFinal`, `UtilidadNetaFinal` y `PorcentajeMortalidadFinal` a la entidad `Lote`. Estos valores se calculan y guardan permanentemente en la base de datos al ejecutar el comando `CerrarLote`.
2. **Estado de Pago en Ventas:** Se introdujo el enum `EstadoPago` (`Pagado`, `Pendiente`, `Parcial`) en la entidad `Venta` para permitir el seguimiento de cuentas por cobrar. Por defecto, todas las ventas se registran como `Pagado`.
3. **Mecánica de Anulación Segura:** Se creó el caso de uso `AnularVentaCommand`. Esta operación realiza un **Soft Delete** de la venta (`IsActive = false`) y **devuelve automáticamente la cantidad de pollos vendidos al inventario del lote**, garantizando la consistencia del conteo biológico. Esta acción está restringida únicamente al rol `Admin` y solo es permitida si el lote asociado no ha sido cerrado.
4. **Cálculo de FCR de Cierre:** El FCR final se calcula sumando todos los movimientos de salida de productos tipo `Alimento` vinculados al lote y dividiéndolos por el peso total vendido.

## Decisión 24.1: Optimización de Base de Datos y Background Jobs
1. **Indexación Estratégica:** Se configuraron índices explícitos en las columnas de alta frecuencia de consulta: `Fecha` en `Ventas` y `MovimientosInventario`. Las llaves foráneas (`LoteId`, `ProductoId`, `ClienteId`) ya cuentan con índices implícitos creados por EF Core. Esto optimiza los reportes transversales y el cálculo de FCR en tiempo real.
2. **Automatización de Alertas Sanitarias:** Se implementó `AlertaSanitariaJob` como un `BackgroundService`. Este servicio escanea diariamente todos los lotes activos, calcula su edad actual (días desde ingreso) y compara contra el `CalendarioSanitario`. Cualquier actividad pendiente (Vacunas/Tratamientos) que deba aplicarse hasta la fecha actual es reportada mediante el Logger del sistema para acción inmediata.

## Decisón 25.1: Dominio Dinámico y Ancla Matemática (SaaS)
Se ha migrado la estructura de productos de Enums estáticos a un modelo de catálogos dinámicos para soportar la escalabilidad multi-tenant y la flexibilidad de tipos de insumos.

1. **Entidades de Catálogo:** Se crearon las entidades `CategoriaProducto` (Nombre, Descripcion) y `UnidadMedida` (Nombre, Abreviatura). Esto permite que cada usuario defina sus propios tipos de productos (Iniciador, Crecimiento, Vacuna Newcastle, etc.) y unidades (Saco 40kg, Frasco 50 dosis, Litro) sin cambios de código.
2. **Equivalencia en Kg (El Ancla):** Se introdujo la propiedad `EquivalenciaEnKg` en la entidad `Producto`. Esta es la decisión técnica más crítica: todos los cálculos de FCR e inventario ahora dependen de este multiplicador decimal.
   - *Ejemplo:* Si un producto es "Alimento Iniciador" y su unidad es "Saco 40kg", su `EquivalenciaEnKg` es `40.0`. Los movimientos de inventario se registran en "Sacos", pero el motor de FCR los procesa en "Kg" automáticamente.
3. **Refactorización de Producto:** Se eliminaron los enums `TipoProducto` y `UnidadMedida`. La entidad `Producto` ahora utiliza llaves foráneas (`CategoriaProductoId`, `UnidadMedidaId`) con navegación obligatoria y carga mediante `.Include()`.

## Decisión 26.1: Estrategia de Migración de Datos SaaS (Zero Data Loss)
Para evitar la pérdida de información de productos existentes durante el cambio de esquema de base de datos, se implementó una migración de EF Core personalizada:

1. **Procedimiento Up():**
   - Se crean las nuevas tablas `CategoriasProductos` y `UnidadesMedida`.
   - Se añaden las columnas de FK a `Productos` como nulables inicialmente.
   - Se realiza un **Seeding de Identidad**: Inserción de categorías y unidades por defecto mediante SQL directo para garantizar IDs consistentes.
   - **Mapeo Transaccional:** Se ejecutaron sentencias `UPDATE` para migrar los antiguos valores de texto (Enums) a los nuevos IDs de catálogo y asignar equivalencias por defecto (ej. Saco -> 40kg).
   - Se eliminan las columnas obsoletas y se aplica la restricción `NOT NULL`.

## Decisión 26.2: Refactorización del Motor de Cálculo (FCR y Stock)
Se actualizaron todos los casos de uso que consumen inventario para alinearse al nuevo modelo:
1. **Identificación de Alimento:** En `CerrarLote` y `ObtenerDetalleLote`, los productos se filtran ahora comparando `p.Categoria.Nombre == "Alimento"`.
2. **Cálculo de Consumo:** La fórmula de alimento consumido cambió de `Sum(Cantidad)` a `Sum(Cantidad * Producto.EquivalenciaEnKg)`, garantizando que el FCR sea siempre una relación Kg/Kg independientemente de la unidad de despacho.
3. **Normalización de Unidades:** El reporte de stock ahora muestra tanto el nombre de la categoría como la unidad de medida dinámica, mejorando la legibilidad para el usuario final.

## Decisión 26.3: Exposición de Catálogos y Seguridad
Se implementaron controladores específicos (`CategoriasController`, `UnidadesMedidaController`) protegidos bajo RBAC:
- **Lectura:** Disponible para `Admin, SubAdmin`.
- **Escritura/Anulación:** Restringida estrictamente a `Admin, SubAdmin`.
- **Soft Delete:** Todas las acciones de eliminación en catálogos utilizan el patrón `IsActive = false` heredado de la clase base `Entity`.
