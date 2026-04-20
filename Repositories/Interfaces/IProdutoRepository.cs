using MaxiMassas.Entities;

namespace MaxiMassas.Repositories.Interfaces;

public interface IProdutoRepository
{
    Task<IEnumerable<Produto>> GetAllAsync();
    Task<Produto?> GetByIdAsync(int id);
    Task<Produto?> GetByIdWithEstoqueAsync(int id);
    Task<Produto> CreateAsync(Produto produto);
    Task<Produto> UpdateAsync(Produto produto);
    Task DeleteAsync(Produto produto);
    Task<bool> ExistsAsync(int id);
    Task<IEnumerable<HistoricoPreco>> GetHistoricoPrecoAsync(int produtoId);
    Task AddHistoricoPrecoAsync(HistoricoPreco historico);
}
