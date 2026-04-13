using GalponERP.Domain.Entities;

namespace GalponERP.Domain.Interfaces.Repositories;

public interface IConversacionRepository
{
    void Agregar(Conversacion conversacion);
    void AgregarMensaje(MensajeChat mensaje);
    Task<Conversacion?> ObtenerPorIdAsync(Guid id);
    Task<IEnumerable<Conversacion>> ObtenerPorUsuarioAsync(Guid usuarioId);
    Task<IEnumerable<MensajeChat>> ObtenerUltimosMensajesAsync(Guid conversacionId, int cantidad);
    void Eliminar(Conversacion conversacion);
    void Actualizar(Conversacion conversacion);
}
