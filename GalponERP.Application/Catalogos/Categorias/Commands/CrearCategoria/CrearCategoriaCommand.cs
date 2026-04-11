using FluentValidation;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Catalogos.Categorias.Commands.CrearCategoria;

public record CrearCategoriaCommand(
    string Nombre,
    string? Descripcion) : IRequest<Guid>;

public class CrearCategoriaCommandValidator : AbstractValidator<CrearCategoriaCommand>
{
    public CrearCategoriaCommandValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre de la categoría es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");
            
        RuleFor(x => x.Descripcion)
            .MaximumLength(500).WithMessage("La descripción no puede exceder los 500 caracteres.");
    }
}

public class CrearCategoriaCommandHandler : IRequestHandler<CrearCategoriaCommand, Guid>
{
    private readonly ICategoriaProductoRepository _categoriaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearCategoriaCommandHandler(ICategoriaProductoRepository categoriaRepository, IUnitOfWork unitOfWork)
    {
        _categoriaRepository = categoriaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CrearCategoriaCommand request, CancellationToken cancellationToken)
    {
        var categoria = new CategoriaProducto(
            Guid.NewGuid(),
            request.Nombre,
            request.Descripcion);

        _categoriaRepository.Agregar(categoria);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return categoria.Id;
    }
}
