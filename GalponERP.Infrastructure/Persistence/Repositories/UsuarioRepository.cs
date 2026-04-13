using GalponERP.Domain.Entities;
using GalponERP.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalponERP.Infrastructure.Persistence.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly GalponDbContext _context;

    public UsuarioRepository(GalponDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Usuarios.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Usuario?> ObtenerPorFirebaseUidAsync(string firebaseUid)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
    }

    public async Task<Usuario?> ObtenerPorEmailAsync(string email)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Usuario?> ObtenerPorTelefonoAsync(string telefono)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Telefono == telefono);
    }

    public async Task<Usuario?> ObtenerPorWhatsAppAsync(string whatsAppNumero)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.WhatsAppNumero == whatsAppNumero);
    }

    public async Task<Usuario?> ObtenerPorCodigoVinculacionAsync(string codigo)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.CodigoVinculacion == codigo);
    }

    public async Task<IEnumerable<Usuario>> ObtenerPorRolAsync(RolGalpon rol)
    {
        return await _context.Usuarios.Where(u => u.Rol == rol).ToListAsync();
    }

    public async Task<IEnumerable<Usuario>> ObtenerTodosAsync()
    {
        return await _context.Usuarios.ToListAsync();
    }

    public async Task<IEnumerable<Usuario>> ObtenerTodosConInactivosAsync()
    {
        return await _context.Usuarios.IgnoreQueryFilters().ToListAsync();
    }

    public void Agregar(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
    }

    public void Actualizar(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
    }
}
