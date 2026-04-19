using GalponERP.Application.Interfaces;
using GalponERP.Application.Reporting.Dtos;
using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Lotes.Queries.ObtenerConsumoAlimentoPdf;

public class ObtenerConsumoAlimentoPdfQueryHandler : IRequestHandler<ObtenerConsumoAlimentoPdfQuery, byte[]>
{
    private readonly ILoteRepository _loteRepository;
    private readonly IInventarioRepository _inventarioRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IPesajeLoteRepository _pesajeRepository;
    private readonly IConfiguracionRepository _configRepository;
    private readonly IPdfService _pdfService;

    public ObtenerConsumoAlimentoPdfQueryHandler(
        ILoteRepository loteRepository,
        IInventarioRepository inventarioRepository,
        IProductoRepository productoRepository,
        IPesajeLoteRepository pesajeRepository,
        IConfiguracionRepository configRepository,
        IPdfService pdfService)
    {
        _loteRepository = loteRepository;
        _inventarioRepository = inventarioRepository;
        _productoRepository = productoRepository;
        _pesajeRepository = pesajeRepository;
        _configRepository = configRepository;
        _pdfService = pdfService;
    }

    public async Task<byte[]> Handle(ObtenerConsumoAlimentoPdfQuery request, CancellationToken cancellationToken)
    {
        var lote = await _loteRepository.ObtenerPorIdAsync(request.LoteId);
        if (lote == null) throw new Exception("Lote no encontrado.");

        var config = await _configRepository.ObtenerAsync();
        var movimientos = await _inventarioRepository.ObtenerPorLoteIdAsync(request.LoteId);
        var productos = await _productoRepository.ObtenerTodosAsync();
        
        var consumos = new List<ConsumoDetalleDto>();
        decimal totalKg = 0;

        foreach (var m in movimientos.Where(x => x.Tipo == TipoMovimiento.Salida).OrderBy(x => x.Fecha))
        {
            var producto = productos.FirstOrDefault(p => p.Id == m.ProductoId);
            decimal kgMovimiento = m.Cantidad * m.PesoUnitarioHistorico;
            totalKg += kgMovimiento;

            consumos.Add(new ConsumoDetalleDto
            {
                Fecha = m.Fecha,
                Producto = producto?.Nombre ?? "Producto Desconocido",
                CantidadKg = kgMovimiento
            });
        }

        // Calcular FCR Proyectado
        var pesajes = await _pesajeRepository.ObtenerPorLoteIdAsync(request.LoteId);
        decimal fcr = 0;
        if (pesajes.Any())
        {
            var ultimoPesoGramos = pesajes.OrderByDescending(p => p.Fecha).First().PesoPromedioGramos;
            decimal biomasaKg = (lote.CantidadActual * ultimoPesoGramos) / 1000m;
            if (biomasaKg > 0)
            {
                fcr = totalKg / biomasaKg;
            }
        }

        var dto = new ConsumoAlimentoReportDto
        {
            NombreEmpresa = config?.NombreEmpresa ?? "Pollos NicoLu",
            Nit = config?.Nit ?? "0000000-0",
            Direccion = config?.Direccion,
            Telefono = config?.Telefono,
            TituloReporte = "Control de Consumo de Alimento (SAVCO-04)",
            NombreLote = lote.Nombre,
            NombreGalpon = lote.Galpon?.Nombre ?? "N/A",
            FechaIngresoLote = lote.FechaIngreso,
            Consumos = consumos,
            TotalKgConsumidos = totalKg,
            FCRProyectado = fcr
        };

        return _pdfService.GenerarConsumoAlimentoPdf(dto);
    }
}
