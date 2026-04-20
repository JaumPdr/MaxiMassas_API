using MaxiMassas.Entities;

namespace MaxiMassas.Repositories.Interfaces;

public interface IVendaRepository
{
    Task<IEnumerable<Venda>> GetAllAsync();
    Task<IEnumerable<Venda>> GetByClienteIdAsync(int clienteId);
    Task<IEnumerable<Venda>> GetByPeriodoAsync(DateTime inicio, DateTime fim);
    Task<Venda?> GetByIdAsync(int id);
    Task<Venda?> GetByIdWithItensAsync(int id);
    Task<Venda> CreateAsync(Venda venda);
    Task<Venda> UpdateAsync(Venda venda);
    Task DeleteAsync(Venda venda);
    Task<bool> ExistsAsync(int id);
}
