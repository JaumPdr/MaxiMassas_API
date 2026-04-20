using MaxiMassas.Config;
using MaxiMassas.DTOs.Venda;
using MaxiMassas.Entities;
using MaxiMassas.Repositories.Interfaces;
using MaxiMassas.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace MaxiMassas.Services;

public class VendaService : IVendaService
{
    private readonly IVendaRepository _vendaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly FreteConfig _freteConfig;

    // Taxas fixas por forma de pagamento
    private const decimal TaxaDebito = 0.0137m;
    private const decimal TaxaCredito = 0.0315m;
    private const decimal TaxaUaiRangoPlatforma = 0.135m;
    private const decimal TaxaUaiRangoEntrega = 0.10m;

    // Horários permitidos
    private static readonly TimeSpan HorarioRetiradaInicio = new(10, 0, 0);
    private static readonly TimeSpan HorarioRetiradaFim = new(20, 0, 0);
    private static readonly TimeSpan HorarioEntregaInicio = new(18, 0, 0);
    private static readonly TimeSpan HorarioEntregaFim = new(20, 0, 0);

    public VendaService(
        IVendaRepository vendaRepository,
        IClienteRepository clienteRepository,
        IProdutoRepository produtoRepository,
        IEstoqueRepository estoqueRepository,
        IOptions<FreteConfig> freteConfig)
    {
        _vendaRepository = vendaRepository;
        _clienteRepository = clienteRepository;
        _produtoRepository = produtoRepository;
        _estoqueRepository = estoqueRepository;
        _freteConfig = freteConfig.Value;
    }

    public async Task<IEnumerable<VendaResponseDto>> GetAllAsync()
    {
        var vendas = await _vendaRepository.GetAllAsync();
        return vendas.Select(MapToDto);
    }

    public async Task<VendaResponseDto> GetByIdAsync(int id)
    {
        var venda = await _vendaRepository.GetByIdWithItensAsync(id)
            ?? throw new KeyNotFoundException($"Venda com ID {id} não encontrada.");
        return MapToDto(venda);
    }

    public async Task<VendaResponseDto> CreateAsync(VendaCreateDto dto)
    {
        // Validar cliente
        if (!await _clienteRepository.ExistsAsync(dto.ClienteId))
            throw new KeyNotFoundException($"Cliente com ID {dto.ClienteId} não encontrado.");

        // Validar horário de entrega
        if (dto.HorarioEntrega.HasValue)
            ValidarHorario(dto.FormaEntrega, dto.HorarioEntrega.Value);

        // Validar itens e estoque
        foreach (var item in dto.Itens)
        {
            var produto = await _produtoRepository.GetByIdAsync(item.ProdutoId)
                ?? throw new KeyNotFoundException($"Produto com ID {item.ProdutoId} não encontrado.");

            var estoque = await _estoqueRepository.GetByProdutoIdAsync(item.ProdutoId);
            if (estoque == null || estoque.Quantidade < item.Quantidade)
                throw new InvalidOperationException(
                    $"Estoque insuficiente para o produto '{produto.Nome}'. " +
                    $"Disponível: {estoque?.Quantidade ?? 0}, Solicitado: {item.Quantidade}.");
        }

        // Calcular taxa
        var taxa = CalcularTaxa(dto.FormaPagamento, dto.PlataformaOrigem);

        // Calcular frete
        var subtotal = dto.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);
        var desconto = dto.DescontoValor > 0
            ? dto.DescontoValor
            : subtotal * (dto.DescontoPercentual / 100);
        var valorComDesconto = subtotal - desconto;
        var frete = CalcularFrete(dto.PlataformaOrigem, dto.FormaEntrega, valorComDesconto);

        var venda = new Venda
        {
            ClienteId = dto.ClienteId,
            DataVenda = DateTime.UtcNow,
            FormaPagamento = dto.FormaPagamento,
            PlataformaOrigem = dto.PlataformaOrigem,
            DescontoValor = dto.DescontoValor,
            DescontoPercentual = dto.DescontoPercentual,
            StatusPagamento = dto.StatusPagamento,
            FormaEntrega = dto.FormaEntrega,
            DataEntrega = dto.DataEntrega,
            HorarioEntrega = dto.HorarioEntrega,
            TaxaAplicada = taxa,
            ValorFrete = frete,
            Observacao = dto.Observacao,
            Itens = dto.Itens.Select(i => new VendaItem
            {
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario
            }).ToList()
        };

        var criada = await _vendaRepository.CreateAsync(venda);

        // Atualizar estoque automaticamente
        foreach (var item in dto.Itens)
        {
            var estoque = await _estoqueRepository.GetByProdutoIdAsync(item.ProdutoId);
            if (estoque != null)
            {
                estoque.Quantidade -= item.Quantidade;
                await _estoqueRepository.UpdateAsync(estoque);
            }
        }

