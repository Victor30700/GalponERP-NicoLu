using GalponERP.Domain.Primitives;
using GalponERP.Domain.Exceptions;

namespace GalponERP.Domain.Entities;

/// <summary>
/// Registra parámetros de bienestar y ambientales de un lote en una fecha específica.
/// </summary>
public class RegistroBienestar : Entity
{
    public Guid LoteId { get; private set; }
    public DateTime Fecha { get; private set; }
    public decimal? Temperatura { get; private set; }
    public decimal? Humedad { get; private set; }
    public decimal? LecturaMedidor { get; private set; } // Lectura del contador de agua
    public decimal? ConsumoAgua { get; private set; } // Litros (calculado o directo)
    public decimal? Ph { get; private set; }
    public decimal? CloroPpm { get; private set; }
    public string? Observaciones { get; private set; }
    public Guid UsuarioId { get; private set; }

    public RegistroBienestar(Guid id, Guid loteId, DateTime fecha, decimal? temperatura, decimal? humedad, decimal? consumoAgua, string? observaciones, Guid usuarioId, decimal? ph = null, decimal? cloroPpm = null, decimal? lecturaMedidor = null) 
        : base(id)
    {
        if (loteId == Guid.Empty)
            throw new LoteDomainException("El ID del lote es obligatorio.");

        if (usuarioId == Guid.Empty)
            throw new LoteDomainException("El ID del usuario es obligatorio para auditoría.");

        LoteId = loteId;
        Fecha = fecha.Date; // Aseguramos que solo sea la fecha
        Temperatura = temperatura;
        Humedad = humedad;
        ConsumoAgua = consumoAgua;
        LecturaMedidor = lecturaMedidor;
        Ph = ph;
        CloroPpm = cloroPpm;
        Observaciones = observaciones;
        UsuarioId = usuarioId;
    }

    public void Actualizar(decimal? temperatura, decimal? humedad, decimal? consumoAgua, string? observaciones, decimal? ph = null, decimal? cloroPpm = null, decimal? lecturaMedidor = null)
    {
        Temperatura = temperatura;
        Humedad = humedad;
        ConsumoAgua = consumoAgua;
        LecturaMedidor = lecturaMedidor;
        Ph = ph;
        CloroPpm = cloroPpm;
        Observaciones = observaciones;
    }

    public void CalcularConsumo(decimal lecturaAnterior)
    {
        if (LecturaMedidor.HasValue && LecturaMedidor.Value >= lecturaAnterior)
        {
            ConsumoAgua = LecturaMedidor.Value - lecturaAnterior;
        }
    }

    // Constructor para EF Core
    private RegistroBienestar() : base() { }
}
