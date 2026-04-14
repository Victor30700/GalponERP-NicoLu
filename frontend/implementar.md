Proveedores


GET
/api/Proveedores


Code	Details
200	
Response body
Download
[
  {
    "id": "e835386f-0ae6-4001-819e-7c139fb7d174",
    "razonSocial": "gggfgfgfg",
    "nitRuc": "fgef43",
    "telefono": "5656565656",
    "email": "prove@gmail.com",
    "direccion": "streeeeeeing",
    "isActive": true
  }
]

POST
/api/Proveedores

{
  "razonSocial": "gggfgfgfg",
  "nitRuc": "fgef43",
  "telefono": "5656565656",
  "email": "prove@gmail.com",
  "direccion": "streeeeeeing"
}

201
Undocumented
Response body
Download
{
  "proveedorId": "e835386f-0ae6-4001-819e-7c139fb7d174"
}

GET
/api/Proveedores/{id}

Name	Description
id *
string($uuid)
(path)
e835386f-0ae6-4001-819e-7c139fb7d174


Code	Details
200	
Response body
Download
{
  "id": "e835386f-0ae6-4001-819e-7c139fb7d174",
  "razonSocial": "gggfgfgfg",
  "nitRuc": "fgef43",
  "telefono": "5656565656",
  "email": "prove@gmail.com",
  "direccion": "streeeeeeing",
  "isActive": true
}

PUT
/api/Proveedores/{id}

Name	Description
id *
string($uuid)
(path)
id

{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "razonSocial": "string",
  "nitRuc": "string",
  "telefono": "string",
  "email": "string",
  "direccion": "string"
}

DELETE
/api/Proveedores/{id}

Name	Description
id *
string($uuid)
(path)
id

GET
/api/Proveedores/{id}/historial

Name	Description
id *
string($uuid)
(path)
e835386f-0ae6-4001-819e-7c139fb7d174


200	
Response body
Download
[]



Sanidad


POST
/api/Sanidad/bienestar

{
  "loteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fecha": "2026-04-14T05:48:16.939Z",
  "temperatura": 0,
  "humedad": 0,
  "consumoAgua": 0,
  "observaciones": "string"
}


Code	Details
200	
Response body
Download
{
  "registroId": "3ad65e93-f439-4d75-96fb-ccc44885cc46"
}

UnidadesMedida


GET
/api/UnidadesMedida

200	
Response body
Download
[
  {
    "id": "bbad2736-6abd-439d-85ee-8f24e6c50006",
    "nombre": "Kilogramo",
    "abreviatura": "Kg"
  },
  {
    "id": "28feb567-5e7d-44a3-9784-6079cc086eff",
    "nombre": "Unidad",
    "abreviatura": "Und"
  },
  {
    "id": "41a833d0-b6d6-44b7-8b0b-fe013381ffff",
    "nombre": "Litro",
    "abreviatura": "L"
  },
  {
    "id": "f8be3169-fde2-419d-ba4a-39b3c9f37faa",
    "nombre": "Saco",
    "abreviatura": "Sc"
  },
  {
    "id": "22372a07-1f5e-45a0-bc2f-cd93cfc16116",
    "nombre": "testUpdate",
    "abreviatura": "test"
  }
]

POST
/api/UnidadesMedida

{
  "nombre": "string",
  "abreviatura": "string"
}


Code	Details
201
Undocumented
Response body
Download
"86ac5e87-02ff-4d6f-8250-c5f3b0932ab0"

GET
/api/UnidadesMedida/{id}

Name	Description
id *
string($uuid)
(path)
86ac5e87-02ff-4d6f-8250-c5f3b0932ab0


Code	Details
200	
Response body
Download
{
  "id": "86ac5e87-02ff-4d6f-8250-c5f3b0932ab0",
  "nombre": "string",
  "abreviatura": "string"
}

PUT
/api/UnidadesMedida/{id}

Name	Description
id *
string($uuid)
(path)
id

{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "nombre": "string",
  "abreviatura": "string"
}

DELETE
/api/UnidadesMedida/{id}

Name	Description
id *
string($uuid)
(path)
id
