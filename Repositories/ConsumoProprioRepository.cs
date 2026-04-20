using MaxiMassas.Data;
using MaxiMassas.Entities;
using MaxiMassas.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MaxiMassas.Repositories;

public class ConsumoProprioRepository : IConsumoProprioRepository
{
    private readonly AppDbContext _context;

    public ConsumoProprioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ConsumoProprio>> GetAllAsync()
    {
        return await _context.ConsumosProprios
            .Include(c => c.Produto)
            .OrderByDescending(c => c.Data)
            .ToListAsync();
    }

    public async Task<IEnumerable<ConsumoProprio>> GetByProdutoIdAsync(int produtoId)
    {
        return await _context.ConsumosProprios
            .Include(c => c.Produto)
            .Where(c => c.ProdutoId == produtoId)
            .OrderByDescending(c => c.Data)
            .ToListAsync();
    }

    public async Task<ConsumoProprio?> GetByIdAsync(int id)
    {
        return await _context.ConsumosProprios
            .Include(c => c.Produto)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<ConsumoProprio> CreateAsync(ConsumoProprio consumo)
    {
        _context.ConsumosProprios.Add(consumo);
        await _context.SaveChangesAsync();
        return consumo;
    }

    public async Task DeleteAsync(ConsumoProprio consumo)
    {
        _context.ConsumosProprios.Remove(consumo);
        await _context.SaveChangesAsync();
    }
}
