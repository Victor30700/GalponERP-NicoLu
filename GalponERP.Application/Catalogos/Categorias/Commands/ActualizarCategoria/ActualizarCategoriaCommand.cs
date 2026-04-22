using FluentValidation;
using GalponERP.Application.Exceptions;
using GalponERP.Application.Interfaces;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

using GalponERP.Domain.Entities;

namespace GalponERP.Application.Catalogos.Categorias.Commands.ActualizarCategoria;

public record ActualizarCategoriaCommand(
    Guid Id,
    string Nombre,
    string? Descripcion,
    TipoCategoria Tipo,
    string? Version = null) : IRequest, IAuditableCommand;

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

        RuleFor(x => x.Version).NotEmpty().WithMessage("La versión de concurrencia es obligatoria.");
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

        if (!string.IsNullOrEmpty(request.Version) && categoria.Version.ToString() != request.Version)
        {
            throw new ConcurrencyException();
        }

        categoria.Actualizar(request.Nombre, request.Descripcion, request.Tipo);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
