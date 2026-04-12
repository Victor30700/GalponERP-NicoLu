using System.ComponentModel;
using GalponERP.Application.Catalogos.Categorias.Commands.CrearCategoria;
using GalponERP.Application.Catalogos.Categorias.Queries.ListarCategorias;
using GalponERP.Application.Catalogos.UnidadesMedida.Queries.ListarUnidadesMedida;
using GalponERP.Application.Productos.Commands.CrearProducto;
using GalponERP.Application.Clientes.Commands.CrearCliente;
using GalponERP.Application.Proveedores.Commands.CrearProveedor;
using MediatR;
using Microsoft.SemanticKernel;
using System.Text;

namespace GalponERP.Application.Agentes.Plugins;

public class GestionCatalogosPlugin
{
    private readonly IMediator _mediator;

    public GestionCatalogosPlugin(IMediator mediator)
    {
        _mediator = mediator;
    }

    [KernelFunction]
    [Description("Crea una nueva categoría de productos en el sistema.")]
    public async Task<string> CrearCategoria(
        [Description("Nombre de la categoría (ej. 'Alimento', 'Medicamento', 'Insumos')")] string nombre,
        [Description("Opcional: Descripción de la categoría")] string? descripcion = null)
    {
        try
        {
            var command = new CrearCategoriaCommand(nombre, descripcion);
            var result = await _mediator.Send(command);
            return $"Categoría '{nombre}' creada exitosamente con ID: {result}. Ya puedes asignar productos a esta categoría.";
        }
        catch (Exception ex)
        {
            return $"Error al crear la categoría: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Crea un nuevo producto. Resuelve automáticamente la categoría y unidad de medida o solicita aclaración.")]
    public async Task<string> CrearProducto(
        [Description("Nombre del producto (ej. 'Maíz Amarillo')")] string nombre,
        [Description("Equivalencia en kilogramos para 1 unidad (ej. 50 para un saco de 50kg)")] decimal equivalenciaKg,
        [Description("Opcional: Nombre de la categoría")] string? nombreCategoria = null,
        [Description("Opcional: Nombre de la unidad de medida (ej. 'Saco', 'Unidad')")] string? nombreUnidad = null,
        [Description("Opcional: Stock mínimo para alertas")] decimal umbralMinimo = 0)
    {
        // 1. Resolver Categoría
        var categorias = (await _mediator.Send(new ListarCategoriasQuery())).ToList();
        CategoriaResponse? catSeleccionada = null;

        if (!string.IsNullOrWhiteSpace(nombreCategoria))
        {
            catSeleccionada = categorias.FirstOrDefault(c => c.Nombre.Contains(nombreCategoria, StringComparison.OrdinalIgnoreCase));
        }

        if (catSeleccionada == null)
        {
            var listaCats = string.Join(", ", categorias.Select(c => c.Nombre));
            if (string.IsNullOrWhiteSpace(nombreCategoria))
            {
                return $"Para crear el producto '{nombre}', necesito que me indiques una categoría. Las disponibles son: [{listaCats}]. Si no existe la que buscas, pídeme que la cree primero.";
            }
            return $"No encontré la categoría '{nombreCategoria}'. Las registradas son: [{listaCats}]. ¿Deseas usar una de estas o que cree una nueva categoría '{nombreCategoria}'?";
        }

        // 2. Resolver Unidad de Medida
        var unidades = (await _mediator.Send(new ListarUnidadesMedidaQuery())).ToList();
        UnidadMedidaResponse? uniSeleccionada = null;

        if (!string.IsNullOrWhiteSpace(nombreUnidad))
        {
            uniSeleccionada = unidades.FirstOrDefault(u => u.Nombre.Contains(nombreUnidad, StringComparison.OrdinalIgnoreCase));
        }
        else if (unidades.Count == 1)
        {
            uniSeleccionada = unidades.First();
        }

        if (uniSeleccionada == null)
        {
            var listaUnis = string.Join(", ", unidades.Select(u => u.Nombre));
            if (string.IsNullOrWhiteSpace(nombreUnidad))
            {
                return $"Necesito saber la unidad de medida para '{nombre}'. Las opciones son: [{listaUnis}].";
            }
            return $"No encontré la unidad '{nombreUnidad}'. Las disponibles son: [{listaUnis}].";
        }

        // 3. Ejecutar Comando
        try
        {
            // Usando el namespace correcto según el archivo leído: GalponERP.Application.Productos.Commands.CrearProducto
            var command = new CrearProductoCommand(nombre, catSeleccionada.Id, uniSeleccionada.Id, equivalenciaKg, umbralMinimo);
            var result = await _mediator.Send(command);
            return $"Producto '{nombre}' creado exitosamente en la categoría '{catSeleccionada.Nombre}' con unidad '{uniSeleccionada.Nombre}'. ID: {result}";
        }
        catch (Exception ex)
        {
            return $"Error al crear el producto: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Registra un nuevo cliente en el sistema.")]
    public async Task<string> RegistrarCliente(
        [Description("Nombre completo o razón social")] string nombre,
        [Description("RUC o Identificación")] string ruc,
        [Description("Opcional: Dirección")] string? direccion = null,
        [Description("Opcional: Teléfono")] string? telefono = null)
    {
        try
        {
            var command = new CrearClienteCommand(nombre, ruc, direccion, telefono);
            var result = await _mediator.Send(command);
            return $"Cliente '{nombre}' registrado con éxito. ID: {result}";
        }
        catch (Exception ex)
        {
            return $"Error al registrar cliente: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Registra un nuevo proveedor en el sistema.")]
    public async Task<string> RegistrarProveedor(
        [Description("Razón Social del proveedor")] string razonSocial,
        [Description("NIT o RUC")] string nitRuc,
        [Description("Opcional: Teléfono")] string? telefono = null,
        [Description("Opcional: Email")] string? email = null,
        [Description("Opcional: Dirección")] string? direccion = null)
    {
        try
        {
            var command = new CrearProveedorCommand(razonSocial, nitRuc, telefono, email, direccion);
            var result = await _mediator.Send(command);
            return $"Proveedor '{razonSocial}' registrado con éxito. ID: {result}";
        }
        catch (Exception ex)
        {
            return $"Error al registrar proveedor: {ex.Message}";
        }
    }
}
