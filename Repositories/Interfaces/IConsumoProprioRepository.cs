using MaxiMassas.Entities;

namespace MaxiMassas.Repositories.Interfaces;

public interface IConsumoProprioRepository
{
    Task<IEnumerable<ConsumoProprio>> GetAllAsync();
    Task<IEnumerable<ConsumoProprio>> GetByProdutoIdAsync(int produtoId);
    Task<ConsumoProprio?> GetByIdAsync(int id);
    Task<ConsumoProprio> CreateAsync(ConsumoProprio consumo);
    Task DeleteAsync(ConsumoProprio consumo);
}
