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

## 0.1 CONFIGURACIÓN GLOBAL

### Obtener Configuración
- **URL:** `/api/Configuracion`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Salida (JSON):**
```json
{
  "nombreEmpresa": "Pollos NicoLu",
  "nit": "000000000-0",
  "telefono": "+591 ...",
  "email": "contacto@nicolu.com",
  "direccion": "Sector ...",
  "monedaPorDefecto": "USD",
  "logoUrl": "..."
}
```

### Actualizar Configuración
- **URL:** `/api/Configuracion`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)
- **Entrada (JSON):** Mismo formato que la salida de GET.
- **Salida:** `204 No Content`

## 1. GALPONES

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

### Crear Galpón
- **URL:** `/api/Galpones`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Entrada (JSON):**
```json
{
  "nombre": "Galpón B",
  "capacidad": 5000,
  "ubicacion": "Sector Sur"
}
```

### Editar Galpón
- **URL:** `/api/Galpones/{id}`
- **Método:** `PUT`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)
- **Entrada (JSON):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "nombre": "Galpón A Modificado",
  "capacidad": 5500,
  "ubicacion": "Sector Norte Actualizado"
}
```

### Eliminar Galpón
- **URL:** `/api/Galpones/{id}`
- **Método:** `DELETE`
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)
- **Nota:** Soft Delete.

## 2. LOTES

### Listar Lotes
- **URL:** `/api/Lotes`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Query Params:** `soloActivos` (bool, default: true)

### Obtener Detalle de Lote
- **URL:** `/api/Lotes/{id}`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)

### Crear Lote
- **URL:** `/api/Lotes`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)

### Actualizar Lote (Datos Iniciales)
- **URL:** `/api/Lotes/{id}`
- **Método:** `PUT`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin**)

### Eliminar Lote
- **URL:** `/api/Lotes/{id}`
- **Método:** `DELETE`
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)

### Cancelar Lote
- **URL:** `/api/Lotes/{id}/cancelar`
- **Método:** `POST`

### Trasladar Lote
- **URL:** `/api/Lotes/{id}/trasladar`
- **Método:** `POST`

### Obtener Rendimiento en Vivo
- **URL:** `/api/Lotes/{id}/rendimiento-vivo`
- **Método:** `GET`

### Cerrar Lote
- **URL:** `/api/Lotes/{id}/cerrar`
- **Método:** `POST`

### Reabrir Lote
- **URL:** `/api/Lotes/{id}/reabrir`
- **Método:** `PUT`
- **Autenticación:** Requerida (Bearer, Rol: **Admin**)

### Obtener Reporte de Cierre (PDF)
- **URL:** `/api/Lotes/{id}/reporte-cierre-pdf`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer, Rol: **Admin, SubAdmin, Empleado**)
- **Salida:** Binary stream (`application/pdf`)

## 3. OPERACIONES DIARIAS

### Mortalidad (Bajas)
- `GET /api/Mortalidad` (Listar histórico)
- `GET /api/Mortalidad/{id}` (Obtener por ID)
- `GET /api/Mortalidad/lote/{loteId}` (Obtener por Lote)
- `GET /api/Mortalidad/lote/{loteId}/tendencias` (Analítica por semana)
- `POST /api/Mortalidad` (Registrar)
- `PUT /api/Mortalidad/{id}` (Actualizar)
- `DELETE /api/Mortalidad/{id}` (Eliminar - Solo Admin)

### Pesajes
- `GET /api/Pesajes/{id}` (Obtener por ID)
- `POST /api/Pesajes` (Registrar)
- `PUT /api/Pesajes/{id}` (Actualizar)
- `DELETE /api/Pesajes/{id}` (Eliminar - Solo Admin)

### Gastos Operativos
- `GET /api/Gastos` (Listar con filtros `galponId` o `loteId`)
- `GET /api/Gastos/{id}` (Obtener por ID)
- `POST /api/Gastos` (Registrar)
- `PUT /api/Gastos/{id}` (Actualizar)
- `DELETE /api/Gastos/{id}` (Eliminar - Solo Admin)

## 4. VENTAS Y COBRANZAS

### Ventas
- `POST /api/Ventas/parcial` (Registrar venta a crédito)
- `PUT /api/Ventas/{id}` (Actualizar datos de venta)
- `POST /api/Ventas/{id}/anular` (Anular venta - Solo Admin)

### Pagos de Ventas
- `GET /api/Ventas/{id}/pagos` (Listar todos los pagos de una venta)
- `POST /api/Ventas/{id}/pagos` (Registrar pago/abono)
- `DELETE /api/Ventas/{id}/pagos/{pagoId}` (Anular pago - Solo Admin)

## 5. INVENTARIO Y COMPRAS

### Stock y Valoración
- `GET /api/inventario/stock` (Stock actual normalizado y en biomasa)
- `GET /api/inventario/valoracion` (Valoración de bodega según PPP)
- `GET /api/inventario/proyecciones` (Días de alimento restantes)
- `GET /api/inventario/productos/{id}/kardex` (Historial de movimientos con saldo acumulado)
- `GET /api/inventario/ajustes` (Reporte de ajustes manuales con justificación)
- `POST /api/inventario/conciliacion` (Ajuste masivo por inventario físico)

### Consumo Operativo
- `POST /api/inventario/consumo-diario` (Registrar alimentación diaria)

### Compras a Proveedores
- `GET /api/inventario/compras` (Listar facturas de compra)
- `POST /api/inventario/compras` (Registrar ingreso de mercadería con costo y proveedor)

### Pagos de Compras (Cuentas por Pagar)
- `GET /api/inventario/compras/{id}/pagos` (Listar pagos a una factura)
- `POST /api/inventario/compras/{id}/pagos` (Registrar abono a proveedor)
- `DELETE /api/inventario/compras/{compraId}/pagos/{pagoId}` (Anular pago a proveedor - Solo Admin)

## 6. SANIDAD Y CALENDARIO

### Plantillas
- `GET /api/Plantillas` (Listar)
- `POST /api/Plantillas` (Crear)
- `PUT /api/Plantillas/{id}` (Actualizar)
- `DELETE /api/Plantillas/{id}` (Eliminar - Solo Admin)

### Calendario del Lote
- `GET /api/calendario/{loteId}` (Ver plan del lote)
- `PATCH /api/calendario/{id}/aplicar` (Marcar como aplicada + descuento stock)
- `POST /api/calendario/actividad-manual` (Añadir actividad fuera de plan)
- `PUT /api/calendario/{id}/reprogramar` (Mover fecha con justificación)

## 7. INTELIGENCIA FINANCIERA (DASHBOARD)

- `GET /api/Dashboard/resumen` (Kpi: Pollos vivos, CxC, Inversión, Alertas Stock)
- `GET /api/Dashboard/comparativa-galpones` (Eficiencia por galpón físico)
- `GET /api/Finanzas/flujo-caja` (Ingresos vs Egresos en tiempo real)
- `GET /api/Finanzas/flujo-proyectado` (Estimación de liquidez a 30 días)
- `GET /api/Finanzas/cuentas-por-cobrar` (Cartera de clientes)
- `GET /api/Finanzas/cuentas-por-pagar` (Deuda con proveedores)
- `GET /api/Finanzas/gastos-por-categoria` (Analítica de costos operativos)

## 8. MAESTROS (CRUD BÁSICO)

### Clientes
- `GET /api/Clientes`
- `GET /api/Clientes/{id}`
- `GET /api/Clientes/{id}/historial` (CRM: Estado de cuenta)
- `POST /api/Clientes`
- `PUT /api/Clientes/{id}`
- `DELETE /api/Clientes/{id}` (Solo Admin)

### Proveedores
- `GET /api/Proveedores`
- `GET /api/Proveedores/{id}`
- `GET /api/Proveedores/{id}/historial` (CRM: Compras realizadas)
- `POST /api/Proveedores`
- `PUT /api/Proveedores/{id}`
- `DELETE /api/Proveedores/{id}` (Solo Admin)

### Productos
- `GET /api/Productos`
- `GET /api/Productos/{id}`
- `POST /api/Productos`
- `PUT /api/Productos/{id}`
- `DELETE /api/Productos/{id}` (Solo Admin)

### Categorías y Unidades
- `GET /api/Categorias` ...
- `GET /api/UnidadesMedida` ...

## 9. AUDITORÍA Y SEGURIDAD

### Logs y Recuperación
- `GET /api/Auditoria/logs` (Trazabilidad de cambios críticos)
- `PATCH /api/Auditoria/restaurar/{entidad}/{id}` (Recuperar registros eliminados - Solo Admin)
- `GET /api/Usuarios/me` (Sesión actual)
- `GET /api/Usuarios` ...
