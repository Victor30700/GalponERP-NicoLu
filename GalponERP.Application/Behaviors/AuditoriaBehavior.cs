using System.Text.Json;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Behaviors;

public class AuditoriaBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IAuditoriaRepository _auditoriaRepository;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IUnitOfWork _unitOfWork;

    public AuditoriaBehavior(
        IAuditoriaRepository auditoriaRepository, 
        ICurrentUserContext currentUserContext,
        IUnitOfWork unitOfWork)
    {
        _auditoriaRepository = auditoriaRepository;
        _currentUserContext = currentUserContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Solo auditamos comandos que implementen IAuditableCommand
        if (request is IAuditableCommand auditableRequest)
        {
            var requestName = typeof(TRequest).Name;
            var response = await next();

            var usuarioId = _currentUserContext.UsuarioId ?? Guid.Empty;
            
            // Determinar la acción basándose en el prefijo del comando (convención)
            var accion = "Ejecutar";
            if (requestName.StartsWith("Actualizar")) accion = "Actualizar";
            else if (requestName.StartsWith("Eliminar")) accion = "Eliminar";
            else if (requestName.StartsWith("Reabrir")) accion = "Reabrir";
            else if (requestName.StartsWith("Crear")) accion = "Crear";
            else if (requestName.StartsWith("Registrar")) accion = "Registrar";
            
            // Intentar extraer el ID de la entidad del request usando reflexión
            var entidadId = Guid.Empty;
            var idProp = typeof(TRequest).GetProperty("Id") ?? 
                         typeof(TRequest).GetProperty("LoteId") ??
                         typeof(TRequest).GetProperty("VentaId") ??
                         typeof(TRequest).GetProperty("ProductoId");

            if (idProp != null)
            {
                var val = idProp.GetValue(request);
                if (val is Guid guid) entidadId = guid;
            }

            var entidad = requestName
                .Replace("Actualizar", "")
                .Replace("Eliminar", "")
                .Replace("Reabrir", "")
                .Replace("Crear", "")
                .Replace("Registrar", "")
                .Replace("Command", "");

            var log = new AuditoriaLog(
                Guid.NewGuid(),
                usuarioId,
                "Sistema", 
                accion,
                entidad,
                entidad, 
                entidadId,
                $"Ejecución de {requestName} vía IAuditableCommand",
                JsonSerializer.Serialize(request)
            );

            _auditoriaRepository.Agregar(log);
            
            return response;
        }

        return await next();
    }
}
