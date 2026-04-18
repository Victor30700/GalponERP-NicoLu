# Documentación de Cambios - Refactorización de Inventario y FCR

## Resumen
Se ha realizado una refactorización integral del manejo de unidades de medida y productos para permitir mayor flexibilidad en categorías no alimenticias y mejorar la precisión del cálculo de FCR (Food Conversion Ratio) mediante la sincronización bidireccional de Unidades y Kilogramos en el registro de consumo.

## Cambios en el Backend (.NET)

### Domain Layer
- **Nueva Entidad `TipoUnidad`**: Enum que clasifica las unidades en `Masa`, `Volumen` y `UnidadFisica`.
- **Entidad `UnidadMedida`**: Se agregó la propiedad `Tipo` para categorizar la unidad.
- **Entidad `Producto`**: 
    - Se flexibilizó el constructor para permitir `PesoUnitarioKg = 0`.
    - Ahora es posible crear productos que no requieren peso (ej. Medicinas, Servicios).

### Application Layer
- **Validadores de Producto**:
    - `CrearProductoCommandValidator` y `ActualizarProductoCommandValidator` ahora implementan validación condicional.
    - Solo se exige `PesoUnitarioKg > 0` si la categoría del producto es "Alimento".
- **Handler de Consumo Diario**:
    - `RegistrarConsumoAlimentoCommandHandler` ahora interpreta la cantidad recibida como **Kilogramos** (antes eran unidades).
    - Realiza la conversión interna a unidades para mantener la consistencia del stock, pero preservando la precisión decimal del pesaje ingresado.

### Infrastructure Layer
- **Migración de Base de Datos**: Se generó y aplicó la migración `AddTipoUnidadToUnidadMedida`.
- **Seeder**: Se actualizaron los datos iniciales de unidades de medida para incluir su tipo.

## Cambios en el Frontend (React)

### Registro de Productos (`ProductosPage.tsx`)
- Se implementó renderizado condicional en el formulario.
- Si el producto es "Alimento", se muestran los campos de Peso por Unidad y Peso Total Inicial.
- Si no es "Alimento", estos campos se ocultan y el sistema envía automáticamente `0` como peso.

### Registro de Consumo (`QuickRecordModal.tsx`)
- Se implementó **Sincronización Bidireccional**:
    - Al ingresar **Unidades** (Sacos), se calculan automáticamente los **Kg**.
    - Al ingresar **Kg** directamente, se calculan las **Unidades** equivalentes.
- Se eliminó el bloqueo del campo Kg, permitiendo ajustes manuales precisos.
- El payload enviado al backend ahora utiliza el valor en Kg como base principal.

## Fase 3 y 4: Integración Total y Pulido (Completado)

### Resumen de Integración
Se ha alcanzado la simetría total entre el backend (.NET) y el frontend (React/Next.js) mediante la centralización de la lógica en hooks de React Query, eliminando todas las llamadas directas a la API en los componentes y asegurando un flujo de datos consistente.

### Cambios en el Frontend (React)

#### Hooks Centralizados (`frontend/src/hooks`)
- **`useAgentes.ts`**: Implementado para gestionar conversaciones con la IA, incluyendo soporte para mensajes de voz (STT/TTS) y gestión de historial.
- **`useAuditoria.ts`**: Refactorizado para alinearse con la entidad del backend, permitiendo la visualización detallada de logs y la restauración de entidades eliminadas.
- **`useLotes.ts`**: Se integraron nuevas funciones para el cierre de lotes (`cerrarLote`), cancelación, reapertura y descarga de reportes PDF.
- **`useFinanzas.ts`**: Se agregó soporte para la consulta de **Cuentas por Pagar**, completando la visibilidad financiera del sistema.

#### Nuevas Funcionalidades y Componentes
- **Chat IA Refactorizado (`chat/page.tsx`)**: Ahora utiliza exclusivamente hooks para el envío de mensajes y carga de historial, mejorando la reactividad y el manejo de errores.
- **Panel de Auditoría (`auditoria/page.tsx`)**: Refactorizado para usar el hook centralizado, con visualización mejorada de detalles técnicos en formato JSON.
- **Liquidación de Lotes (`CerrarLoteModal.tsx`)**: 
    - Nuevo componente modal que captura datos críticos de liquidación (precio de venta promedio, fecha de cierre y observaciones).
    - Integrado en el dashboard de lotes para formalizar el fin del ciclo productivo.
- **Reportes PDF**: Integración de la descarga de informes de cierre directamente desde la interfaz de gestión de lotes.

#### Validación y Feedback Visual
- **React Query**: Se auditó la invalidación de queries en todas las mutaciones. El uso de `queryClient.invalidateQueries` asegura que al realizar cambios (ej. eliminar un lote, registrar un pago), todas las vistas relacionadas se actualicen instantáneamente sin recargar la página.
- **Feedback de Usuario**: Se estandarizó el uso de `sonner` para notificaciones tipo toast y `SweetAlert2` para diálogos de confirmación, asegurando que cada acción (éxito o error) sea comunicada claramente al usuario.
- **Navegación**: Se implementaron redirecciones automáticas tras acciones destructivas (ej. volver al listado tras eliminar un lote).

### Cambios en el Backend (.NET)
- Se verificó la consistencia de los DTOs de Auditoría y Finanzas para asegurar la compatibilidad con los nuevos campos requeridos por el frontend.

## Verificación Final
- **Simetría**: El 100% de los controladores del backend ahora tienen un hook correspondiente en el frontend.
- **UX**: Flujo de navegación fluido con manejo de estados de carga (`isLoading`) y errores en todas las vistas críticas.
- **Persistencia**: Se confirmó que las acciones de restauración en el panel de auditoría funcionan correctamente reintegrando los datos eliminados.
