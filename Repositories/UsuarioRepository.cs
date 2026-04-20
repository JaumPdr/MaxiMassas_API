using MaxiMassas.Data;
using MaxiMassas.Entities;
using MaxiMassas.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MaxiMassas.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.Ativo);
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        return await _context.Usuarios.FindAsync(id);
    }

    public async Task<IEnumerable<Usuario>> GetAllAsync()
    {
        return await _context.Usuarios
            .Where(u => u.Ativo)
            .OrderBy(u => u.Nome)
            .ToListAsync();
    }

    public async Task<Usuario> CreateAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task<Usuario> UpdateAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Usuarios
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }
}
