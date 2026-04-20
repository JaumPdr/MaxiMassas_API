using MaxiMassas.DTOs.Estoque;
using MaxiMassas.Repositories.Interfaces;
using MaxiMassas.Services.Interfaces;

namespace MaxiMassas.Services;

public class EstoqueService : IEstoqueService
{
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly IProdutoRepository _produtoRepository;
    private const int LimiteAlertaBaixo = 3;

    public EstoqueService(IEstoqueRepository estoqueRepository, IProdutoRepository produtoRepository)
    {
        _estoqueRepository = estoqueRepository;
        _produtoRepository = produtoRepository;
    }

    public async Task<IEnumerable<EstoqueResponseDto>> GetAllAsync()
    {
        var estoques = await _estoqueRepository.GetAllAsync();
        return estoques.Select(MapToDto);
    }

    public async Task<EstoqueResponseDto> GetByProdutoIdAsync(int produtoId)
    {
        var estoque = await _estoqueRepository.GetByProdutoIdAsync(produtoId)
            ?? throw new KeyNotFoundException($"Estoque para o produto ID {produtoId} não encontrado.");
        return MapToDto(estoque);
    }

    public async Task<EstoqueResponseDto> ReporAsync(EstoqueReposicaoDto dto)
    {
        if (dto.Quantidade <= 0)
            throw new InvalidOperationException("A quantidade de reposição deve ser maior que zero.");

        if (!await _produtoRepository.ExistsAsync(dto.ProdutoId))
            throw new KeyNotFoundException($"Produto com ID {dto.ProdutoId} não encontrado.");

        var estoque = await _estoqueRepository.GetByProdutoIdAsync(dto.ProdutoId);

        if (estoque == null)
        {
            var novoEstoque = new Entities.Estoque
            {
                ProdutoId = dto.ProdutoId,
                Quantidade = dto.Quantidade
            };
            var criado = await _estoqueRepository.CreateAsync(novoEstoque);
            return MapToDto(criado);
        }

        estoque.Quantidade += dto.Quantidade;
        var atualizado = await _estoqueRepository.UpdateAsync(estoque);
        return MapToDto(atualizado);
    }

    public async Task<EstoqueResponseDto> AjustarAsync(EstoqueAjusteDto dto)
    {
        if (dto.NovaQuantidade < 0)
            throw new InvalidOperationException("A quantidade não pode ser negativa.");

        if (!await _produtoRepository.ExistsAsync(dto.ProdutoId))
            throw new KeyNotFoundException($"Produto com ID {dto.ProdutoId} não encontrado.");

        var estoque = await _estoqueRepository.GetByProdutoIdAsync(dto.ProdutoId);

        if (estoque == null)
        {
            var novoEstoque = new Entities.Estoque
            {
                ProdutoId = dto.ProdutoId,
                Quantidade = dto.NovaQuantidade
            };
            var criado = await _estoqueRepository.CreateAsync(novoEstoque);
            return MapToDto(criado);
        }

        estoque.Quantidade = dto.NovaQuantidade;
        var atualizado = await _estoqueRepository.UpdateAsync(estoque);
        return MapToDto(atualizado);
    }

    public async Task<IEnumerable<EstoqueResponseDto>> GetAlertasBaixoAsync()
    {
        var estoques = await _estoqueRepository.GetEstoqueBaixoAsync(LimiteAlertaBaixo);
        return estoques.Select(MapToDto);
    }

    private static EstoqueResponseDto MapToDto(Entities.Estoque e) => new()
    {
        Id = e.Id,
        ProdutoId = e.ProdutoId,
        NomeProduto = e.Produto?.Nome ?? string.Empty,
        Quantidade = e.Quantidade,
        AtualizadoEm = e.AtualizadoEm,
        AlertaBaixo = e.Quantidade <= LimiteAlertaBaixo
    };
}
