using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class ConversacionRepository : IConversacionRepository
{
    private readonly GalponDbContext _context;

    public ConversacionRepository(GalponDbContext context)
    {
        _context = context;
    }

    public void Agregar(Conversacion conversacion)
    {
        _context.Conversaciones.Add(conversacion);
    }

    public void AgregarMensaje(MensajeChat mensaje)
    {
        _context.MensajesChat.Add(mensaje);
    }

    public async Task<Conversacion?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Conversaciones
            .Include(c => c.Mensajes)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Conversacion>> ObtenerPorUsuarioAsync(Guid usuarioId)
    {
        return await _context.Conversaciones
            .Include(c => c.Mensajes)
            .Where(c => c.UsuarioId == usuarioId)
            .OrderByDescending(c => c.FechaInicio)
            .ToListAsync();
    }

    public async Task<IEnumerable<MensajeChat>> ObtenerUltimosMensajesAsync(Guid conversacionId, int cantidad)
    {
        return await _context.MensajesChat
            .Where(m => m.ConversacionId == conversacionId)
            .OrderByDescending(m => m.Fecha)
            .Take(cantidad)
            .OrderBy(m => m.Fecha) // Retornar en orden cronológico para el ChatHistory
            .ToListAsync();
    }

    public void Eliminar(Conversacion conversacion)
    {
        _context.Conversaciones.Remove(conversacion);
    }

    public void Actualizar(Conversacion conversacion)
    {
        _context.Conversaciones.Update(conversacion);
    }
}
