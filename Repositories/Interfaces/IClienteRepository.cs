using MaxiMassas.Entities;

namespace MaxiMassas.Repositories.Interfaces;

public interface IClienteRepository
{
    Task<IEnumerable<Cliente>> GetAllAsync();
    Task<Cliente?> GetByIdAsync(int id);
    Task<Cliente?> GetByIdWithVendasAsync(int id);
    Task<Cliente> CreateAsync(Cliente cliente);
    Task<Cliente> UpdateAsync(Cliente cliente);
    Task DeleteAsync(Cliente cliente);
    Task<bool> ExistsAsync(int id);
}
