using MaxiMassas.DTOs.ConsumoProprio;
using MaxiMassas.Entities;
using MaxiMassas.Repositories.Interfaces;
using MaxiMassas.Services.Interfaces;

namespace MaxiMassas.Services;

public class ConsumoProprioService : IConsumoProprioService
{
    private readonly IConsumoProprioRepository _consumoRepository;
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly IProdutoRepository _produtoRepository;

    public ConsumoProprioService(
        IConsumoProprioRepository consumoRepository,
        IEstoqueRepository estoqueRepository,
        IProdutoRepository produtoRepository)
    {
        _consumoRepository = consumoRepository;
        _estoqueRepository = estoqueRepository;
        _produtoRepository = produtoRepository;
    }

    public async Task<IEnumerable<ConsumoProprioResponseDto>> GetAllAsync()
    {
        var consumos = await _consumoRepository.GetAllAsync();
        return consumos.Select(MapToDto);
    }

    public async Task<ConsumoProprioResponseDto> GetByIdAsync(int id)
    {
        var consumo = await _consumoRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Consumo próprio com ID {id} não encontrado.");
        return MapToDto(consumo);
    }

    public async Task<IEnumerable<ConsumoProprioResponseDto>> GetByProdutoIdAsync(int produtoId)
    {
        if (!await _produtoRepository.ExistsAsync(produtoId))
            throw new KeyNotFoundException($"Produto com ID {produtoId} não encontrado.");

        var consumos = await _consumoRepository.GetByProdutoIdAsync(produtoId);
        return consumos.Select(MapToDto);
    }

    public async Task<ConsumoProprioResponseDto> CreateAsync(ConsumoProprioCreateDto dto)
    {
        if (dto.Quantidade <= 0)
            throw new InvalidOperationException("A quantidade deve ser maior que zero.");

        var produto = await _produtoRepository.GetByIdAsync(dto.ProdutoId)
            ?? throw new KeyNotFoundException($"Produto com ID {dto.ProdutoId} não encontrado.");

        var estoque = await _estoqueRepository.GetByProdutoIdAsync(dto.ProdutoId);
        if (estoque == null || estoque.Quantidade < dto.Quantidade)
            throw new InvalidOperationException(
                $"Estoque insuficiente para o produto '{produto.Nome}'. " +
                $"Disponível: {estoque?.Quantidade ?? 0}, Solicitado: {dto.Quantidade}.");

        var consumo = new ConsumoProprio
        {
            ProdutoId = dto.ProdutoId,
            Quantidade = dto.Quantidade,
            Data = dto.Data ?? DateTime.UtcNow,
            Motivo = dto.Motivo?.Trim()
        };

        var criado = await _consumoRepository.CreateAsync(consumo);

        // Reduzir estoque automaticamente
        estoque.Quantidade -= dto.Quantidade;
        await _estoqueRepository.UpdateAsync(estoque);

        return MapToDto(criado);
    }

    public async Task DeleteAsync(int id)
    {
        var consumo = await _consumoRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Consumo próprio com ID {id} não encontrado.");
        await _consumoRepository.DeleteAsync(consumo);
    }

    private static ConsumoProprioResponseDto MapToDto(ConsumoProprio c) => new()
    {
        Id = c.Id,
        ProdutoId = c.ProdutoId,
        NomeProduto = c.Produto?.Nome ?? string.Empty,
        Quantidade = c.Quantidade,
        Data = c.Data,
        Motivo = c.Motivo
    };
}
