# Instrucciones de Desarrollo: Refactorización Sección "Operación" - GalponERP

## 1. Contexto y Objetivo
Estamos trabajando en el frontend del proyecto **GalponERP (Pollos NicoLu)**. Específicamente en la vista de detalle de un lote (`/lotes/{loteId}`). 

El objetivo es transformar la sección de **"OPERACIÓN"** (donde se registran Bajas, Alimento, Agua y Pesajes). Actualmente solo permite la creación (POST), pero necesitamos implementar el ciclo completo (CRUD) y la visualización histórica con filtros.

## 2. Archivos de Referencia Obligatoria (Grounding)
Antes de generar cualquier código o propuesta, DEBES leer y analizar los siguientes archivos locales en mi máquina:

1.  **Mapa de Endpoints:** `D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\frontend\endpoints.md`
    * *Nota:* Préstale especial atención a la diferencia de nombres de campos entre las respuestas (GET) y los envíos (POST/PUT). No asumas que son iguales.
2.  **Plan de Trabajo (Destino):** `D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\frontend\plan.md`
    * Aquí es donde deberás escribir el paso a paso de la implementación antes de codificar.

## 3. Requerimientos Funcionales Estrictos
El agente debe implementar la lógica para que el usuario pueda:

* **Visualizar Historial:** Listar los registros de Mortalidad, Pesajes, Alimento y Agua asociados al `loteId` actual.
* **Filtrado Inteligente:** Implementar filtros por Fecha (Día), Mes y Año. Dado que son registros diarios, la interfaz debe permitir navegar fácilmente por el histórico.
* **Corregir Errores (Edición/Eliminación):** * Habilitar la edición (PUT) para casos donde el usuario ingresó mal una cantidad.
    * Habilitar la eliminación (DELETE) para registros accidentales.
* **Gestión de Pesajes:** Considerar que, a diferencia de las bajas o el alimento, el pesaje no es diario, pero se realiza múltiples veces durante la vida del lote.

## 4. Reglas Técnicas y de Arquitectura
* **Cero Invenciones:** Tienes prohibido inventar nombres de campos o rutas de API. Si un campo no está en `endpoints.md`, pregunta antes de asumir.
* **Seguridad:** Recuerda que el sistema utiliza un modelo de autenticación híbrido. El backend genera su propio JWT tras la validación con Firebase; asegúrate de que las cabeceras de las peticiones respeten este flujo.
* **Tipado:** Si el proyecto usa TypeScript, genera las interfaces basándote estrictamente en los JSON de ejemplo del archivo de endpoints.

## 5. Entregable Inmediato
No escribas el código del frontend todavía. Tu primera tarea es:
1.  Analizar `endpoints.md`.
2.  Generar un plan de trabajo detallado (incluyendo componentes a crear, manejo de estado para filtros y servicios de API) y guardarlo en: `D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\frontend\plan.md`.