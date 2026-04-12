# BITÁCORA DE ARQUITECTURA - FASE 3.1

## SPRINT 56: Conciliación de Almacén y Reportabilidad SaaS

### Decisiones Tomadas
1. **Motor de Conciliación Masiva:** Se implementó `RegistrarConciliacionStockCommand` para automatizar los ajustes tras inventarios físicos. El sistema:
   - Calcula el stock teórico (sistema) vs el físico (conteo).
   - Genera automáticamente movimientos de `AjusteEntrada` o `AjusteSalida` solo por la diferencia.
   - Valora estos ajustes automáticamente usando el PPP actual del producto para mantener la integridad contable.
2. **Infraestructura de Reportes PDF:** Se integró la librería `QuestPDF` para la generación de documentos profesionales. Se optó por una arquitectura de "Servicio de Infraestructura" (`PdfService`) inyectado mediante una interfaz en Application, permitiendo generar la "Ficha de Liquidación de Lote" de forma programática.
3. **Optimización de Descargas:** El endpoint `/api/lotes/{id}/reporte-cierre-pdf` devuelve un flujo de bytes con el MIME type `application/pdf`, permitiendo al navegador o aplicación móvil previsualizar o descargar el documento directamente.
