Productos


GET
/api/Productos


Code	Details
200	
Response body
Download
[
  {
    "id": "01ae0259-fd86-44db-a2d2-20afd565ed4e",
    "nombre": "medicamento",
    "categoriaId": "b126e519-c2b0-4ee9-ac76-e1b65815acc0",
    "categoriaNombre": "Medicamento",
    "unidadMedidaId": "41a833d0-b6d6-44b7-8b0b-fe013381ffff",
    "unidadMedidaNombre": "Litro",
    "pesoUnitarioKg": 0,
    "umbralMinimo": 10,
    "stockActual": 0,
    "stockActualKg": 0,
    "isActive": true,
    "tipoUnidad": 0,
    "equivalenciaEnKg": 0
  },
  {
    "id": "3619dcdb-6070-438f-a3a5-41bec1e38a15",
    "nombre": "medicacion",
    "categoriaId": "b126e519-c2b0-4ee9-ac76-e1b65815acc0",
    "categoriaNombre": "Medicamento",
    "unidadMedidaId": "ac6d09fd-8496-466f-8eed-a45a7b7d3e04",
    "unidadMedidaNombre": "Mililitros",
    "pesoUnitarioKg": 0,
    "umbralMinimo": 10,
    "stockActual": 10,
    "stockActualKg": 0,
    "isActive": true,
    "tipoUnidad": 0,
    "equivalenciaEnKg": 0
  },
  {
    "id": "14e5b850-9d94-4d0e-a171-58c2316aabbf",
    "nombre": "saco alimento",
    "categoriaId": "50fc56d1-9a43-4f74-a0a1-32ed4c0dd56d",
    "categoriaNombre": "Alimento",
    "unidadMedidaId": "bbad2736-6abd-439d-85ee-8f24e6c50006",
    "unidadMedidaNombre": "Kilogramo",
    "pesoUnitarioKg": 45,
    "umbralMinimo": 100,
    "stockActual": 0.94,
    "stockActualKg": 42.3,
    "isActive": true,
    "tipoUnidad": 0,
    "equivalenciaEnKg": 42.3
  },
  {
    "id": "de4e2f44-288f-4007-9768-91d97b66b3e0",
    "nombre": "saco",
    "categoriaId": "50fc56d1-9a43-4f74-a0a1-32ed4c0dd56d",
    "categoriaNombre": "Alimento",
    "unidadMedidaId": "bbad2736-6abd-439d-85ee-8f24e6c50006",
    "unidadMedidaNombre": "Kilogramo",
    "pesoUnitarioKg": 50,
    "umbralMinimo": 100,
    "stockActual": 0,
    "stockActualKg": 0,
    "isActive": true,
    "tipoUnidad": 0,
    "equivalenciaEnKg": 0
  }
]
 
POST
/api/Productos

{
  "nombre": "test test test test",
  "categoriaProductoId": "de4e2f44-288f-4007-9768-91d97b66b3e0",
  "unidadMedidaId": "41a833d0-b6d6-44b7-8b0b-fe013381ffff",
  "pesoUnitarioKg": 100,
  "umbralMinimo": 10,
  "stockInicial": 100
}


Code	Details
201
Undocumented
Response body
Download
{
  "productoId": "1ec0ac18-a86e-449b-96f4-bcf83e1235e4"
}

GET
/api/Productos/{id}

Name	Description
id *
string($uuid)
(path)
1ec0ac18-a86e-449b-96f4-bcf83e1235e4

Code	Details
200	
Response body
Download
{
  "id": "1ec0ac18-a86e-449b-96f4-bcf83e1235e4",
  "nombre": "string",
  "categoriaId": "50fc56d1-9a43-4f74-a0a1-32ed4c0dd56d",
  "categoriaNombre": "Alimento",
  "unidadMedidaId": "41a833d0-b6d6-44b7-8b0b-fe013381ffff",
  "unidadMedidaNombre": "Litro",
  "pesoUnitarioKg": 10,
  "umbralMinimo": 10,
  "stockActual": 100,
  "stockActualKg": 1000,
  "isActive": true,
  "tipoUnidad": 0,
  "equivalenciaEnKg": 1000
}

PUT
/api/Productos/{id}

Name	Description
id *
string($uuid)
(path)
id


{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "nombre": "string",
  "categoriaProductoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "unidadMedidaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "pesoUnitarioKg": 0,
  "umbralMinimo": 0
}

DELETE
/api/Productos/{id}

Name	Description
id *
string($uuid)
(path)
id
