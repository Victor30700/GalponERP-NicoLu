# 🛠️ INSTRUCCIONES DE IMPLEMENTACIÓN: REFUERZO Y RESILIENCIA

Actúa como un **Senior Backend & Frontend Engineer**. Tu objetivo es ejecutar el "Plan de Refuerzo de Casos de Borde" detallado en `plan.md` para blindar GalponERP contra fallos de concurrencia e infraestructura.

---

## 🏗️ REGLAS GENERALES
1.  **Surgical Updates:** Realiza cambios mínimos necesarios. No refactorices código que ya funciona y no está en el plan.
2.  **Concurrency First:** La integridad de los datos es la prioridad #1. Ninguna mutación `Update` debe quedar sin su chequeo de `xmin`.
3.  **No Crashing:** La API debe ser capaz de informar que faltan credenciales sin entrar en un bucle de reinicio infinito (Crash-loop).

---

## 🎯 FASE 1: BLINDAJE DE CONCURRENCIA (BACKEND)

### 1.1 Unificación de Comandos
*   Busca todos los archivos `Actualizar...Command.cs`.
*   Asegura que la propiedad de concurrencia se llame `string? Version`.
*   **Crítico:** En `ActualizarMortalidadCommand`, cambia el tipo de `uint` a `string?`.

### 1.2 Implementación en Handlers
En los Handlers de **Mortalidad, Usuario, GastoOperativo, Bienestar (Sanidad), Pesaje y Formula**:
*   Obtén la entidad del repositorio.
*   Compara `entidad.Version.ToString() != request.Version`.
*   Si no coincide, lanza `throw new ConcurrencyException();`.
*   Asegúrate de importar `GalponERP.Application.Exceptions`.

### 1.3 Validación Estricta
*   En cada `Actualizar...CommandValidator`, añade la regla para que `Version` no sea nulo ni vacío.

---

## ⚙️ FASE 2: RESILIENCIA Y SEGURIDAD (CORE)

### 2.1 Robustez de Firebase
*   En `FirebaseAuthService.cs`, envuelve la lógica de inicialización en bloques que capturen excepciones.
*   Si `firebase-admin.json` no existe, registra un `_logger.LogCritical` indicando exactamente qué falta, pero permite que el servicio se instancie (aunque falle en tiempo de ejecución al usarse) para evitar que el contenedor de DI falle al arrancar la app.

### 2.2 Enmascaramiento de API Key
*   Revisa `AuditoriaBehavior.cs` (MediatR pipeline). Si registra los headers o el cuerpo del request, asegúrate de que el valor de `x-api-key` sea reemplazado por `***MASKED***`.
*   Haz lo mismo en `GlobalExceptionHandler.cs` si imprime detalles del request en logs.

---

## 💻 FASE 3: RESILIENCIA FRONTEND (NEXT.JS)

### 3.1 Snapshot Integrity (React Query)
En `useInventario.ts`, `useVentas.ts` y `useSanidad.ts`:
*   Dentro de `onMutate`, llama a `await queryClient.cancelQueries({ queryKey: [...] })` para todas las llaves relacionadas antes de modificar el caché.
*   Asegúrate de que el `onError` haga el rollback al `context.previousData`.

### 3.2 Idempotencia en UI
*   Revisa los componentes que disparan registros (Ventas, Compras, Pagos).
*   La `idempotencyKey = crypto.randomUUID()` debe generarse en el `handleSubmit` o función del componente, NO dentro del `mutationFn` del hook, para que si TanStack Query reintenta la petición por red, use la MISMA llave.

---

## ✅ VALIDACIÓN FINAL
1.  **Build:** `dotnet build GalponERP.sln` debe pasar sin errores.
2.  **Audit Logs:** Verifica que no aparezcan API Keys en los logs de consola.
3.  **Concurrency:** Simula un cambio de versión manual en la base de datos y verifica que el frontend muestre el `toast` de "Conflicto de edición".

---
*Instrucciones para el Agente de Desarrollo - GalponERP v1.1*
