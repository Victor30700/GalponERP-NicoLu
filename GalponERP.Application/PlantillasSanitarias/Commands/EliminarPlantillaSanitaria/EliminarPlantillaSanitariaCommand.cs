using MediatR;

namespace GalponERP.Application.PlantillasSanitarias.Commands.EliminarPlantillaSanitaria;

public record EliminarPlantillaSanitariaCommand(Guid Id) : IRequest;
