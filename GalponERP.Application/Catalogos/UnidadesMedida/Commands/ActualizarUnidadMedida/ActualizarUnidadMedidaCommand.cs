using FluentValidation;
using GalponERP.Application.Interfaces;
using GalponERP.Application.Exceptions;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Catalogos.UnidadesMedida.Commands.ActualizarUnidadMedida;

public record ActualizarUnidadMedidaCommand(
    Guid Id,
    string Nombre,
    string Abreviatura,
    TipoUnidad Tipo,
    string? Version = null) : IRequest, IAuditableCommand;

public class ActualizarUnidadMedidaCommandValidator : AbstractValidator<ActualizarUnidadMedidaCommand>
{
    public ActualizarUnidadMedidaCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El ID es obligatorio.");
        
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.");
            
        RuleFor(x => x.Abreviatura)
            .NotEmpty().WithMessage("La abreviatura es obligatoria.")
            .MaximumLength(10).WithMessage("La abreviatura no puede exceder los 10 caracteres.");

        RuleFor(x => x.Tipo)
            .IsInEnum().WithMessage("El tipo de unidad no es válido.");

        RuleFor(x => x.Version).NotEmpty().WithMessage("La versión de concurrencia es obligatoria.");
    }
}

public class ActualizarUnidadMedidaCommandHandler : IRequestHandler<ActualizarUnidadMedidaCommand>
{
    private readonly IUnidadMedidaRepository _unidadMedidaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarUnidadMedidaCommandHandler(IUnidadMedidaRepository unidadMedidaRepository, IUnitOfWork unitOfWork)
    {
        _unidadMedidaRepository = unidadMedidaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ActualizarUnidadMedidaCommand request, CancellationToken cancellationToken)
    {
        var unidad = await _unidadMedidaRepository.ObtenerPorIdAsync(request.Id);
        if (unidad == null)
            throw new Exception("Unidad de medida no encontrada");

        if (!string.IsNullOrEmpty(request.Version) && unidad.Version.ToString() != request.Version)
        {
            throw new ConcurrencyException();
        }

        unidad.Actualizar(request.Nombre, request.Abreviatura, request.Tipo);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