        var vendaCompleta = await _vendaRepository.GetByIdWithItensAsync(criada.Id);
        return MapToDto(vendaCompleta!);
    }

    public async Task<VendaResponseDto> UpdateAsync(int id, VendaUpdateDto dto)
    {
        var venda = await _vendaRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Venda com ID {id} não encontrada.");

        venda.FormaPagamento = dto.FormaPagamento;
        venda.StatusPagamento = dto.StatusPagamento;
        venda.DescontoValor = dto.DescontoValor;
        venda.DescontoPercentual = dto.DescontoPercentual;
        venda.DataEntrega = dto.DataEntrega;
        venda.HorarioEntrega = dto.HorarioEntrega;
        venda.Observacao = dto.Observacao;
        venda.TaxaAplicada = CalcularTaxa(dto.FormaPagamento, venda.PlataformaOrigem);

        await _vendaRepository.UpdateAsync(venda);

        var vendaCompleta = await _vendaRepository.GetByIdWithItensAsync(id);
        return MapToDto(vendaCompleta!);
    }

    public async Task DeleteAsync(int id)
    {
        var venda = await _vendaRepository.GetByIdWithItensAsync(id)
            ?? throw new KeyNotFoundException($"Venda com ID {id} não encontrada.");

        // Devolver ao estoque
        foreach (var item in venda.Itens)
        {
            var estoque = await _estoqueRepository.GetByProdutoIdAsync(item.ProdutoId);
            if (estoque != null)
            {
                estoque.Quantidade += item.Quantidade;
                await _estoqueRepository.UpdateAsync(estoque);
            }
        }

        await _vendaRepository.DeleteAsync(venda);
    }

    private static decimal CalcularTaxa(FormaPagamento pagamento, PlataformaOrigem plataforma)
    {
        if (plataforma == PlataformaOrigem.UaiRango)
            return TaxaUaiRangoPlatforma; // UAI Rango pago pela plataforma

        return pagamento switch
        {
            FormaPagamento.Debito => TaxaDebito,
            FormaPagamento.Credito => TaxaCredito,
            _ => 0m
        };
    }

    private decimal CalcularFrete(PlataformaOrigem plataforma, FormaEntrega forma, decimal valorPedido)
    {
        if (forma == FormaEntrega.Retirada)
            return 0m;

        if (plataforma == PlataformaOrigem.UaiRango)
            return _freteConfig.ValorFreteUaiRango;

        // Outras plataformas: grátis acima do valor mínimo
        return valorPedido >= _freteConfig.ValorMinimoFreteGratis ? 0m : _freteConfig.ValorFreteUaiRango;
    }

    private static void ValidarHorario(FormaEntrega forma, TimeSpan horario)
    {
        if (forma == FormaEntrega.Retirada)
        {
            if (horario < HorarioRetiradaInicio || horario > HorarioRetiradaFim)
                throw new InvalidOperationException("Horário de retirada permitido: 10h às 20h.");
        }
        else
        {
            if (horario < HorarioEntregaInicio || horario > HorarioEntregaFim)
                throw new InvalidOperationException("Horário de entrega permitido: 18h às 20h.");
        }
    }

    private static VendaResponseDto MapToDto(Venda v)
    {
        var subtotal = v.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);
        var desconto = v.DescontoValor > 0
            ? v.DescontoValor
            : subtotal * (v.DescontoPercentual / 100);
        var comDesconto = subtotal - desconto;
        var comTaxa = comDesconto * (1 + v.TaxaAplicada);
        var total = comTaxa + v.ValorFrete;

        return new VendaResponseDto
        {
            Id = v.Id,
            ClienteId = v.ClienteId,
            NomeCliente = v.Cliente?.Nome ?? string.Empty,
            DataVenda = v.DataVenda,
            FormaPagamento = v.FormaPagamento.ToString(),
            PlataformaOrigem = v.PlataformaOrigem.ToString(),
            DescontoValor = v.DescontoValor,
            DescontoPercentual = v.DescontoPercentual,
            StatusPagamento = v.StatusPagamento.ToString(),
            FormaEntrega = v.FormaEntrega.ToString(),
            DataEntrega = v.DataEntrega,
            HorarioEntrega = v.HorarioEntrega,
            TaxaAplicada = v.TaxaAplicada,
            ValorFrete = v.ValorFrete,
            SubTotal = subtotal,
            ValorDesconto = desconto,
            ValorComTaxa = comTaxa,
            ValorTotal = total,
            Observacao = v.Observacao,
            Itens = v.Itens.Select(i => new VendaItemResponseDto
            {
                Id = i.Id,
                ProdutoId = i.ProdutoId,
                NomeProduto = i.Produto?.Nome ?? string.Empty,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario,
                SubTotal = i.Quantidade * i.PrecoUnitario
            }).ToList()
        };
    }
}
