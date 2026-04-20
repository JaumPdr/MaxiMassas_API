using MaxiMassas.Entities;

namespace MaxiMassas.Repositories.Interfaces;

public interface IEstoqueRepository
{
    Task<IEnumerable<Estoque>> GetAllAsync();
    Task<Estoque?> GetByProdutoIdAsync(int produtoId);
    Task<Estoque> CreateAsync(Estoque estoque);
    Task<Estoque> UpdateAsync(Estoque estoque);
    Task<IEnumerable<Estoque>> GetEstoqueBaixoAsync(int limite = 3);
}
