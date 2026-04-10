namespace GalponERP.Domain.ValueObjects;

/// <summary>
/// Objeto de Valor que representa dinero en Bolivianos (BS).
/// Encapsula la lógica de redondeo y validaciones de dinero.
/// </summary>
public record Moneda
{
    public decimal Monto { get; init; }

    public Moneda(decimal monto)
    {
        if (monto < 0)
        {
            throw new ArgumentException("El monto no puede ser negativo.", nameof(monto));
        }

        // Redondeo estándar para transacciones financieras (2 decimales)
        Monto = Math.Round(monto, 2, MidpointRounding.AwayFromZero);
    }

    // Operadores matemáticos básicos para facilitar el manejo de dinero
    public static Moneda operator +(Moneda a, Moneda b) => new(a.Monto + b.Monto);
    public static Moneda operator -(Moneda a, Moneda b) => new(a.Monto - b.Monto);
    public static Moneda operator *(Moneda a, decimal b) => new(a.Monto * b);
    public static Moneda operator *(decimal a, Moneda b) => new(a * b.Monto);
    
    // Comparaciones implícitas ya manejadas por record (value equality)

    public override string ToString() => $"{Monto:N2} BS";

    public static Moneda Zero => new(0);
}
