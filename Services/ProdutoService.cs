using MaxiMassas.DTOs.Produto;
using MaxiMassas.Entities;
using MaxiMassas.Repositories.Interfaces;
using MaxiMassas.Services.Interfaces;

namespace MaxiMassas.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IEstoqueRepository _estoqueRepository;
    private const int LimiteAlertaBaixo = 3;

    public ProdutoService(IProdutoRepository produtoRepository, IEstoqueRepository estoqueRepository)
    {
        _produtoRepository = produtoRepository;
        _estoqueRepository = estoqueRepository;
    }

    public async Task<IEnumerable<ProdutoResponseDto>> GetAllAsync()
    {
        var produtos = await _produtoRepository.GetAllAsync();
        return produtos.Select(MapToDto);
    }

    public async Task<ProdutoResponseDto> GetByIdAsync(int id)
    {
        var produto = await _produtoRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");
        return MapToDto(produto);
    }

    public async Task<ProdutoResponseDto> CreateAsync(ProdutoCreateDto dto)
    {
        var produto = new Produto
        {
            Nome = dto.Nome.Trim(),
            Peso = dto.Peso,
            PrecoVenda = dto.PrecoVenda,
            Custo = dto.Custo,
            TipoVariacao = dto.TipoVariacao?.Trim()
        };

        var criado = await _produtoRepository.CreateAsync(produto);

        // Criar estoque zerado automaticamente
        var estoque = new Estoque
        {
            ProdutoId = criado.Id,
            Quantidade = 0
        };
        await _estoqueRepository.CreateAsync(estoque);

        return MapToDto(criado);
    }

    public async Task<ProdutoResponseDto> UpdateAsync(int id, ProdutoUpdateDto dto)
    {
        var produto = await _produtoRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");

        bool precoAlterado = produto.PrecoVenda != dto.PrecoVenda || produto.Custo != dto.Custo;

        if (precoAlterado)
        {
            var historico = new HistoricoPreco
            {
                ProdutoId = produto.Id,
                PrecoVendaAnterior = produto.PrecoVenda,
                CustoAnterior = produto.Custo,
                PrecoVendaNovo = dto.PrecoVenda,
                CustoNovo = dto.Custo,
                Motivo = dto.MotivoAlteracao
            };
            await _produtoRepository.AddHistoricoPrecoAsync(historico);
        }

        produto.Nome = dto.Nome.Trim();
        produto.Peso = dto.Peso;
        produto.PrecoVenda = dto.PrecoVenda;
        produto.Custo = dto.Custo;
        produto.TipoVariacao = dto.TipoVariacao?.Trim();

        var atualizado = await _produtoRepository.UpdateAsync(produto);
        return MapToDto(atualizado);
    }

    public async Task DeleteAsync(int id)
    {
        var produto = await _produtoRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");
        await _produtoRepository.DeleteAsync(produto);
    }

    public async Task<IEnumerable<HistoricoPrecoDto>> GetHistoricoPrecoAsync(int produtoId)
    {
        if (!await _produtoRepository.ExistsAsync(produtoId))
            throw new KeyNotFoundException($"Produto com ID {produtoId} não encontrado.");

        var historico = await _produtoRepository.GetHistoricoPrecoAsync(produtoId);
        return historico.Select(h => new HistoricoPrecoDto
        {
            Id = h.Id,
            ProdutoId = h.ProdutoId,
            NomeProduto = h.Produto.Nome,
            PrecoVendaAnterior = h.PrecoVendaAnterior,
            CustoAnterior = h.CustoAnterior,
            PrecoVendaNovo = h.PrecoVendaNovo,
            CustoNovo = h.CustoNovo,
            AlteradoEm = h.AlteradoEm,
            Motivo = h.Motivo
        });
    }

    private static ProdutoResponseDto MapToDto(Produto p) => new()
    {
        Id = p.Id,
        Nome = p.Nome,
        Peso = p.Peso,
        PrecoVenda = p.PrecoVenda,
        Custo = p.Custo,
        TipoVariacao = p.TipoVariacao,
        Ativo = p.Ativo,
        CriadoEm = p.CriadoEm,
        QuantidadeEstoque = p.Estoque?.Quantidade ?? 0,
        AlertaEstoqueBaixo = (p.Estoque?.Quantidade ?? 0) <= LimiteAlertaBaixo
    };
}
