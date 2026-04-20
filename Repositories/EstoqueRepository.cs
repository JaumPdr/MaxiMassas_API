using MaxiMassas.Data;
using MaxiMassas.Entities;
using MaxiMassas.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MaxiMassas.Repositories;

public class EstoqueRepository : IEstoqueRepository
{
    private readonly AppDbContext _context;

    public EstoqueRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Estoque>> GetAllAsync()
    {
        return await _context.Estoques
            .Include(e => e.Produto)
            .OrderBy(e => e.Produto.Nome)
            .ToListAsync();
    }

    public async Task<Estoque?> GetByProdutoIdAsync(int produtoId)
    {
        return await _context.Estoques
            .Include(e => e.Produto)
            .FirstOrDefaultAsync(e => e.ProdutoId == produtoId);
    }

    public async Task<Estoque> CreateAsync(Estoque estoque)
    {
        _context.Estoques.Add(estoque);
        await _context.SaveChangesAsync();
        return estoque;
    }

    public async Task<Estoque> UpdateAsync(Estoque estoque)
    {
        estoque.AtualizadoEm = DateTime.UtcNow;
        _context.Estoques.Update(estoque);
        await _context.SaveChangesAsync();
        return estoque;
    }

    public async Task<IEnumerable<Estoque>> GetEstoqueBaixoAsync(int limite = 3)
    {
        return await _context.Estoques
            .Include(e => e.Produto)
            .Where(e => e.Quantidade <= limite && e.Produto.Ativo)
            .OrderBy(e => e.Quantidade)
            .ToListAsync();
    }
}
