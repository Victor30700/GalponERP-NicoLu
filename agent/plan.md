# PROMPT MAESTRO - SPRINT 11: COMPLETAR OPERACIONES Y AUTH BACKEND

## Contexto
El usuario requiere exponer todas las entidades operativas del galpón que existen en el Dominio pero no tienen Casos de Uso ni Controladores. Además, necesitamos un endpoint de Login en el backend para facilitar las pruebas con Swagger interactuando con la REST API de Firebase.

## Tareas a Ejecutar (En orden estricto):

**1. Endpoint de Autenticación (Login Backend)**
- Crea `GalponERP.Application/Auth/Commands/Login/LoginCommand.cs` (Recibe Email y Password).
- Implementa el handler que utilice `HttpClient` para llamar a la REST API de Firebase (`https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={ApiKey}`) y devuelva el `idToken` y `refreshToken`. Obtén el ApiKey del `appsettings.json`.
- Crea `GalponERP.Api/Controllers/AuthController.cs` con el endpoint `[HttpPost("login")]` y el atributo `[AllowAnonymous]`.

**2. Operaciones Diarias: Mortalidad (Bajas)**
- **Application:** Crea los casos de uso en `Mortalidad/Commands` para `RegistrarMortalidadCommand` (LoteId, Cantidad, Causa, Fecha).
- **API:** Crea `MortalidadController.cs` con el endpoint `POST /api/mortalidad` protegido con `[Authorize]`.

**3. Operaciones Diarias: Gastos Operativos**
- **Application:** Crea los casos de uso en `Gastos/Commands` para `RegistrarGastoOperativoCommand` (Descripcion, Monto, Fecha, TipoGasto).
- **API:** Crea `GastosController.cs` con los endpoints `POST /api/gastos` y `GET /api/gastos` protegidos con `[Authorize]`.

**4. Operaciones Diarias: Calendario Sanitario (Vacunas)**
- **Application:** Crea los casos de uso para listar y actualizar vacunas (`MarcarVacunaAplicadaCommand`).
- **API:** Crea `CalendarioSanitarioController.cs` con `GET /api/calendario/{loteId}` y `PUT /api/calendario/{actividadId}/aplicar`.

**5. Revisión de Usuarios y Galpones (CRUD Completo)**
- Asegúrate de que `UsuariosController` tenga los métodos POST (Crear), PUT (Editar), GET (Listar) y DELETE (Desactivar).
- Asegúrate de que `GalponesController` tenga los métodos POST (Crear), PUT (Editar) y GET (Listar).

**6. Documentación Estricta (`docs/endpoints.md`)**
- **REGLA CRÍTICA:** Actualiza obligatoriamente el archivo `docs/endpoints.md`.
- Documenta el nuevo endpoint `/api/auth/login` indicando claramente que NO requiere Bearer Token y detallando el JSON de entrada (email/password) y salida (tokens).
- Documenta todos los nuevos endpoints operativos (Mortalidad, Gastos, Calendario, Usuarios, Galpones).
- Actualiza la bitácora en `agent/docs.md`.

## Restricciones
- Respeta estrictamente Clean Architecture y el patrón CQRS con MediatR.
- Todos los controladores (excepto Auth) deben tener `[Authorize]`.
- Verifica que el proyecto compile sin errores antes de finalizar.