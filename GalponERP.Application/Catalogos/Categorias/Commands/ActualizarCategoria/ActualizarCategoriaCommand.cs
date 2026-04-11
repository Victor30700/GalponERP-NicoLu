using FluentValidation;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Catalogos.Categorias.Commands.ActualizarCategoria;

public record ActualizarCategoriaCommand(
    Guid Id,
    string Nombre,
    string? Descripcion) : IRequest;

public class ActualizarCategoriaCommandValidator : AbstractValidator<ActualizarCategoriaCommand>
{
    public ActualizarCategoriaCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El ID es obligatorio.");
        
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre de la categoría es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");
            
        RuleFor(x => x.Descripcion)
            .MaximumLength(500).WithMessage("La descripción no puede exceder los 500 caracteres.");
    }
}

public class ActualizarCategoriaCommandHandler : IRequestHandler<ActualizarCategoriaCommand>
{
    private readonly ICategoriaProductoRepository _categoriaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarCategoriaCommandHandler(ICategoriaProductoRepository categoriaRepository, IUnitOfWork unitOfWork)
    {
        _categoriaRepository = categoriaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ActualizarCategoriaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await _categoriaRepository.ObtenerPorIdAsync(request.Id);
        if (categoria == null)
            throw new Exception("Categoría no encontrada");

        categoria.Actualizar(request.Nombre, request.Descripcion);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
