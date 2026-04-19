# OBJETIVO
Implementar de principio a fin la generación, exposición y consumo de 9 reportes operativos y gerenciales. Esto abarca desde la refactorización de la infraestructura de PDFs en el backend (.NET), la creación de flujos de lectura granulares (Queries/Handlers), hasta la integración de la descarga segura de archivos binarios (Blobs) en el frontend (Next.js).

# REGLAS DE FLUJO DE TRABAJO (ESTRICTO)
1. **Auditoría de Componentes:** Debes analizar el estado actual de `GalponERP.Infrastructure/Reporting/PdfService.cs`, `IPdfService.cs` y los controladores de la API relacionados con Lotes, Inventario y Sanidad.
2. **Generación del Plan:** En `D:\\scripts-csharp\\Pollos_NicoLu\\Pollos-NicoLu\\agent\\plan_reportes_ejecucion.md`, crea un checklist técnico detallado separando la implementación por Fases (Infraestructura, CQRS Backend, API, Frontend), indicando los nombres exactos de los archivos que vas a crear o modificar.
3. **ESPERA:** Una vez generado el plan, DETENTE y espera mi aprobación para proceder.
4. **Implementación Real:** No solo describas los cambios. Usa tus herramientas para SOBREESCRIBIR y CREAR los archivos `.cs`, `.ts` y `.tsx` correspondientes, inyectando la lógica completa de generación, cálculo de métricas y descarga.
5. **Documentación:** Al finalizar, detalla en `D:\\scripts-csharp\\Pollos_NicoLu\\Pollos-NicoLu\\agent\\docs_reportes.md` la estructura de los DTOs creados, cómo se manejan los estilos compartidos en QuestPDF y cómo funciona la utilidad de descarga en el frontend.

# ALCANCE TÉCNICO POR CAPA

## 1. Refactorización de Infraestructura (Motor QuestPDF)
* **`PdfService.cs` y `IPdfService.cs`:** Modificar el servicio actual para extraer la lógica repetitiva. Crear submétodos privados como `ComposeHeader` (Logo, NIT, Título dinámico) y `ComposeFooter` (Paginación, Fecha, Marca de agua).
* **Tipado Estricto:** Reemplazar cualquier uso de objetos anónimos (`object` o reflexión) por DTOs fuertemente tipados en las firmas de la interfaz para los 9 reportes previstos.

## 2. Desarrollo de Casos de Uso (CQRS Backend)
* **Queries y Handlers:** Implementar los flujos de lectura aislados para los 3 grupos de reportes (A: Ciclo de Vida, B: Sanidad/Bienestar, C: Inventario/Cierre).
* **Lógica de Negocio en Handlers:** Los Handlers deben ser responsables de cruzar la información necesaria (ej. sumar salidas de alimento del `IInventarioRepository` en el reporte de consumo, o calcular la Mortalidad Acumulada y el FCR). El `PdfService` SOLO debe encargarse de dibujar (maquetar) los datos recibidos.

## 3. Exposición (Controladores API)
* **Endpoints de Descarga:** Crear o extender `LotesController`, `InventarioController`, etc., añadiendo endpoints GET (ej. `api/lotes/{id}/reportes/ingreso`).
* **Retorno Binario:** Asegurar que los controladores retornen un `FileContentResult` (o `File`) con el MIME type `application/pdf` y un nombre de archivo dinámico estructurado.

## 4. Integración Frontend (Next.js)
* **Utilidad Blob:** Implementar en `src/lib/api.ts` una función robusta para procesar respuestas HTTP con `responseType: 'blob'`, previniendo la corrupción de caracteres en la descarga del PDF.
* **Componentes UI:** Integrar botones de descarga (ej. `<BotonReporte />`) en las vistas correspondientes (`lotes/[id]`, inventario, sanidad) manejando estados de carga (loading spinners) mientras el backend procesa el documento.

# INSTRUCCIÓN DE RENDIMIENTO Y CALIDAD
NO asumas cálculos simplificados. Si el reporte exige "Conversión Alimenticia" o "Mortalidad Acumulada", la lógica matemática en C# debe ser precisa. La generación de la tabla visual en QuestPDF debe manejar correctamente el salto de página (`PageBreak`) para tablas largas (ej. bitácoras o historiales de mortalidad). Todo componente visual generado en PDF debe utilizar una paleta de colores y fuentes estandarizadas (Azul SAVCO).

# INICIO
Comienza auditando `GalponERP.Infrastructure/Reporting/PdfService.cs` y `IPdfService.cs`. Luego, genera el `plan.md` con los pasos exactos, reportes a construir y los archivos afectados para mi revisión.