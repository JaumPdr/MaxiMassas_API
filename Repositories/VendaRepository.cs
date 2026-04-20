using MaxiMassas.Data;
using MaxiMassas.Entities;
using MaxiMassas.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MaxiMassas.Repositories;

public class VendaRepository : IVendaRepository
{
    private readonly AppDbContext _context;

    public VendaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Venda>> GetAllAsync()
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Itens)
                .ThenInclude(i => i.Produto)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync();
    }

    public async Task<IEnumerable<Venda>> GetByClienteIdAsync(int clienteId)
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Itens)
                .ThenInclude(i => i.Produto)
            .Where(v => v.ClienteId == clienteId)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync();
    }

    public async Task<IEnumerable<Venda>> GetByPeriodoAsync(DateTime inicio, DateTime fim)
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Itens)
                .ThenInclude(i => i.Produto)
            .Where(v => v.DataVenda >= inicio && v.DataVenda <= fim)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync();
    }

    public async Task<Venda?> GetByIdAsync(int id)
    {
        return await _context.Vendas.FindAsync(id);
    }

    public async Task<Venda?> GetByIdWithItensAsync(int id)
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Itens)
                .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<Venda> CreateAsync(Venda venda)
    {
        _context.Vendas.Add(venda);
        await _context.SaveChangesAsync();
        return venda;
    }

    public async Task<Venda> UpdateAsync(Venda venda)
    {
        _context.Vendas.Update(venda);
        await _context.SaveChangesAsync();
        return venda;
    }

    public async Task DeleteAsync(Venda venda)
    {
        _context.Vendas.Remove(venda);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Vendas.AnyAsync(v => v.Id == id);
    }
}
