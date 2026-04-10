namespace GalponERP.Domain.Exceptions;

/// <summary>
/// Excepción específica para errores en la lógica de negocio de Inventario.
/// </summary>
public class InventarioDomainException : DomainException
{
    public InventarioDomainException(string message) : base(message) { }
}
