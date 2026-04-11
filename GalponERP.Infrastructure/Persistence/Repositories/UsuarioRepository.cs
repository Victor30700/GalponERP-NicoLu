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
        return await _context.Usuarios.FindAsync(id);
    }

    public async Task<Usuario?> ObtenerPorFirebaseUidAsync(string firebaseUid)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
    }

    public async Task<Usuario?> ObtenerPorEmailAsync(string email)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<Usuario>> ObtenerPorRolAsync(RolGalpon rol)
    {
        return await _context.Usuarios.Where(u => u.Rol == rol).ToListAsync();
    }

    public async Task<IEnumerable<Usuario>> ObtenerTodosAsync()
    {
        return await _context.Usuarios.ToListAsync();
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
