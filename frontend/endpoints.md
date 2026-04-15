Mortalidad


GET
/api/Mortalidad
200	
Response body
Download
[
  {
    "id": "040173a1-7227-48dd-9f27-946a8dfd8872",
    "loteId": "5c499942-d1a4-47f6-836f-731ff98760d5",
    "fecha": "2026-04-15T02:01:42.808Z",
    "cantidadBajas": 10,
    "causa": "bajas "
  },
  {
    "id": "879a3512-5962-425c-bae9-a97e4eb8e0e7",
    "loteId": "5c499942-d1a4-47f6-836f-731ff98760d5",
    "fecha": "2026-04-15T01:53:53.306Z",
    "cantidadBajas": 5,
    "causa": "Registro rutinario"
  },
  {
    "id": "5209d200-2e15-47b6-ac5d-470c1ae86705",
    "loteId": "77991288-37d9-4d19-bf74-b200f023bf47",
    "fecha": "2026-04-14T03:23:02.181Z",
    "cantidadBajas": 1,
    "causa": "Registro rutinario"
  },
  {
    "id": "ed42fa49-b889-4e65-8a29-edbf67242d65",
    "loteId": "77991288-37d9-4d19-bf74-b200f023bf47",
    "fecha": "2026-04-14T03:22:22.298Z",
    "cantidadBajas": 1,
    "causa": "Registro rutinario"
  },
  {
    "id": "66f5ddcf-c2e8-4fde-b516-3a3ee91e7fdc",
    "loteId": "d68785af-7f59-4e18-af4a-a172b66359bb",
    "fecha": "2026-04-14T03:14:09.005Z",
    "cantidadBajas": 10,
    "causa": "ddddddddd"
  },
  {
    "id": "b258236c-599f-4321-b242-0ace943977b2",
    "loteId": "d68785af-7f59-4e18-af4a-a172b66359bb",
    "fecha": "2026-04-11T20:02:49.709Z",
    "cantidadBajas": 50,
    "causa": "esto es una prueba"
  }
]


POST
/api/Mortalidad
{
  "loteId": "5c499942-d1a4-47f6-836f-731ff98760d5",
  "cantidad": 10,
  "causa": "bajas ",
  "fecha": "2026-04-15T02:01:42.808Z"
}

200	
Response body
Download
"040173a1-7227-48dd-9f27-946a8dfd8872"

GET
/api/Mortalidad/{id}

Name	Description
id *
string($uuid)
(path)
040173a1-7227-48dd-9f27-946a8dfd8872


Code	Details
200	
Response body
Download
{
  "id": "040173a1-7227-48dd-9f27-946a8dfd8872",
  "loteId": "5c499942-d1a4-47f6-836f-731ff98760d5",
  "fecha": "2026-04-15T02:01:42.808Z",
  "cantidadBajas": 10,
  "causa": "bajas ",
  "usuarioId": "6a95eb11-fe5d-4c93-8fcf-e0e648dcf97f"
}

PUT
/api/Mortalidad/{id}

Name	Description
id *
string($uuid)
(path)
040173a1-7227-48dd-9f27-946a8dfd8872

{
  "id": "040173a1-7227-48dd-9f27-946a8dfd8872",
  "cantidad": 10,
  "causa": "string",
  "fecha": "2026-04-15T02:08:08.351Z"
}

GET
/api/Mortalidad/lote/{loteId}

Name	Description
loteId *
string($uuid)
(path)
5c499942-d1a4-47f6-836f-731ff98760d5


Code	Details
200	
Response body
Download
[
  {
    "id": "040173a1-7227-48dd-9f27-946a8dfd8872",
    "loteId": "5c499942-d1a4-47f6-836f-731ff98760d5",
    "fecha": "2026-04-15T02:08:08.351Z",
    "cantidadBajas": 10,
    "causa": "string"
  },
  {
    "id": "879a3512-5962-425c-bae9-a97e4eb8e0e7",
    "loteId": "5c499942-d1a4-47f6-836f-731ff98760d5",
    "fecha": "2026-04-15T01:53:53.306Z",
    "cantidadBajas": 5,
    "causa": "Registro rutinario"
  }
]

GET
/api/Mortalidad/reporte-transversal

Name	Description
inicio
string($date-time)
(query)
inicio
fin
string($date-time)
(query)
fin


