using FluentValidation;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Catalogos.UnidadesMedida.Commands.CrearUnidadMedida;

public record CrearUnidadMedidaCommand(
    string Nombre,
    string Abreviatura,
    TipoUnidad Tipo) : IRequest<Guid>, IAuditableCommand;

public class CrearUnidadMedidaCommandValidator : AbstractValidator<CrearUnidadMedidaCommand>
{
    public CrearUnidadMedidaCommandValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.");
            
        RuleFor(x => x.Abreviatura)
            .NotEmpty().WithMessage("La abreviatura es obligatoria.")
            .MaximumLength(10).WithMessage("La abreviatura no puede exceder los 10 caracteres.");

        RuleFor(x => x.Tipo)
            .IsInEnum().WithMessage("El tipo de unidad no es válido.");
    }
}

public class CrearUnidadMedidaCommandHandler : IRequestHandler<CrearUnidadMedidaCommand, Guid>
{
    private readonly IUnidadMedidaRepository _unidadMedidaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearUnidadMedidaCommandHandler(IUnidadMedidaRepository unidadMedidaRepository, IUnitOfWork unitOfWork)
    {
        _unidadMedidaRepository = unidadMedidaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CrearUnidadMedidaCommand request, CancellationToken cancellationToken)
    {
        var unidad = new UnidadMedida(
            Guid.NewGuid(),
            request.Nombre,
            request.Abreviatura,
            request.Tipo);

        _unidadMedidaRepository.Agregar(unidad);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return unidad.Id;
    }
}
