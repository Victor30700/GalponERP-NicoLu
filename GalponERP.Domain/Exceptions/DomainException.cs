namespace GalponERP.Domain.Exceptions;

/// <summary>
/// Excepción base para errores de lógica de negocio en el dominio.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}
