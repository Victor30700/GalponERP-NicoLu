Ventas


GET
/api/Ventas


Code	Details
200	
Response body
Download
[
  {
    "id": "f19310c1-d268-4ac9-aead-01403ec8ac5a",
    "loteId": "d68785af-7f59-4e18-af4a-a172b66359bb",
    "clienteId": "3954a7ae-3826-4c00-89e6-557b7d5dced3",
    "clienteNombre": "test ppppppp",
    "fecha": "2026-04-11T20:42:51.375Z",
    "cantidadPollos": 1000,
    "pesoTotalKg": 2500,
    "precioPorKilo": 19.5,
    "total": 48750,
    "saldoPendiente": 48750,
    "estadoPago": "Pagado"
  }
]

GET
/api/Ventas/{id}

Name	Description
id *
string($uuid)
(path)
f19310c1-d268-4ac9-aead-01403ec8ac5a


Code	Details
200	
Response body
Download
{
  "id": "f19310c1-d268-4ac9-aead-01403ec8ac5a",
  "loteId": "d68785af-7f59-4e18-af4a-a172b66359bb",
  "clienteId": "3954a7ae-3826-4c00-89e6-557b7d5dced3",
  "clienteNombre": "test ppppppp",
  "fecha": "2026-04-11T20:42:51.375Z",
  "cantidadPollos": 1000,
  "pesoTotalKg": 2500,
  "precioPorKilo": 19.5,
  "total": 48750,
  "saldoPendiente": 48750,
  "estadoPago": "Pagado"
}

PUT
/api/Ventas/{id}

Name	Description
id *
string($uuid)
(path)
id

{
  "ventaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "cantidadPollos": 0,
  "pesoTotalVendido": 0,
  "precioPorKilo": 0
}

GET
/api/Ventas/lote/{loteId}

Name	Description
loteId *
string($uuid)
(path)
d68785af-7f59-4e18-af4a-a172b66359bb

200	
Response body
Download
[
  {
    "id": "f19310c1-d268-4ac9-aead-01403ec8ac5a",
    "loteId": "d68785af-7f59-4e18-af4a-a172b66359bb",
    "clienteId": "3954a7ae-3826-4c00-89e6-557b7d5dced3",
    "clienteNombre": "test ppppppp",
    "fecha": "2026-04-11T20:42:51.375Z",
    "cantidadPollos": 1000,
    "pesoTotalKg": 2500,
    "precioPorKilo": 19.5,
    "total": 48750,
    "saldoPendiente": 48750,
    "estadoPago": "Pagado"
  }
]


GET
/api/Ventas/{id}/pagos

Name	Description
id *
string($uuid)
(path)
3fa85f64-5717-4562-b3fc-2c963f66afa6


Code	Details
200	
Response body
Download
[]


POST
/api/Ventas/{id}/pagos

Name	Description
id *
string($uuid)
(path)
id

{
  "monto": 0,
  "fechaPago": "2026-04-14T19:29:02.548Z",
  "metodoPago": 1
}

POST
/api/Ventas/parcial

{
  "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "clienteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fecha": "2026-04-14T19:29:17.221Z",
  "cantidadPollos": 0,
  "pesoTotalVendido": 0,
  "precioPorKilo": 0
}

POST
/api/Ventas/{id}/anular

Name	Description
id *
string($uuid)
(path)
id


DELETE
/api/Ventas/{id}/pagos/{pagoId}

Name	Description
id *
string($uuid)
(path)
id
pagoId *
string($uuid)
(path)
pagoId