# Documentación de Integración y Seguridad - Galpon ERP

## Módulo de Autenticación y Usuarios (Actualización Firestore)

### 1. Sincronización con Firebase Firestore
Se ha implementado la persistencia de usuarios en las colecciones de Firestore para asegurar que los datos del perfil estén disponibles tanto en Firebase Auth como en la base de datos de documentos.

- **Paquete Instalado:** `Google.Cloud.Firestore` en la capa de Infraestructura.
- **Colección:** `users`
- **Identificador:** Se utiliza el `Uid` de Firebase Auth como ID del documento en Firestore para mantener una relación 1:1.
- **Campos Sincronizados:**
  - `uid`: Identificador único de Firebase.
  - `email`: Correo electrónico del usuario.
  - `displayName`: Nombre completo (Nombre + Apellidos).
  - `nombre`: Nombre de pila.
  - `apellidos`: Apellidos del usuario.
  - `rol`: Rol asignado (Admin, Veterinario, Operador).
  - `direccion`: Dirección física.
  - `profesion`: Profesión o cargo.
  - `fechaNacimiento`: Fecha de nacimiento formateada (YYYY-MM-DD).
  - `createdAt`: Timestamp de creación en Firestore.

### 2. Cambios en la Capa de Aplicación
- **IAuthenticationService:** Se actualizó la interfaz para permitir el envío de `IDictionary<string, object> extraUserData` durante la creación del usuario.
- **RegistrarUsuarioCommandHandler:** Se modificó la lógica para capturar todos los campos del comando y enviarlos a Firestore a través del servicio de autenticación antes de persistir en la base de datos local (PostgreSQL).

### 3. Cambios en la Capa de Infraestructura
- **FirebaseAuthService:**
  - Inicialización de `FirestoreDb` utilizando el `ProjectId` configurado.
  - Implementación de `SetAsync` en la colección `users` al ejecutar `CreateUserAsync`.
  - Manejo de excepciones para recuperación de UID si el email ya existe en Firebase.

## Flujo de Registro de Usuario
1. **Validación Local:** Se verifica si el email ya existe en PostgreSQL.
2. **Firebase Auth:** Se crea el registro de autenticación (credenciales).
3. **Firebase Firestore:** Se crea el documento en la colección `users` con los metadatos del perfil.
4. **Persistencia Local:** Se guarda el usuario en PostgreSQL para relaciones relacionales (Lotes, Galpones, etc.).

---
*Documentación actualizada al: 10 de Abril, 2026*
