# Instrucciones de Desarrollo: Refactorización de Productos e Inventario (Control de Alimento en Kg)

## 1. Contexto y Objetivo
Estamos trabajando en el proyecto Fullstack **GalponERP (Pollos NicoLu)**. Necesitamos modificar la forma en que se registran y gestionan los productos (específicamente la categoría "Alimento") para llevar un control exacto del inventario en Kilogramos y Unidades.

Actualmente, el registro de productos asume un modelo básico. Necesitamos agregar el **Peso Unitario (Kg)** y replantear el seguimiento del stock total para que, al registrar el consumo diario de los pollos en un lote, se descuenten los Kilogramos exactos del inventario global del producto.

## 2. Cambios Requeridos en la API (Contrato)
El endpoint de creación y edición de productos (`POST /api/Productos` y `PUT /api/Productos/{id}`) debe ser modificado.

**Payload Actual:**
```json
{
  "nombre": "string",
  "categoriaProductoId": "uuid",
  "unidadMedidaId": "uuid",
  "equivalenciaEnKg": 0,
  "umbralMinimo": 0,
  "stockInicial": 0
}
Nuevo Payload Esperado:

JSON
{
  "nombre": "string",
  "categoriaProductoId": "uuid",
  "unidadMedidaId": "uuid",
  "pesoUnitarioKg": 50, 
  "equivalenciaEnKg": 5000, 
  "umbralMinimo": 100,
  "stockInicial": 100
}
Nota de Negocio: pesoUnitarioKg representa cuánto pesa 1 unidad (ej. 1 saco = 50kg). equivalenciaEnKg pasará a representar el Peso Total Inicial (ej. 100 sacos * 50kg = 5000kg).

3. Flujo de Trabajo y Requerimientos Arquitectónicos
A. Backend (.NET - Clean Architecture & CQRS)
Domain: Actualizar la entidad Producto.cs para incluir PesoUnitarioKg.

Infrastructure: Actualizar la configuración de Entity Framework (ProductoConfiguration.cs) y generar la migración correspondiente.

Application: - Modificar CrearProductoCommand, ActualizarProductoCommand y sus respectivos validadores y Handlers.

Crucial: Revisar la lógica en RegistrarConsumoAlimentoCommandHandler (o el handler de inventario correspondiente). Al registrar un consumo diario (que vendrá en Kg), el sistema debe descontar esos Kg del total disponible (equivalenciaEnKg o el campo de tracking de peso total) y calcular cuántas unidades físicas representa para descontar el Stock.

B. Frontend (Next.js / React)
Módulo de Productos (/productos):

Actualizar el formulario de "Nuevo Producto" para incluir el input PesoUnitarioKg.

Implementar un cálculo reactivo: Cuando el usuario ingrese Stock Inicial (ej. 100) y Peso Unitario (ej. 50), el campo Peso Total / Equivalencia (ej. 5000) debe calcularse automáticamente para enviarse al backend.

Módulo de Operaciones (/lotes/[id] - Consumo):

Modificar la vista de "Registrar Alimento" diario.

Mostrar el stock restante dinámico (ej. "Disponibles: 4500 Kg").

Al registrar el consumo diario en Kg, preparar el payload para el endpoint correspondiente de la API de Operaciones/Inventario.

4. Archivos y Rutas de Destino (Strict Grounding)
Debes generar la salida de tu análisis en archivos locales específicos. Utiliza las siguientes rutas absolutas:

Plan de Trabajo (DESTINO 1): D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\agent\plan.md

Documentación Final (DESTINO 2): D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\agent\docs.md

5. Instrucciones de Ejecución Inmediata (¡ALTO AQUÍ!)
Tu ÚNICA tarea en este momento es analizar el proyecto (Backend y Frontend) y redactar el Plan de Trabajo en formato Markdown dentro del archivo agent\plan.md.

Este plan debe contener:

Los archivos exactos que vas a modificar en el Backend.

Los archivos exactos que vas a modificar en el Frontend.

Las consultas/comandos SQL o migraciones de EF Core a generar.

REGLA CRÍTICA: UNA VEZ GENERADO EL ARCHIVO plan.md, DETENTE POR COMPLETO. No escribas ni modifiques ningún código fuente de la aplicación. Espera explícitamente mis instrucciones para proceder con la implementación. Cuando yo te dé la orden, ejecutarás el código y, al finalizar, documentarás todo en docs.md.