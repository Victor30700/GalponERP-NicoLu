namespace GalponERP.Domain.Exceptions;

/// <summary>
/// Excepciones específicas para la lógica del Calendario Sanitario.
/// </summary>
public class CalendarioDomainException : DomainException
{
    public CalendarioDomainException(string message) : base(message) { }
}
