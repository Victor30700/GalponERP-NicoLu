using GalponERP.Application.Interfaces;
using GalponERP.Application.Reporting.Dtos;
using GalponERP.Domain.Interfaces.Repositories;
using MediatR;

namespace GalponERP.Application.Inventario.Queries.ObtenerInventarioPdf;

public class ObtenerInventarioPdfQueryHandler : IRequestHandler<ObtenerInventarioPdfQuery, byte[]>
{
    private readonly IProductoRepository _productoRepository;
    private readonly ICategoriaProductoRepository _categoriaRepository;
    private readonly IConfiguracionRepository _configRepository;
    private readonly IPdfService _pdfService;

    public ObtenerInventarioPdfQueryHandler(
        IProductoRepository productoRepository,
        ICategoriaProductoRepository categoriaRepository,
        IConfiguracionRepository configRepository,
        IPdfService pdfService)
    {
        _productoRepository = productoRepository;
        _categoriaRepository = categoriaRepository;
        _configRepository = configRepository;
        _pdfService = pdfService;
    }

    public async Task<byte[]> Handle(ObtenerInventarioPdfQuery request, CancellationToken cancellationToken)
    {
        var config = await _configRepository.ObtenerAsync();
        var productos = await _productoRepository.ObtenerTodosAsync();
        
        string categoriaNombre = "Todas";
        if (request.CategoriaId.HasValue)
        {
            var cat = await _categoriaRepository.ObtenerPorIdAsync(request.CategoriaId.Value);
            categoriaNombre = cat?.Nombre ?? "N/A";
            productos = productos.Where(p => p.CategoriaProductoId == request.CategoriaId.Value);
        }

        var dto = new InventarioReportDto
        {
            NombreEmpresa = config?.NombreEmpresa ?? "Pollos NicoLu",
            Nit = config?.Nit ?? "0000000-0",
            Direccion = config?.Direccion,
            Telefono = config?.Telefono,
            TituloReporte = "Reporte de Inventario de Insumos (SAVCO-08)",
            Categoria = categoriaNombre,
            Productos = productos.Select(p => new ProductoStockDto
            {
                Nombre = p.Nombre,
                StockActual = p.StockActual,
                Unidad = p.Unidad?.Abreviatura ?? "Und",
                PrecioUnitario = p.CostoUnitarioActual,
                FechaVencimiento = null // Si el dominio lo soporta en el futuro
            }).OrderBy(x => x.Nombre).ToList()
        };

        dto.ValorTotalInventario = dto.Productos.Sum(x => x.Subtotal);

        return _pdfService.GenerarReporteInventarioPdf(dto);
    }
}
