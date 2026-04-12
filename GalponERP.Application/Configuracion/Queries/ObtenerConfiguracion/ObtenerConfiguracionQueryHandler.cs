using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Configuracion.Queries.ObtenerConfiguracion;

public class ObtenerConfiguracionQueryHandler : IRequestHandler<ObtenerConfiguracionQuery, ConfiguracionResponse>
{
    private readonly IConfiguracionRepository _configuracionRepository;

    public ObtenerConfiguracionQueryHandler(IConfiguracionRepository configuracionRepository)
    {
        _configuracionRepository = configuracionRepository;
    }

    public async Task<ConfiguracionResponse> Handle(ObtenerConfiguracionQuery request, CancellationToken cancellationToken)
    {
        var config = await _configuracionRepository.ObtenerAsync();

        if (config == null)
        {
            // Retornar valores por defecto si no existe configuración
            return new ConfiguracionResponse(
                "Pollos NicoLu",
                "000000000-0",
                null,
                null,
                null,
                "USD",
                null);
        }

        return new ConfiguracionResponse(
            config.NombreEmpresa,
            config.Nit,
            config.Telefono,
            config.Email,
            config.Direccion,
            config.MonedaPorDefecto,
            config.LogoUrl);
    }
}
