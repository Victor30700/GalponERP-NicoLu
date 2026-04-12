using MediatR;

namespace GalponERP.Application.Configuracion.Commands.ActualizarConfiguracion;

public record ActualizarConfiguracionCommand(
    string NombreEmpresa,
    string Nit,
    string? Telefono,
    string? Email,
    string? Direccion,
    string MonedaPorDefecto,
    string? LogoUrl) : IRequest;
