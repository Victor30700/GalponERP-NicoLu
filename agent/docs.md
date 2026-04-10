# BITÁCORA DE ARQUITECTURA - GALPON ERP

## SPRINT 11: Operaciones Diarias y Auth Backend
Se han expuesto las entidades operativas del dominio y se ha facilitado la integración con un endpoint de Login en el backend.

### Cambios Realizados:
- **Autenticación (Login Backend):**
  - Se implementó `LoginCommand` y `LoginCommandHandler` en la capa de Aplicación.
  - El handler utiliza `HttpClient` para comunicarse directamente con la REST API de Firebase Auth, permitiendo obtener el `idToken` y `refreshToken` desde el backend.
  - Se configuró la `ApiKey` de Firebase en `appsettings.json`.
  - Se creó `AuthController` con el endpoint `POST /api/auth/login` (`[AllowAnonymous]`).
- **Mortalidad (Bajas):**
  - Se actualizó la entidad `MortalidadDiaria` para incluir el campo `Causa`.
  - Se implementó `RegistrarMortalidadCommand` que actualiza los contadores de bajas en la entidad `Lote` y persiste el registro diario.
  - Se creó `MortalidadController` protegido por `[Authorize]`.
- **Gastos Operativos:**
  - Se actualizó la entidad `GastoOperativo` para incluir el campo `TipoGasto`.
  - Se implementaron los casos de uso `RegistrarGastoOperativoCommand` y `ObtenerGastosQuery` (con filtros por GalponId y LoteId).
  - Se creó `GastosController` con endpoints `POST` y `GET`.
- **Calendario Sanitario (Vacunas):**
  - Se implementaron los casos de uso `MarcarVacunaAplicadaCommand` y `ObtenerCalendarioPorLoteQuery`.
  - Se creó `CalendarioSanitarioController` con endpoints `GET /api/calendario/{loteId}` y `PUT /api/calendario/{actividadId}/aplicar`.
- **Infraestructura:**
  - Se crearon e integraron `IMortalidadRepository` y su implementación.
  - Se actualizó `IGastoOperativoRepository` para incluir `ObtenerTodosAsync`.
  - Se registró `HttpClient` en el contenedor de dependencias de la capa de Aplicación.

### Decisiones de Diseño:
- **Login Proxy:** Se decidió implementar el login en el backend para facilitar el uso de Swagger y herramientas de prueba, manteniendo Firebase como el proveedor de identidad centralizado.
- **Validación en el Dominio:** El registro de mortalidad invoca métodos de la entidad `Lote` (`RegistrarBajas`), asegurando que las reglas de negocio (ej. no registrar más bajas que pollos vivos) se validen en el corazón del dominio.
- **Flexibilidad en Gastos:** Los gastos operativos pueden asociarse a un galpón de forma general o a un lote específico, permitiendo un análisis de costos más granular.

## SPRINT 9.3: Seeding Automático y Gestión Total
Se ha automatizado la creación del administrador inicial y se han completado los CRUDs de usuarios y galpones.

### Cambios Realizados:
- **Seeding Automático (Program.cs):**
  - Se implementó un bloque de inicialización que verifica la existencia del usuario "Admin Maestro" con el Firebase UID `utq0GMrQZESnNsyQWUEFOV5fKf23`.
  - Si no existe, se crea automáticamente en la base de datos local para asegurar que el primer acceso tenga permisos de administración.
- **Gestión de Usuarios:**
  - Se añadieron `ActualizarUsuarioCommand` (edición de nombre y rol) y `EliminarUsuarioCommand` (Soft Delete mediante la propiedad `IsActive` de la entidad base).
  - Se expusieron los endpoints `PUT /api/usuarios/{id}` y `DELETE /api/usuarios/{id}`.
- **Gestión de Galpones:**
  - Se creó la entidad `Galpon` con soporte para CRUD completo.
  - Se implementaron los casos de uso: `CrearGalponCommand`, `ListarGalponesQuery` y `EditarGalponCommand`.
  - Se creó el controlador `GalponesController` con todos los métodos protegidos por `[Authorize]`.
- **Arquitectura:**
  - Se integró `IGalponRepository` siguiendo el patrón de Repositorio y Unidad de Trabajo (Unit of Work).
  - Se actualizaron las entidades para incluir métodos de actualización de estado siguiendo los principios de DDD.

## SPRINT 10: Inicialización del Frontend y Autenticación
Se ha inicializado el proyecto Frontend usando Next.js 14 y se ha implementado la base de la autenticación con Firebase.

### Cambios Realizados:
- **Proyecto Frontend:**
  - Se creó el proyecto `frontend` usando `create-next-app` con TypeScript, Tailwind CSS y App Router.
  - Se configuró el `src` directory para una mejor organización.
- **Firebase Client:**
  - Se instaló la librería `firebase`.
  - Se creó `frontend/src/config/firebase.ts` para inicializar el SDK del cliente utilizando variables de entorno.
- **Autenticación:**
  - Se implementó `frontend/src/context/AuthContext.tsx` utilizando React Context para gestionar el estado del usuario de forma global.
  - El contexto escucha cambios en el estado de autenticación mediante `onAuthStateChanged` y expone el JWT Token (`getIdToken()`) para futuras integraciones con el backend.
- **Interfaz de Usuario (UI):**
  - Se creó la página de login en `frontend/src/app/login/page.tsx` con un diseño limpio y responsivo usando Tailwind CSS.
  - Se actualizó la página principal (`frontend/src/app/page.tsx`) para actuar como un dashboard protegido que redirige al login si no hay un usuario autenticado.
- **Configuración Global:**
  - Se envolvió la aplicación con el `AuthProvider` en `frontend/src/app/layout.tsx`.

### Decisiones de Diseño:
- **Next.js App Router:** Se eligió por sus capacidades modernas de renderizado y facilidad de enrutamiento.
- **React Context para Auth:** Permite un acceso sencillo al estado del usuario y al token en cualquier componente del frontend sin necesidad de librerías externas de gestión de estado complejas.
- **Seguridad en Cliente:** Aunque la validación final ocurre en el backend, el frontend gestiona la persistencia de la sesión y la obtención de tokens de forma segura a través de Firebase SDK.

---

## SPRINT 9: Exposición de API y Seguridad
Se ha habilitado la capa de presentación (API) para interactuar con los casos de uso definidos en la capa de Aplicación.
...
