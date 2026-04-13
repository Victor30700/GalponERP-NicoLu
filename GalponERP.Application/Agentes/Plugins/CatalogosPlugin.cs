using System.ComponentModel;
using GalponERP.Application.Productos.Queries;
using GalponERP.Application.Productos.Queries.ListarProductos;
using GalponERP.Application.Proveedores.Queries.ListarProveedores;
using GalponERP.Application.Clientes.Queries.ListarClientes;
using GalponERP.Application.Catalogos.Categorias.Queries.ListarCategorias;
using MediatR;
using Microsoft.SemanticKernel;
using System.Text;

namespace GalponERP.Application.Agentes.Plugins;

public class CatalogosPlugin
{
    private readonly IMediator _mediator;

    public CatalogosPlugin(IMediator mediator)
    {
        _mediator = mediator;
    }

    [KernelFunction]
    [Description("Obtiene un resumen de todos los catálogos disponibles (Productos, Proveedores, Clientes, Categorías) para saber qué registros existen en el sistema.")]
    public async Task<string> ListarOpcionesDisponibles()
    {
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("Catálogos disponibles en el sistema:");

            // 1. Productos
            var productos = await _mediator.Send(new ListarProductosQuery());
            sb.AppendLine("\n--- PRODUCTOS ---");
            if (!productos.Any()) sb.AppendLine("No hay productos registrados.");
            else sb.AppendLine(string.Join(", ", productos.Where(p => p.IsActive).Select(p => p.Nombre)));

            // 2. Proveedores
            var proveedores = await _mediator.Send(new ListarProveedoresQuery());
            sb.AppendLine("\n--- PROVEEDORES ---");
            if (!proveedores.Any()) sb.AppendLine("No hay proveedores registrados.");
            else sb.AppendLine(string.Join(", ", proveedores.Where(p => p.IsActive).Select(p => p.RazonSocial)));

            // 3. Clientes
            var clientes = await _mediator.Send(new ListarClientesQuery());
            sb.AppendLine("\n--- CLIENTES ---");
            if (!clientes.Any()) sb.AppendLine("No hay clientes registrados.");
            else sb.AppendLine(string.Join(", ", clientes.Where(c => c.IsActive).Select(c => c.Nombre)));

            // 4. Categorías de Productos
            var categorias = await _mediator.Send(new ListarCategoriasQuery());
            sb.AppendLine("\n--- CATEGORÍAS ---");
            if (!categorias.Any()) sb.AppendLine("No hay categorías registradas.");
            else sb.AppendLine(string.Join(", ", categorias.Select(c => c.Nombre)));

            return sb.ToString();
        }
        catch (Exception ex)
        {
            return $"Error al listar opciones de catálogos: {ex.Message}";
        }
    }
}
