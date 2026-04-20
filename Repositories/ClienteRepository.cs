using MaxiMassas.Data;
using MaxiMassas.Entities;
using MaxiMassas.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MaxiMassas.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Cliente>> GetAllAsync()
    {
        return await _context.Clientes
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        return await _context.Clientes.FindAsync(id);
    }

    public async Task<Cliente?> GetByIdWithVendasAsync(int id)
    {
        return await _context.Clientes
            .Include(c => c.Vendas)
                .ThenInclude(v => v.Itens)
                    .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Cliente> CreateAsync(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return cliente;
    }

    public async Task<Cliente> UpdateAsync(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
        return cliente;
    }

    public async Task DeleteAsync(Cliente cliente)
    {
        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Clientes.AnyAsync(c => c.Id == id);
    }
}
