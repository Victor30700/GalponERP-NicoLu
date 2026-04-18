namespace GalponERP.Domain.Exceptions;

/// <summary>
/// Excepción específica para errores en la lógica de negocio de los Usuarios.
/// </summary>
public class UsuarioDomainException : DomainException
{
    public UsuarioDomainException(string message) : base(message) { }
}
