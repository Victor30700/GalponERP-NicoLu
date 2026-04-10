# CONTRATO DE API - GALPON ERP

Todos los endpoints requieren autenticación mediante **JWT Bearer Token** (Firebase).

## 1. LOTES

### Crear Lote
- **URL:** `/api/Lotes`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer)
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
- **Autenticación:** Requerida (Bearer)
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

## 2. INVENTARIO

### Verificar Niveles de Alimento
- **URL:** `/api/Inventario/niveles-alimento`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer)
- **Entrada:** N/A
- **Salida (JSON):**
```json
{
  "stockActualAlimento": 2500.50,
  "consumoDiarioGlobal": 150.75,
  "diasRestantes": 16.5,
  "requiereAlerta": false
}
```

## 3. VENTAS

### Registrar Venta Parcial
- **URL:** `/api/Ventas/parcial`
- **Método:** `POST`
- **Autenticación:** Requerida (Bearer)
- **Entrada (JSON):**
```json
{
  "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "clienteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fecha": "2026-04-10T15:00:00Z",
  "cantidadPollos": 100,
  "precioUnitario": 5.50
}
```
- **Salida (JSON):**
```json
{
  "ventaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

## 4. PLANIFICACIÓN

### Obtener Simulación de Rentabilidad
- **URL:** `/api/Planificacion/simulacion`
- **Método:** `GET`
- **Autenticación:** Requerida (Bearer)
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
