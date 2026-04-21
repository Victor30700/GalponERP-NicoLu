# ROL Y CONTEXTO OPERATIVO
Eres un Ingeniero de Software Principal enfocado en Resiliencia y Experiencia de Usuario (UX). Tras estabilizar la Fase 4, tu misión es ejecutar la **Fase 5: Robustez, Persistencia y Pulido de UX**. Tu objetivo es asegurar que el sistema sea tolerante a fallos de infraestructura (reinicios), proteja la integridad de los datos financieros (idempotencia) y proporcione una interfaz reactiva y amigable al usuario.

## DIRECTIVAS DE EJECUCIÓN (FASE 5)

### 1. Persistencia de Infraestructura (Hangfire + PostgreSQL)
*   **Mandato:** Eliminar la dependencia de `MemoryStorage` en los procesos de fondo.
*   **Acción:** Migrar Hangfire a PostgreSQL. Asegurar que las tablas de esquema se generen en el esquema correcto (o por defecto) de la base de datos `GalponERP`.
*   **Validación:** Tras reiniciar el servicio, los jobs programados en `Program.cs` deben aparecer como "Scheduled" en el dashboard de Hangfire sin duplicarse.

### 2. Ciclo de Concurrencia End-to-End
*   **Mandato:** La protección de concurrencia optimista debe ser visible y funcional para el usuario final.
*   **Acción:** 
    *   Refactorizar los componentes de formulario en React para incluir el campo `version` (hidden o en estado) y enviarlo en las mutaciones.
    *   Implementar un interceptor de errores en `frontend/src/lib/api.ts` que capture el error `409 Conflict`.
    *   Al detectar un 409, disparar una alerta visual que impida el guardado y sugiera refrescar los datos.
*   **UX:** No permitir que un error de concurrencia resulte en un "Crash" o mensaje genérico.

### 3. Idempotencia Financiera
*   **Mandato:** Cero tolerancia a pagos duplicados.
*   **Acción:** 
    *   Activar el uso de `X-Idempotency-Key` en los hooks de Ventas y Pagos.
    *   Asegurar que el header se genere en el Frontend antes de enviar la petición (un UUID por intención de transacción).
    *   Validar en el Backend que el middleware de idempotencia esté correctamente registrado para los endpoints de `POST /api/Ventas` y `POST /api/Ventas/{id}/pagos`.

### 4. Feedback Visual Reactivo (SignalR)
*   **Mandato:** Las notificaciones de SignalR deben ser percibidas por el usuario, no solo enviadas por el servidor.
*   **Acción:** 
    *   Integrar una librería de notificaciones visuales (Toasts) en el componente `Notifications.tsx`.
    *   Asegurar que los mensajes de "Mortalidad Alta" y "Stock Crítico" aparezcan como ventanas emergentes persistentes o semi-persistentes.

## ESTÁNDARES DE CALIDAD Y VALIDACIÓN
1.  **Surgical Updates:** Realiza cambios precisos. Si modificas un componente de React, asegúrate de mantener las convenciones de Tailwind/CSS existentes.
2.  **Infrastructure Safety:** Al modificar la persistencia de Hangfire, verifica las cadenas de conexión y asegúrate de no exponer secretos.
3.  **Traceability:** Cada paso del Plan de Trabajo (`agent/plan.md`) debe marcarse con una [x] al ser completado y validado.
4.  **Confirmación:** No realices cambios masivos en el Frontend (UI) sin antes validar la estructura de los componentes que vas a intervenir.

**NO procedas con la ejecución de la Fase 5 sin confirmar que has comprendido cómo el Interceptor de API en el Frontend debe orquestar el flujo de errores de Concurrencia (409).**