Code	Details
200	
Response body
Download
{
  "totalBajas": 77,
  "porCausa": [
    {
      "causa": "esto es una prueba",
      "cantidad": 50,
      "porcentaje": 64.93506493506493
    },
    {
      "causa": "ddddddddd",
      "cantidad": 10,
      "porcentaje": 12.987012987012987
    },
    {
      "causa": "string",
      "cantidad": 10,
      "porcentaje": 12.987012987012987
    },
    {
      "causa": "Registro rutinario",
      "cantidad": 7,
      "porcentaje": 9.090909090909092
    }
  ],
  "detalle": [
    {
      "id": "040173a1-7227-48dd-9f27-946a8dfd8872",
      "fecha": "2026-04-15T02:08:08.351Z",
      "lote": "5c499942",
      "cantidad": 10,
      "causa": "string"
    },
    {
      "id": "879a3512-5962-425c-bae9-a97e4eb8e0e7",
      "fecha": "2026-04-15T01:53:53.306Z",
      "lote": "5c499942",
      "cantidad": 5,
      "causa": "Registro rutinario"
    },
    {
      "id": "5209d200-2e15-47b6-ac5d-470c1ae86705",
      "fecha": "2026-04-14T03:23:02.181Z",
      "lote": "77991288",
      "cantidad": 1,
      "causa": "Registro rutinario"
    },
    {
      "id": "ed42fa49-b889-4e65-8a29-edbf67242d65",
      "fecha": "2026-04-14T03:22:22.298Z",
      "lote": "77991288",
      "cantidad": 1,
      "causa": "Registro rutinario"
    },
    {
      "id": "66f5ddcf-c2e8-4fde-b516-3a3ee91e7fdc",
      "fecha": "2026-04-14T03:14:09.005Z",
      "lote": "d68785af",
      "cantidad": 10,
      "causa": "ddddddddd"
    },
    {
      "id": "b258236c-599f-4321-b242-0ace943977b2",
      "fecha": "2026-04-11T20:02:49.709Z",
      "lote": "d68785af",
      "cantidad": 50,
      "causa": "esto es una prueba"
    }
  ]
}

GET
/api/Mortalidad/lote/{loteId}/tendencias

Name	Description
loteId *
string($uuid)
(path)
5c499942-d1a4-47f6-836f-731ff98760d5


Code	Details
200	
Response body
Download
[
  {
    "fecha": "Semana 1",
    "cantidad": 15,
    "porcentaje": 0.6,
    "semana": 1,
    "fcr": 0
  }
]

Pesajes

POST
/api/Pesajes

{
  "loteId": "5c499942-d1a4-47f6-836f-731ff98760d5",
  "fecha": "2026-04-15T02:10:59.482Z",
  "pesoPromedioGramos": 2.5,
  "cantidadMuestreada": 100
}


Code	Details
200	
Response body
Download
"985a8f4e-bd9c-4187-836d-c38f86695383"

GET
/api/Pesajes/{id}

Name	Description
id *
string($uuid)
(path)
985a8f4e-bd9c-4187-836d-c38f86695383

Code	Details
200	
Response body
Download
{
  "id": "985a8f4e-bd9c-4187-836d-c38f86695383",
  "loteId": "5c499942-d1a4-47f6-836f-731ff98760d5",
  "fecha": "2026-04-15T02:10:59.482Z",
  "pesoPromedioGramos": 2.5,
  "cantidadMuestreada": 100,
  "usuarioId": "6a95eb11-fe5d-4c93-8fcf-e0e648dcf97f"
}

PUT
/api/Pesajes/{id}

Name	Description
id *
string($uuid)
(path)
985a8f4e-bd9c-4187-836d-c38f86695383


Code	Details
204
Undocumented
Response headers
 date: Wed,15 Apr 2026 02:21:55 GMT 
 server: Kestrel 

DELETE
/api/Pesajes/{id}

Name	Description
id *
string($uuid)
(path)
id


GET
/api/Pesajes/lote/{loteId}

Name	Description
loteId *
string($uuid)
(path)
5c499942-d1a4-47f6-836f-731ff98760d5

Code	Details
200	
Response body
Download
[
  {
    "id": "985a8f4e-bd9c-4187-836d-c38f86695383",
    "loteId": "5c499942-d1a4-47f6-836f-731ff98760d5",
    "fecha": "2026-04-15T02:21:40.163Z",
    "pesoPromedioGramos": 10,
    "cantidadMuestreada": 100
  }
]