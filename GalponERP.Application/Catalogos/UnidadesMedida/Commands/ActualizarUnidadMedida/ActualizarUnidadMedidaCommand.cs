using FluentValidation;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Catalogos.UnidadesMedida.Commands.ActualizarUnidadMedida;

public record ActualizarUnidadMedidaCommand(
    Guid Id,
    string Nombre,
    string Abreviatura) : IRequest;

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

        unidad.Actualizar(request.Nombre, request.Abreviatura);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
