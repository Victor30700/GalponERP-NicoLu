using System.ComponentModel;
using GalponERP.Application.Productos.Commands.ActualizarProducto;
using GalponERP.Application.Productos.Queries;
using GalponERP.Application.Productos.Queries.ListarProductos;
using GalponERP.Application.Productos.Queries.ObtenerProductoPorId;
using MediatR;
using Microsoft.SemanticKernel;
using System.Text;

namespace GalponERP.Application.Agentes.Plugins;

public class ConfiguracionPlugin
{
    private readonly IMediator _mediator;

    public ConfiguracionPlugin(IMediator mediator)
    {
        _mediator = mediator;
    }

    [KernelFunction]
    [Description("Ajusta el umbral mÃ­nimo de stock para un producto. Esto define cuÃ¡ndo el sistema debe generar alertas de reabastecimiento.")]
    public async Task<string> AjustarUmbralStock(
        [Description("Nombre del producto (ej. 'MaÃ­z')")] string nombreProducto,
        [Description("Nuevo umbral mÃ­nimo (cantidad)")] decimal nuevoUmbral)
    {
        // 1. Resolver Producto
        var productos = (await _mediator.Send(new ListarProductosQuery())).Where(p => p.IsActive).ToList();
        var productoRes = productos.FirstOrDefault(p => p.Nombre.Contains(nombreProducto, StringComparison.OrdinalIgnoreCase));
        
        if (productoRes == null)
        {
            var lista = string.Join(", ", productos.Select(p => p.Nombre));
            return $"No encontrÃ© el producto '{nombreProducto}'. Disponibles: [{lista}].";
        }

        // 2. Obtener detalle completo para la actualizaciÃ³n
        // Nota: ActualizarProductoCommand suele requerir todos los campos.
        // Como no tenemos un comando 'PatchUmbral', usamos el comando de actualizaciÃ³n completo.
        try
        {
            // Asumimos que podemos obtener los datos actuales para no sobreescribirlos con vacÃ­os
            // Si ObtenerProductoPorIdQuery no existe, tendrÃ­amos que buscar otra forma, 
            // pero el standard es tenerlo.
            var pFull = await _mediator.Send(new ObtenerProductoPorIdQuery(productoRes.Id));
            if (pFull == null) return "No se pudo recuperar el detalle completo del producto.";

            var command = new ActualizarProductoCommand(
                pFull.Id,
                pFull.Nombre,
                pFull.CategoriaId,
                pFull.UnidadMedidaId,
                pFull.PesoUnitarioKg,
                nuevoUmbral);

            await _mediator.Send(command);
            return $"El umbral mÃ­nimo para '{pFull.Nombre}' ha sido actualizado a {nuevoUmbral} {pFull.UnidadMedidaNombre}.";
        }
        catch (Exception ex)
        {
            return $"Error al actualizar el umbral: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Muestra los umbrales de stock mÃ­nimo configurados para todos los productos.")]
    public async Task<string> ConsultarUmbralesConfigurados()
    {
        var productos = await _mediator.Send(new ListarProductosQuery());
        if (!productos.Any()) return "No hay productos registrados.";

        var sb = new StringBuilder();
        sb.AppendLine("ConfiguraciÃ³n de Umbrales de Stock MÃ­nimo:");
        foreach (var p in productos.Where(p => p.IsActive))
        {
            sb.AppendLine($"- {p.Nombre}: {p.UmbralMinimo} {p.UnidadMedidaNombre}");
        }

        return sb.ToString();
    }
}
