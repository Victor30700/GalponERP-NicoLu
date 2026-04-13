# Documentación de Arquitectura y Estado del Proyecto - GalponERP

## Resumen del Proyecto
GalponERP es un sistema de gestión para la producción avícola, diseñado con una arquitectura SaaS multitenant. Permite el control total del ciclo de vida de los pollos, desde el ingreso del lote hasta la venta final, integrando módulos de inventario, finanzas, sanidad y análisis inteligente mediante IA.

## Tecnologías Principales
- **Backend**: .NET 10, ASP.NET Core API.
- **Frontend**: Next.js 14, Tailwind CSS, TypeScript.
- **Base de Datos**: PostgreSQL (Persistencia relacional), Firebase Firestore (Metadatos de usuario).
- **IA**: Semantic Kernel, Ollama (Gemma), OpenAI Whisper/TTS.
- **Seguridad**: Firebase Auth (JWT).

## Arquitectura
El sistema sigue los principios de **Clean Architecture** y **Domain-Driven Design (DDD)**:
1. **Domain Layer**: Entidades de negocio, Value Objects (`Moneda`), interfaces de repositorio y lógica pura.
2. **Application Layer**: Casos de uso (Commands/Queries con MediatR), Validaciones (FluentValidation) y Orquestación de IA.
3. **Infrastructure Layer**: Implementación de repositorios (EF Core), servicios externos (Firebase, OpenAI, WhatsApp) y persistencia.
4. **API Layer**: Controladores REST, Middleware de excepciones y configuración de Swagger.

## Estado Actual: Maestría Operativa
El proyecto ha completado todas las fases de desarrollo planificadas, alcanzando un estado de "Maestría Operativa":
- **Backend Blindado**: Consistencia total de datos, auditoría completa y precisión matemática.
- **IA Omnicanal**: Operador inteligente capaz de ejecutar flujos complejos vía Web, WhatsApp y Voz.
- **Control Financiero**: Ciclo de caja 100% cubierto (Compras/Pagos, Ventas/Cobros, Costeo PPP).
- **UX Inteligente**: Resolución de entidades por búsqueda difusa y proactividad basada en snapshots de datos.

## Documentación Relevante
- `docs/contextProject.md`: Historial completo de Sprints y decisiones de arquitectura.
- `docs/endpoints.md`: Contrato completo de la API REST.
- `agent/instrucciones.md`: Instrucciones técnicas para el mantenimiento del sistema.
- `agent/plan.md`: Hoja de ruta (Fase 4 completada).
