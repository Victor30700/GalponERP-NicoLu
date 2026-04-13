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
        var requestName = typeof(TRequest).Name;

        // Solo auditamos comandos de actualización o eliminación
        if (requestName.Contains("Actualizar") || requestName.Contains("Eliminar") || requestName.Contains("Reabrir"))
        {
            var response = await next();

            var usuarioId = _currentUserContext.UsuarioId ?? Guid.Empty;
            var accion = requestName.Contains("Actualizar") ? "Actualizar" : 
                         requestName.Contains("Eliminar") ? "Eliminar" : "Reabrir";
            
            // Intentar extraer el ID de la entidad del request usando reflexión
            var entidadId = Guid.Empty;
            var idProp = typeof(TRequest).GetProperty("Id") ?? typeof(TRequest).GetProperty("LoteId");
            if (idProp != null)
            {
                var val = idProp.GetValue(request);
                if (val is Guid guid) entidadId = guid;
            }

            var entidad = requestName
                .Replace("Actualizar", "")
                .Replace("Eliminar", "")
                .Replace("Reabrir", "")
                .Replace("Command", "");

            var log = new AuditoriaLog(
                Guid.NewGuid(),
                usuarioId,
                "Sistema", // Placeholder ya que no tenemos Nombre en el contexto
                accion,
                entidad,
                entidad, // Usamos el nombre de la clase como nombre de la entidad
                entidadId,
                $"Ejecución de {requestName}",
                JsonSerializer.Serialize(request)
            );

            _auditoriaRepository.Agregar(log);
            // El IUnitOfWork.SaveChangesAsync() se llamará en el handler del comando, 
            // pero como esto es un pipeline, los cambios se guardarán juntos.
            
            return response;
        }

        return await next();
    }
}
