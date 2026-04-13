using System.ComponentModel;
using GalponERP.Application.Usuarios.Queries.ObtenerUsuarios;
using GalponERP.Application.Usuarios.Commands.CambiarRolUsuario;
using GalponERP.Application.Usuarios.Commands.CambiarEstadoBloqueoUsuario;
using GalponERP.Application.Common;
using GalponERP.Domain.Entities;
using MediatR;
using Microsoft.SemanticKernel;
using System.Text;
using System.Text.Json;
using GalponERP.Application.Agentes.Confirmacion.Commands;

namespace GalponERP.Application.Agentes.Plugins;

public class AdministracionPlugin
{
    private readonly IMediator _mediator;

    public AdministracionPlugin(IMediator mediator)
    {
        _mediator = mediator;
    }

    [KernelFunction]
    [Description("Lista todos los usuarios del sistema, mostrando sus roles y si están bloqueados o activos.")]
    public async Task<string> ListarUsuarios()
    {
        var usuarios = await _mediator.Send(new ObtenerUsuariosQuery());
        if (!usuarios.Any()) return "No hay usuarios registrados en el sistema.";

        var sb = new StringBuilder();
        sb.AppendLine("Usuarios registrados en GalponERP:");
        
        foreach (var u in usuarios.OrderBy(x => x.Nombre))
        {
            var estado = u.IsActive ? "✅ ACTIVO" : "❌ BLOQUEADO";
            sb.AppendLine($"- {u.Nombre} ({u.Email}) | Rol: {u.Rol} | Estado: {estado}");
        }

        return sb.ToString();
    }

    [KernelFunction]
    [Description("Cambia el rol de un usuario en el sistema. Requiere confirmación.")]
    public async Task<string> CambiarRolUsuario(
        [Description("Nombre del usuario a modificar (búsqueda difusa)")] string nombreUsuario,
        [Description("Nuevo rol a asignar: 'Empleado', 'SubAdmin' o 'Admin'")] string nuevoRol,
        [Description("ID de la conversación actual (Guid)")] Guid conversacionId,
        [Description("Obligatorio para ejecutar la acción: confirmar=true")] bool confirmar = false)
    {
        // 1. Resolver Usuario
        var usuarios = await _mediator.Send(new ObtenerUsuariosQuery());
        var (usuario, msgUser) = EntityResolver.Resolve(usuarios, nombreUsuario, u => u.Nombre, "Usuario");
        if (usuario == null) return msgUser!;

        // 2. Validar Rol
        if (!Enum.TryParse<RolGalpon>(nuevoRol, true, out var rolEnum))
            return $"El rol '{nuevoRol}' no es válido. Opciones: Empleado, SubAdmin, Admin.";

        // 3. Confirmación
        if (!confirmar)
        {
            var parametros = new
            {
                nombreUsuario = usuario.Nombre,
                nuevoRol = rolEnum.ToString(),
                conversacionId
            };
            
            var json = JsonSerializer.Serialize(parametros);
            await _mediator.Send(new RegistrarIntencionCommand(conversacionId, nameof(AdministracionPlugin), nameof(CambiarRolUsuario), json));

            return $"¿Confirmas que deseas cambiar el rol del usuario '{usuario.Nombre}' a '{rolEnum}'?";
        }

        // 4. Ejecutar
        try
        {
            await _mediator.Send(new CambiarRolUsuarioCommand(usuario.Id, rolEnum));
            return $"Rol del usuario '{usuario.Nombre}' cambiado exitosamente a '{rolEnum}'.";
        }
        catch (Exception ex)
        {
            return $"Error al cambiar el rol: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Bloquea o desbloquea el acceso al sistema de un usuario. Requiere confirmación.")]
    public async Task<string> CambiarEstadoBloqueoUsuario(
        [Description("Nombre del usuario a modificar (búsqueda difusa)")] string nombreUsuario,
        [Description("True para bloquear al usuario, False para desbloquear (reactivar)")] bool bloquear,
        [Description("ID de la conversación actual (Guid)")] Guid conversacionId,
        [Description("Obligatorio para ejecutar la acción: confirmar=true")] bool confirmar = false)
    {
        // 1. Resolver Usuario
        var usuarios = await _mediator.Send(new ObtenerUsuariosQuery());
        var (usuario, msgUser) = EntityResolver.Resolve(usuarios, nombreUsuario, u => u.Nombre, "Usuario");
        if (usuario == null) return msgUser!;

        var accionTexto = bloquear ? "BLOQUEAR" : "DESBLOQUEAR";

        // 2. Confirmación
        if (!confirmar)
        {
            var parametros = new
            {
                nombreUsuario = usuario.Nombre,
                bloquear,
                conversacionId
            };
            
            var json = JsonSerializer.Serialize(parametros);
            await _mediator.Send(new RegistrarIntencionCommand(conversacionId, nameof(AdministracionPlugin), nameof(CambiarEstadoBloqueoUsuario), json));

            return $"⚠️ ¿Estás seguro de que deseas {accionTexto} al usuario '{usuario.Nombre}'?";
        }

        // 3. Ejecutar
        try
        {
            await _mediator.Send(new CambiarEstadoBloqueoUsuarioCommand(usuario.Id, bloquear));
            return $"El usuario '{usuario.Nombre}' ha sido {(bloquear ? "bloqueado" : "desbloqueado")} exitosamente.";
        }
        catch (Exception ex)
        {
            return $"Error al cambiar estado de bloqueo: {ex.Message}";
        }
    }
}
