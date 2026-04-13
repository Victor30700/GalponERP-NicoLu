using GalponERP.Domain.Primitives;
using GalponERP.Domain.ValueObjects;
using GalponERP.Domain.Exceptions;

namespace GalponERP.Domain.Entities;

public enum EstadoOrdenCompra
{
    Pendiente,
    Recibida,
    Cancelada
}

public class OrdenCompra : Entity
{
    public Guid ProveedorId { get; private set; }
    public Proveedor Proveedor { get; private set; } = null!;
    public DateTime Fecha { get; private set; }
    public Moneda Total { get; private set; } = null!;
    public EstadoOrdenCompra Estado { get; private set; }
    public string? Nota { get; private set; }
    public Guid UsuarioIdRegistro { get; private set; }

    private readonly List<OrdenCompraItem> _items = new();
    public IReadOnlyCollection<OrdenCompraItem> Items => _items.AsReadOnly();

    public OrdenCompra(Guid id, Guid proveedorId, DateTime fecha, Moneda total, Guid usuarioIdRegistro, string? nota = null) 
        : base(id)
    {
        if (proveedorId == Guid.Empty)
            throw new InventarioDomainException("El ID del proveedor es obligatorio.");

        if (usuarioIdRegistro == Guid.Empty)
            throw new InventarioDomainException("El ID del usuario es obligatorio para auditoría.");

        ProveedorId = proveedorId;
        Fecha = fecha;
        Total = total;
        UsuarioIdRegistro = usuarioIdRegistro;
        Nota = nota;
        Estado = EstadoOrdenCompra.Pendiente;
    }

    public void AgregarItem(Guid id, Guid productoId, decimal cantidad, Moneda precioUnitario)
    {
        if (Estado != EstadoOrdenCompra.Pendiente)
            throw new InventarioDomainException("Solo se pueden agregar ítems a una orden de compra pendiente.");

        var item = new OrdenCompraItem(id, Id, productoId, cantidad, precioUnitario);
        _items.Add(item);
        
        RecalcularTotal();
    }

    public void MarcarComoRecibida()
    {
        if (Estado != EstadoOrdenCompra.Pendiente)
            throw new InventarioDomainException("Solo se pueden marcar como recibidas las órdenes pendientes.");

        Estado = EstadoOrdenCompra.Recibida;
    }

    public void Cancelar(string motivo)
    {
        if (Estado != EstadoOrdenCompra.Pendiente)
            throw new InventarioDomainException("Solo se pueden cancelar órdenes pendientes.");

        Estado = EstadoOrdenCompra.Cancelada;
        Nota = string.IsNullOrWhiteSpace(Nota) ? $"Cancelada: {motivo}" : $"{Nota} | Cancelada: {motivo}";
    }

    private void RecalcularTotal()
    {
        var totalMonto = _items.Sum(i => i.Total.Monto);
        Total = new Moneda(totalMonto);
    }

    // Constructor para EF Core
    private OrdenCompra() : base() { }
}

public class OrdenCompraItem : Entity
{
    public Guid OrdenCompraId { get; private set; }
    public Guid ProductoId { get; private set; }
    public Producto Producto { get; private set; } = null!;
    public decimal Cantidad { get; private set; }
    public Moneda PrecioUnitario { get; private set; } = null!;
    public Moneda Total { get; private set; } = null!;

    public OrdenCompraItem(Guid id, Guid ordenCompraId, Guid productoId, decimal cantidad, Moneda precioUnitario) 
        : base(id)
    {
        if (ordenCompraId == Guid.Empty)
            throw new InventarioDomainException("El ID de la orden de compra es obligatorio.");

        if (productoId == Guid.Empty)
            throw new InventarioDomainException("El ID del producto es obligatorio.");

        if (cantidad <= 0)
            throw new InventarioDomainException("La cantidad debe ser mayor a cero.");

        OrdenCompraId = ordenCompraId;
        ProductoId = productoId;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
        Total = new Moneda(cantidad * precioUnitario.Monto);
    }

    // Constructor para EF Core
    private OrdenCompraItem() : base() { }
}
