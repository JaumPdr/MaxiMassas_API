using MaxiMassas.DTOs.Auth;
using MaxiMassas.DTOs.Cliente;
using MaxiMassas.DTOs.ConsumoProprio;
using MaxiMassas.DTOs.Estoque;
using MaxiMassas.DTOs.Produto;
using MaxiMassas.DTOs.Relatorio;
using MaxiMassas.DTOs.Venda;

namespace MaxiMassas.Services.Interfaces;

// ─── Auth ───────────────────────────────────────────────────────────────────
public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
    Task<UsuarioResponseDto> RegistrarAsync(RegistrarUsuarioDto dto);
    Task<IEnumerable<UsuarioResponseDto>> GetUsuariosAsync();
}

// ─── Cliente ─────────────────────────────────────────────────────────────────
public interface IClienteService
{
    Task<IEnumerable<ClienteResponseDto>> GetAllAsync();
    Task<ClienteResponseDto> GetByIdAsync(int id);
    Task<ClienteResponseDto> CreateAsync(ClienteCreateDto dto);
    Task<ClienteResponseDto> UpdateAsync(int id, ClienteUpdateDto dto);
    Task DeleteAsync(int id);
    Task<ClienteHistoricoDto> GetHistoricoAsync(int id);
}

// ─── Produto ─────────────────────────────────────────────────────────────────
public interface IProdutoService
{
    Task<IEnumerable<ProdutoResponseDto>> GetAllAsync();
    Task<ProdutoResponseDto> GetByIdAsync(int id);
    Task<ProdutoResponseDto> CreateAsync(ProdutoCreateDto dto);
    Task<ProdutoResponseDto> UpdateAsync(int id, ProdutoUpdateDto dto);
    Task DeleteAsync(int id);
    Task<IEnumerable<HistoricoPrecoDto>> GetHistoricoPrecoAsync(int produtoId);
}

// ─── Venda ───────────────────────────────────────────────────────────────────
public interface IVendaService
{
    Task<IEnumerable<VendaResponseDto>> GetAllAsync();
    Task<VendaResponseDto> GetByIdAsync(int id);
    Task<VendaResponseDto> CreateAsync(VendaCreateDto dto);
    Task<VendaResponseDto> UpdateAsync(int id, VendaUpdateDto dto);
    Task DeleteAsync(int id);
}

// ─── Estoque ─────────────────────────────────────────────────────────────────
public interface IEstoqueService
{
    Task<IEnumerable<EstoqueResponseDto>> GetAllAsync();
    Task<EstoqueResponseDto> GetByProdutoIdAsync(int produtoId);
    Task<EstoqueResponseDto> ReporAsync(EstoqueReposicaoDto dto);
    Task<EstoqueResponseDto> AjustarAsync(EstoqueAjusteDto dto);
    Task<IEnumerable<EstoqueResponseDto>> GetAlertasBaixoAsync();
}

// ─── ConsumoProprio ──────────────────────────────────────────────────────────
public interface IConsumoProprioService
{
    Task<IEnumerable<ConsumoProprioResponseDto>> GetAllAsync();
    Task<ConsumoProprioResponseDto> GetByIdAsync(int id);
    Task<IEnumerable<ConsumoProprioResponseDto>> GetByProdutoIdAsync(int produtoId);
    Task<ConsumoProprioResponseDto> CreateAsync(ConsumoProprioCreateDto dto);
    Task DeleteAsync(int id);
}

// ─── Relatório ───────────────────────────────────────────────────────────────
public interface IRelatorioService
{
    Task<RelatorioFinanceiroDto> GetRelatorioFinanceiroAsync(RelatorioFiltroDto filtro);
    Task<List<TopClienteDto>> GetTopClientesAsync(RelatorioFiltroDto filtro, int top = 5);
    Task<List<ProdutoMaisVendidoDto>> GetProdutosMaisVendidosAsync(RelatorioFiltroDto filtro, int top = 10);
    Task<RelatorioCompletoDto> GetRelatorioCompletoAsync(RelatorioFiltroDto filtro);
}
