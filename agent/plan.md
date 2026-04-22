# 🛠️ Plan de Trabajo: Refuerzo de Casos de Borde y Resiliencia (Post-Auditoría)

Este plan aborda los puntos ciegos detectados en el "Certificado de Calidad Técnica" para garantizar que GalponERP sea una roca en producción.

---

## 🎯 Fase 1: Blindaje de Concurrencia y Consistencia (Backend)
*Objetivo: Eliminar el riesgo de "Lost Updates" en todas las entidades maestras.*

1.  **Unificación de Tipos de Versión:**
    *   Estandarizar todos los `Actualizar...Command` para recibir `string? Version`.
    *   Asegurar que `ActualizarMortalidadCommand` cambie de `uint` a `string?`.
2.  **Cierre de Brechas en Handlers:**
    *   Implementar el chequeo de `ConcurrencyException` en los siguientes Handlers:
        *   `ActualizarMortalidadCommandHandler`
        *   `ActualizarUsuarioCommandHandler`
        *   `ActualizarGastoOperativoCommandHandler`
        *   `ActualizarBienestarCommandHandler`
        *   `ActualizarPesajeCommandHandler`
        *   `ActualizarFormulaCommandHandler`
3.  **Validación en Validators:**
    *   Asegurar que todos los `Actualizar...CommandValidator` tengan la regla:
        `RuleFor(x => x.Version).NotEmpty().WithMessage("La versión de concurrencia es obligatoria.");`

---

## 🏗️ Fase 2: Resiliencia de Infraestructura e Inicio (Core)
*Objetivo: Evitar "Crashes" en el despliegue de Docker/Producción.*

1.  **Robustez en `FirebaseAuthService.cs`:**
    *   Modificar el constructor para que, si no hay credenciales, registre un `LogError` crítico pero **no lance una excepción que detenga el hilo principal** (si es posible que la app funcione degradada) o, mejor aún, que lance una excepción con un mensaje claro de "Falta configuración de Firebase".
    *   Asegurar que las llamadas a `FirebaseAuth.DefaultInstance` verifiquen si la instancia fue inicializada correctamente.
2.  **Protección de Logs de Auditoría:**
    *   Revisar `AuditoriaBehavior.cs` y el `GlobalExceptionHandler` para asegurar que el header `x-api-key` sea omitido o enmascarado en los registros.

---

## 💻 Fase 3: Resiliencia del Frontend y UX (Next.js)
*Objetivo: Sincronización perfecta entre UI y Estado del Servidor.*

1.  **Auditoría de Hooks de Mutación:**
    *   Verificar que `useInventario.ts`, `useVentas.ts` y `useSanidad.ts` implementen `queryClient.cancelQueries` en su `onMutate`.
    *   Asegurar que todos los hooks de "Actualización" en el frontend capturen el error `CONCURRENCY_ERROR` y sugieran al usuario refrescar la página (UX unificado).
2.  **Estandarización de Idempotencia:**
    *   Verificar que todas las operaciones `POST` críticas en el frontend (Registrar Venta, Registrar Compra, Registrar Pago) generen su `idempotencyKey` fuera de la función de mutación para persistir en reintentos.

---

## ✅ Entregables Finales
1.  **Código Blindado:** Todos los handlers con soporte de concurrencia.
2.  **Arranque Seguro:** API que informa errores de configuración sin morir silenciosamente.
3.  **Frontend Atómico:** UI que maneja colisiones de edición de forma elegante.

---
*Plan de Refuerzo Técnico - GalponERP v1.1*
