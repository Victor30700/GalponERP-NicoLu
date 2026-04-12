using MediatR;

namespace GalponERP.Application.Configuracion.Queries.ObtenerConfiguracion;

public record ConfiguracionResponse(
    string NombreEmpresa,
    string Nit,
    string? Telefono,
    string? Email,
    string? Direccion,
    string MonedaPorDefecto,
    string? LogoUrl);

public record ObtenerConfiguracionQuery() : IRequest<ConfiguracionResponse>;
