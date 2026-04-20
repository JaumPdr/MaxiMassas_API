using MaxiMassas.DTOs.Relatorio;
using MaxiMassas.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMassas.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class RelatoriosController : ControllerBase
{
    private readonly IRelatorioService _relatorioService;

    public RelatoriosController(IRelatorioService relatorioService)
    {
        _relatorioService = relatorioService;
    }

    /// <summary>
    /// Retorna o relatório financeiro completo: faturamento, custo, lucro e % de lucro.
    /// Períodos: diario | semanal | mensal. Ou informe DataInicio e DataFim para período customizado.
    /// </summary>
    [HttpGet("financeiro")]
    [ProducesResponseType(typeof(RelatorioFinanceiroDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFinanceiro([FromQuery] RelatorioFiltroDto filtro)
    {
        var relatorio = await _relatorioService.GetRelatorioFinanceiroAsync(filtro);
        return Ok(relatorio);
    }

    /// <summary>
    /// Retorna o relatório financeiro do dia atual.
    /// </summary>
    [HttpGet("financeiro/diario")]
    [ProducesResponseType(typeof(RelatorioFinanceiroDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFinanceiroDiario()
    {
        var relatorio = await _relatorioService.GetRelatorioFinanceiroAsync(
            new RelatorioFiltroDto { Periodo = "diario" });
        return Ok(relatorio);
    }

    /// <summary>
    /// Retorna o relatório financeiro da semana atual.
    /// </summary>
    [HttpGet("financeiro/semanal")]
    [ProducesResponseType(typeof(RelatorioFinanceiroDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFinanceiroSemanal()
    {
        var relatorio = await _relatorioService.GetRelatorioFinanceiroAsync(
            new RelatorioFiltroDto { Periodo = "semanal" });
        return Ok(relatorio);
    }

    /// <summary>
    /// Retorna o relatório financeiro do mês atual.
    /// </summary>
    [HttpGet("financeiro/mensal")]
    [ProducesResponseType(typeof(RelatorioFinanceiroDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFinanceiroMensal()
    {
        var relatorio = await _relatorioService.GetRelatorioFinanceiroAsync(
            new RelatorioFiltroDto { Periodo = "mensal" });
        return Ok(relatorio);
    }

    /// <summary>
    /// Retorna o top 5 clientes que mais gastaram no período.
    /// </summary>
    [HttpGet("top-clientes")]
    [ProducesResponseType(typeof(List<TopClienteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopClientes([FromQuery] RelatorioFiltroDto filtro, [FromQuery] int top = 5)
    {
        var ranking = await _relatorioService.GetTopClientesAsync(filtro, top);
        return Ok(ranking);
    }

    /// <summary>
    /// Retorna os produtos mais vendidos por quantidade no período.
    /// </summary>
    [HttpGet("produtos-mais-vendidos")]
    [ProducesResponseType(typeof(List<ProdutoMaisVendidoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProdutosMaisVendidos([FromQuery] RelatorioFiltroDto filtro, [FromQuery] int top = 10)
    {
        var ranking = await _relatorioService.GetProdutosMaisVendidosAsync(filtro, top);
        return Ok(ranking);
    }

    /// <summary>
    /// Retorna o relatório completo: financeiro + top clientes + produtos mais vendidos.
    /// </summary>
    [HttpGet("completo")]
    [ProducesResponseType(typeof(RelatorioCompletoDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCompleto([FromQuery] RelatorioFiltroDto filtro)
    {
        var relatorio = await _relatorioService.GetRelatorioCompletoAsync(filtro);
        return Ok(relatorio);
    }
}
