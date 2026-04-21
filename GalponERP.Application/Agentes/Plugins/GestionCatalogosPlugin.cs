using System.ComponentModel;
using GalponERP.Application.Catalogos.Categorias.Commands.CrearCategoria;
using GalponERP.Application.Catalogos.Categorias.Queries.ListarCategorias;
using GalponERP.Application.Catalogos.UnidadesMedida.Queries.ListarUnidadesMedida;
using GalponERP.Application.Productos.Commands.CrearProducto;
using GalponERP.Application.Clientes.Commands.CrearCliente;
using GalponERP.Application.Proveedores.Commands.CrearProveedor;
using GalponERP.Application.Productos.Commands.ActualizarProducto;
using GalponERP.Application.Productos.Commands.EliminarProducto;
using GalponERP.Application.Productos.Queries;
using GalponERP.Application.Productos.Queries.ListarProductos;
using GalponERP.Application.Productos.Queries.ObtenerProductoPorId;
using GalponERP.Application.Clientes.Commands.ActualizarCliente;
using GalponERP.Application.Clientes.Commands.EliminarCliente;
using GalponERP.Application.Clientes.Queries.ListarClientes;
using GalponERP.Application.Clientes.Queries.ObtenerClientePorId;
using GalponERP.Application.Agentes.Confirmacion.Commands;
using GalponERP.Domain.Entities;
using GalponERP.Application.Common;
using MediatR;
using Microsoft.SemanticKernel;
using System.Text;
using System.Text.Json;

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
        [Description("Opcional: Descripción de la categoría")] string? descripcion = null,
        [Description("Opcional: Tipo de categoría (Alimento, Medicamento, Vacuna, Insumo, Otros)")] string? tipo = "Otros")
    {
        try
        {
            if (!Enum.TryParse<TipoCategoria>(tipo, true, out var tipoEnum))
                tipoEnum = TipoCategoria.Otros;

            var command = new CrearCategoriaCommand(nombre, descripcion, tipoEnum);
            var result = await _mediator.Send(command);
            return $"Categoría '{nombre}' (Tipo: {tipoEnum}) creada exitosamente. Ya puedes asignar productos a esta categoría.";
        }
        catch (Exception ex)
        {
            return $"Error al crear la categoría: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Crea un nuevo producto. Resuelve automÃ¡ticamente la categorÃ­a y unidad de medida.")]
    public async Task<string> CrearProducto(
        [Description("Nombre del producto (ej. 'MaÃ­z Amarillo')")] string nombre,
        [Description("Equivalencia en kilogramos para 1 unidad (ej. 50 para un saco de 50kg)")] decimal equivalenciaKg,
        [Description("Opcional: Nombre de la categorÃ­a")] string? nombreCategoria = null,
        [Description("Opcional: Nombre de la unidad de medida (ej. 'Saco', 'Unidad')")] string? nombreUnidad = null,
        [Description("Opcional: Stock mÃ­nimo para alertas")] decimal umbralMinimo = 0)
    {
        // 1. Resolver CategorÃ­a
        var categorias = (await _mediator.Send(new ListarCategoriasQuery())).ToList();
        var (catSeleccionada, msgCat) = EntityResolver.Resolve(categorias, nombreCategoria, c => c.Nombre, "CategorÃ­a");
        if (catSeleccionada == null) return msgCat!;

        // 2. Resolver Unidad de Medida
        var unidades = (await _mediator.Send(new ListarUnidadesMedidaQuery())).ToList();
        var (uniSeleccionada, msgUni) = EntityResolver.Resolve(unidades, nombreUnidad, u => u.Nombre, "Unidad de Medida");
        if (uniSeleccionada == null) return msgUni!;

        // 3. Ejecutar Comando
        try
        {
            var command = new CrearProductoCommand(nombre, catSeleccionada.Id, uniSeleccionada.Id, equivalenciaKg, umbralMinimo);
            var result = await _mediator.Send(command);
            return $"Producto '{nombre}' creado exitosamente en la categorÃ­a '{catSeleccionada.Nombre}' con unidad '{uniSeleccionada.Nombre}'. ID: {result}";
        }
        catch (Exception ex)
        {
            return $"Error al crear el producto: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Registra un nuevo cliente en el sistema.")]
    public async Task<string> RegistrarCliente(
        [Description("Nombre completo o razÃ³n social")] string nombre,
        [Description("RUC o IdentificaciÃ³n")] string ruc,
        [Description("Opcional: DirecciÃ³n")] string? direccion = null,
        [Description("Opcional: TelÃ©fono")] string? telefono = null)
    {
        try
        {
            var command = new CrearClienteCommand(nombre, ruc, direccion, telefono);
            var result = await _mediator.Send(command);
            return $"Cliente '{nombre}' registrado con Ã©xito. ID: {result}";
        }
        catch (Exception ex)
        {
            return $"Error al registrar cliente: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Registra un nuevo proveedor en el sistema.")]
    public async Task<string> RegistrarProveedor(
        [Description("RazÃ³n Social del proveedor")] string razonSocial,
        [Description("NIT o RUC")] string nitRuc,
        [Description("Opcional: TelÃ©fono")] string? telefono = null,
        [Description("Opcional: Email")] string? email = null,
        [Description("Opcional: DirecciÃ³n")] string? direccion = null)
    {
        try
        {
            var command = new CrearProveedorCommand(razonSocial, nitRuc, telefono, email, direccion);
            var result = await _mediator.Send(command);
            return $"Proveedor '{razonSocial}' registrado con Ã©xito. ID: {result}";
        }
        catch (Exception ex)
        {
            return $"Error al registrar proveedor: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Actualiza la informaciÃ³n de un producto existente.")]
    public async Task<string> ActualizarProducto(
        [Description("Nombre actual del producto para identificarlo")] string nombreIdentificador,
        [Description("Opcional: Nuevo nombre")] string? nuevoNombre = null,
        [Description("Opcional: Nueva equivalencia en Kg")] decimal equivalenciaKg = 0,
        [Description("Opcional: Nuevo umbral mÃ­nimo")] decimal umbralMinimo = -1)
    {
        var productos = await _mediator.Send(new ListarProductosQuery());
        var (p, msgP) = EntityResolver.Resolve(productos, nombreIdentificador, x => x.Nombre, "Producto");
        if (p == null) return msgP!;

        try
        {
            var pFull = await _mediator.Send(new ObtenerProductoPorIdQuery(p.Id));
            if (pFull == null) return "No se pudo recuperar el detalle completo del producto.";

            var command = new ActualizarProductoCommand(
                pFull.Id,
                nuevoNombre ?? pFull.Nombre,
                pFull.CategoriaId,
                pFull.UnidadMedidaId,
                equivalenciaKg > 0 ? equivalenciaKg : pFull.PesoUnitarioKg,
                umbralMinimo >= 0 ? umbralMinimo : pFull.UmbralMinimo,
                pFull.StockActual);

            await _mediator.Send(command);
            return $"Producto '{pFull.Nombre}' actualizado correctamente.";
        }
        catch (Exception ex)
        {
            return $"Error al actualizar producto: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Elimina (desactiva) un producto del sistema. Requiere confirmaciÃ³n.")]
    public async Task<string> EliminarProducto(
        [Description("Nombre del producto a eliminar")] string nombre,
        [Description("ID de la conversaciÃ³n actual")] Guid conversacionId,
        [Description("Indica si el usuario ha confirmado la acciÃ³n destructiva")] bool confirmar = false)
    {
        var productos = await _mediator.Send(new ListarProductosQuery());
        var (p, msgP) = EntityResolver.Resolve(productos, nombre, x => x.Nombre, "Producto");
        if (p == null) return msgP!;

        if (!confirmar)
        {
            var parametros = new { nombre, conversacionId };
            var json = JsonSerializer.Serialize(parametros);
            await _mediator.Send(new RegistrarIntencionCommand(conversacionId, nameof(GestionCatalogosPlugin), nameof(EliminarProducto), json));

            return $"âš ï¸ ATENCIÃ“N: EstÃ¡s a punto de eliminar el producto '{p.Nombre}'. Esta acciÃ³n lo desactivarÃ¡ del catÃ¡logo y no podrÃ¡ usarse en nuevos registros. Â¿EstÃ¡s seguro de que deseas continuar?";
        }

        try
        {
            await _mediator.Send(new EliminarProductoCommand(p.Id));
            return $"El producto '{p.Nombre}' ha sido eliminado (desactivado) exitosamente.";
        }
        catch (Exception ex)
        {
            return $"Error al eliminar producto: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Actualiza los datos de un cliente.")]
    public async Task<string> ActualizarCliente(
        [Description("Nombre actual del cliente")] string nombreIdentificador,
        [Description("Opcional: Nuevo nombre")] string? nuevoNombre = null,
        [Description("Opcional: Nuevo RUC/ID")] string? nuevoRuc = null,
        [Description("Opcional: Nueva direcciÃ³n")] string? nuevaDireccion = null,
        [Description("Opcional: Nuevo telÃ©fono")] string? nuevoTelefono = null)
    {
        var clientes = await _mediator.Send(new ListarClientesQuery());
        var (c, msgC) = EntityResolver.Resolve(clientes, nombreIdentificador, x => x.Nombre, "Cliente");
        if (c == null) return msgC!;

        try
        {
            var command = new ActualizarClienteCommand(
                c.Id,
                nuevoNombre ?? c.Nombre,
                nuevoRuc ?? c.Ruc,
                nuevaDireccion ?? c.Direccion,
                nuevoTelefono ?? c.Telefono);

            await _mediator.Send(command);
            return $"Datos del cliente '{c.Nombre}' actualizados con Ã©xito.";
        }
        catch (Exception ex)
        {
            return $"Error al actualizar cliente: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Elimina (desactiva) un cliente del sistema. Requiere confirmaciÃ³n.")]
    public async Task<string> EliminarCliente(
        [Description("Nombre del cliente a eliminar")] string nombre,
        [Description("ID de la conversaciÃ³n actual")] Guid conversacionId,
        [Description("ConfirmaciÃ³n de la acciÃ³n")] bool confirmar = false)
    {
        var clientes = await _mediator.Send(new ListarClientesQuery());
        var (c, msgC) = EntityResolver.Resolve(clientes, nombre, x => x.Nombre, "Cliente");
        if (c == null) return msgC!;

        if (!confirmar)
        {
            var parametros = new { nombre, conversacionId };
            var json = JsonSerializer.Serialize(parametros);
            await _mediator.Send(new RegistrarIntencionCommand(conversacionId, nameof(GestionCatalogosPlugin), nameof(EliminarCliente), json));

            return $"âš ï¸ ADVERTENCIA: Se desactivarÃ¡ al cliente '{c.Nombre}'. Esto impedirÃ¡ realizar nuevas ventas a su nombre. Â¿Confirmas la eliminaciÃ³n?";
        }

        try
        {
            await _mediator.Send(new EliminarClienteCommand(c.Id));
            return $"Cliente '{c.Nombre}' eliminado correctamente.";
        }
        catch (Exception ex)
        {
            return $"Error al eliminar cliente: {ex.Message}";
        }
    }
}
