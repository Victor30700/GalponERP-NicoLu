namespace GalponERP.Domain.Exceptions;

/// <summary>
/// Excepción específica para errores en la lógica de negocio de los Lotes.
/// </summary>
public class LoteDomainException : DomainException
{
    public LoteDomainException(string message) : base(message) { }
}
