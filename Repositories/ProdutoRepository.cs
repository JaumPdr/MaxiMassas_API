using MaxiMassas.Data;
using MaxiMassas.Entities;
using MaxiMassas.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MaxiMassas.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Produto>> GetAllAsync()
    {
        return await _context.Produtos
            .Include(p => p.Estoque)
            .Where(p => p.Ativo)
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }

    public async Task<Produto?> GetByIdAsync(int id)
    {
        return await _context.Produtos
            .Include(p => p.Estoque)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Produto?> GetByIdWithEstoqueAsync(int id)
    {
        return await _context.Produtos
            .Include(p => p.Estoque)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Produto> CreateAsync(Produto produto)
    {
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
        return produto;
    }

    public async Task<Produto> UpdateAsync(Produto produto)
    {
        _context.Produtos.Update(produto);
        await _context.SaveChangesAsync();
        return produto;
    }

    public async Task DeleteAsync(Produto produto)
    {
        produto.Ativo = false;
        _context.Produtos.Update(produto);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Produtos.AnyAsync(p => p.Id == id && p.Ativo);
    }

    public async Task<IEnumerable<HistoricoPreco>> GetHistoricoPrecoAsync(int produtoId)
    {
        return await _context.HistoricoPrecos
            .Include(h => h.Produto)
            .Where(h => h.ProdutoId == produtoId)
            .OrderByDescending(h => h.AlteradoEm)
            .ToListAsync();
    }

    public async Task AddHistoricoPrecoAsync(HistoricoPreco historico)
    {
        _context.HistoricoPrecos.Add(historico);
        await _context.SaveChangesAsync();
    }
}
