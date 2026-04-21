namespace GalponERP.Application.Exceptions;

public class ConcurrencyException : Exception
{
    public ConcurrencyException(string message) : base(message)
    {
    }

    public ConcurrencyException() : base("El registro fue modificado o eliminado por otro usuario. Por favor, recargue los datos e intente nuevamente.")
    {
    }
}
