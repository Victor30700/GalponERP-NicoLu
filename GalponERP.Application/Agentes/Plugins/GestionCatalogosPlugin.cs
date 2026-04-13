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
    [Description("Crea un nuevo producto. Resuelve automáticamente la categoría y unidad de medida.")]
    public async Task<string> CrearProducto(
        [Description("Nombre del producto (ej. 'Maíz Amarillo')")] string nombre,
        [Description("Equivalencia en kilogramos para 1 unidad (ej. 50 para un saco de 50kg)")] decimal equivalenciaKg,
        [Description("Opcional: Nombre de la categoría")] string? nombreCategoria = null,
        [Description("Opcional: Nombre de la unidad de medida (ej. 'Saco', 'Unidad')")] string? nombreUnidad = null,
        [Description("Opcional: Stock mínimo para alertas")] decimal umbralMinimo = 0)
    {
        // 1. Resolver Categoría
        var categorias = (await _mediator.Send(new ListarCategoriasQuery())).ToList();
        var (catSeleccionada, msgCat) = EntityResolver.Resolve(categorias, nombreCategoria, c => c.Nombre, "Categoría");
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

    [KernelFunction]
    [Description("Actualiza la información de un producto existente.")]
    public async Task<string> ActualizarProducto(
        [Description("Nombre actual del producto para identificarlo")] string nombreIdentificador,
        [Description("Opcional: Nuevo nombre")] string? nuevoNombre = null,
        [Description("Opcional: Nueva equivalencia en Kg")] decimal equivalenciaKg = 0,
        [Description("Opcional: Nuevo umbral mínimo")] decimal umbralMinimo = -1)
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
                equivalenciaKg > 0 ? equivalenciaKg : pFull.EquivalenciaEnKg,
                umbralMinimo >= 0 ? umbralMinimo : pFull.UmbralMinimo);

            await _mediator.Send(command);
            return $"Producto '{pFull.Nombre}' actualizado correctamente.";
        }
        catch (Exception ex)
        {
            return $"Error al actualizar producto: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Elimina (desactiva) un producto del sistema. Requiere confirmación.")]
    public async Task<string> EliminarProducto(
        [Description("Nombre del producto a eliminar")] string nombre,
        [Description("ID de la conversación actual")] Guid conversacionId,
        [Description("Indica si el usuario ha confirmado la acción destructiva")] bool confirmar = false)
    {
        var productos = await _mediator.Send(new ListarProductosQuery());
        var (p, msgP) = EntityResolver.Resolve(productos, nombre, x => x.Nombre, "Producto");
        if (p == null) return msgP!;

        if (!confirmar)
        {
            var parametros = new { nombre, conversacionId };
            var json = JsonSerializer.Serialize(parametros);
            await _mediator.Send(new RegistrarIntencionCommand(conversacionId, nameof(GestionCatalogosPlugin), nameof(EliminarProducto), json));

            return $"⚠️ ATENCIÓN: Estás a punto de eliminar el producto '{p.Nombre}'. Esta acción lo desactivará del catálogo y no podrá usarse en nuevos registros. ¿Estás seguro de que deseas continuar?";
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
        [Description("Opcional: Nueva dirección")] string? nuevaDireccion = null,
        [Description("Opcional: Nuevo teléfono")] string? nuevoTelefono = null)
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
            return $"Datos del cliente '{c.Nombre}' actualizados con éxito.";
        }
        catch (Exception ex)
        {
            return $"Error al actualizar cliente: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Elimina (desactiva) un cliente del sistema. Requiere confirmación.")]
    public async Task<string> EliminarCliente(
        [Description("Nombre del cliente a eliminar")] string nombre,
        [Description("ID de la conversación actual")] Guid conversacionId,
        [Description("Confirmación de la acción")] bool confirmar = false)
    {
        var clientes = await _mediator.Send(new ListarClientesQuery());
        var (c, msgC) = EntityResolver.Resolve(clientes, nombre, x => x.Nombre, "Cliente");
        if (c == null) return msgC!;

        if (!confirmar)
        {
            var parametros = new { nombre, conversacionId };
            var json = JsonSerializer.Serialize(parametros);
            await _mediator.Send(new RegistrarIntencionCommand(conversacionId, nameof(GestionCatalogosPlugin), nameof(EliminarCliente), json));

            return $"⚠️ ADVERTENCIA: Se desactivará al cliente '{c.Nombre}'. Esto impedirá realizar nuevas ventas a su nombre. ¿Confirmas la eliminación?";
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
