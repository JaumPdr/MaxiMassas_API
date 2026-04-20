using MaxiMassas.DTOs.Relatorio;
using MaxiMassas.Entities;
using MaxiMassas.Repositories.Interfaces;
using MaxiMassas.Services.Interfaces;

namespace MaxiMassas.Services;

public class RelatorioService : IRelatorioService
{
    private readonly IVendaRepository _vendaRepository;
    private readonly IClienteRepository _clienteRepository;

    public RelatorioService(IVendaRepository vendaRepository, IClienteRepository clienteRepository)
    {
        _vendaRepository = vendaRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<RelatorioFinanceiroDto> GetRelatorioFinanceiroAsync(RelatorioFiltroDto filtro)
    {
        var (inicio, fim) = ResolverPeriodo(filtro);
        var vendas = (await _vendaRepository.GetByPeriodoAsync(inicio, fim)).ToList();

        var faturamento = vendas.Sum(v => CalcularValorTotal(v));
        var custoTotal = vendas.Sum(v => v.Itens.Sum(i => i.Quantidade * (i.Produto?.Custo ?? 0)));
        var lucro = faturamento - custoTotal;
        var percentualLucro = faturamento > 0 ? (lucro / faturamento) * 100 : 0;

        return new RelatorioFinanceiroDto
        {
            Faturamento = Math.Round(faturamento, 2),
            CustoTotal = Math.Round(custoTotal, 2),
            Lucro = Math.Round(lucro, 2),
            PercentualLucro = Math.Round(percentualLucro, 2),
            TotalVendas = vendas.Count,
            Periodo = filtro.Periodo,
            DataInicio = inicio,
            DataFim = fim
        };
    }

    public async Task<List<TopClienteDto>> GetTopClientesAsync(RelatorioFiltroDto filtro, int top = 5)
    {
        var (inicio, fim) = ResolverPeriodo(filtro);
        var vendas = (await _vendaRepository.GetByPeriodoAsync(inicio, fim)).ToList();

        var ranking = vendas
            .GroupBy(v => new { v.ClienteId, v.Cliente.Nome, v.Cliente.Telefone })
            .Select((g, idx) => new TopClienteDto
            {
                ClienteId = g.Key.ClienteId,
                NomeCliente = g.Key.Nome,
                Telefone = g.Key.Telefone,
                TotalGasto = Math.Round(g.Sum(v => CalcularValorTotal(v)), 2),
                TotalCompras = g.Count()
            })
            .OrderByDescending(c => c.TotalGasto)
            .Take(top)
            .ToList();

        for (int i = 0; i < ranking.Count; i++)
            ranking[i].Posicao = i + 1;

        return ranking;
    }

    public async Task<List<ProdutoMaisVendidoDto>> GetProdutosMaisVendidosAsync(RelatorioFiltroDto filtro, int top = 10)
    {
        var (inicio, fim) = ResolverPeriodo(filtro);
        var vendas = (await _vendaRepository.GetByPeriodoAsync(inicio, fim)).ToList();

        var ranking = vendas
            .SelectMany(v => v.Itens)
            .GroupBy(i => new { i.ProdutoId, Nome = i.Produto?.Nome ?? string.Empty })
            .Select((g, idx) => new ProdutoMaisVendidoDto
            {
                ProdutoId = g.Key.ProdutoId,
                NomeProduto = g.Key.Nome,
                QuantidadeVendida = g.Sum(i => i.Quantidade),
                ReceitaGerada = Math.Round(g.Sum(i => i.Quantidade * i.PrecoUnitario), 2)
            })
            .OrderByDescending(p => p.QuantidadeVendida)
            .Take(top)
            .ToList();

        for (int i = 0; i < ranking.Count; i++)
            ranking[i].Posicao = i + 1;

        return ranking;
    }

    public async Task<RelatorioCompletoDto> GetRelatorioCompletoAsync(RelatorioFiltroDto filtro)
    {
        var financeiro = await GetRelatorioFinanceiroAsync(filtro);
        var topClientes = await GetTopClientesAsync(filtro, 5);
        var produtosMaisVendidos = await GetProdutosMaisVendidosAsync(filtro, 10);

        return new RelatorioCompletoDto
        {
            Financeiro = financeiro,
            TopClientes = topClientes,
            ProdutosMaisVendidos = produtosMaisVendidos
        };
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static (DateTime inicio, DateTime fim) ResolverPeriodo(RelatorioFiltroDto filtro)
    {
        if (filtro.DataInicio.HasValue && filtro.DataFim.HasValue)
            return (filtro.DataInicio.Value, filtro.DataFim.Value.Date.AddDays(1).AddTicks(-1));

        var hoje = DateTime.UtcNow.Date;

        return filtro.Periodo?.ToLower() switch
        {
            "diario" => (hoje, hoje.AddDays(1).AddTicks(-1)),
            "semanal" => (hoje.AddDays(-(int)hoje.DayOfWeek), hoje.AddDays(7 - (int)hoje.DayOfWeek).AddTicks(-1)),
            _ => (new DateTime(hoje.Year, hoje.Month, 1), new DateTime(hoje.Year, hoje.Month, 1).AddMonths(1).AddTicks(-1))
        };
    }

    private static decimal CalcularValorTotal(Venda v)
    {
        var subtotal = v.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);
        var desconto = v.DescontoValor > 0
            ? v.DescontoValor
            : subtotal * (v.DescontoPercentual / 100);
        var comDesconto = subtotal - desconto;
        return (comDesconto * (1 + v.TaxaAplicada)) + v.ValorFrete;
    }
}
