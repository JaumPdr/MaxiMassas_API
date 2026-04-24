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
    private readonly IRelatorioService _relatorioService; // Serviço responsável pelas regras de negócio dos relatórios

    // Injeta o serviço de relatórios
    public RelatoriosController(IRelatorioService relatorioService)
    {
        _relatorioService = relatorioService;
    }

    // Retorna o relatório financeiro completo
    // Períodos: diario | semanal | mensal. Ou informe DataInicio e DataFim para período customizado.
    [HttpGet("financeiro")]
    [ProducesResponseType(typeof(RelatorioFinanceiroDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFinanceiro([FromQuery] RelatorioFiltroDto filtro)
    {
        var relatorio = await _relatorioService.GetRelatorioFinanceiroAsync(filtro); // Gera relatório financeiro com base no filtro informado
        return Ok(relatorio); // Retorna relatório financeiro
    }

    // Retorna o relatório financeiro do dia atual.
    [HttpGet("financeiro/diario")]
    [ProducesResponseType(typeof(RelatorioFinanceiroDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFinanceiroDiario()
    {
        var relatorio = await _relatorioService.GetRelatorioFinanceiroAsync( 
            new RelatorioFiltroDto { Periodo = "diario" }); // Gera relatório financeiro diário
        return Ok(relatorio); // Retorna relatório diário
    }

    // Retorna o relatório financeiro da semana atual.
    [HttpGet("financeiro/semanal")]
    [ProducesResponseType(typeof(RelatorioFinanceiroDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFinanceiroSemanal()
    {
        var relatorio = await _relatorioService.GetRelatorioFinanceiroAsync(
            new RelatorioFiltroDto { Periodo = "semanal" }); // Gera relatório financeiro semanal
        return Ok(relatorio); // Retorna relatório semanal
    }

    // Retorna o relatório financeiro do mês atual.
    [HttpGet("financeiro/mensal")]
    [ProducesResponseType(typeof(RelatorioFinanceiroDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFinanceiroMensal()
    {
        var relatorio = await _relatorioService.GetRelatorioFinanceiroAsync(
            new RelatorioFiltroDto { Periodo = "mensal" }); // Gera relatório financeiro mensal
        return Ok(relatorio); // Retorna relatório mensal
    }

    // Retorna o ranking dos clientes que mais gastaram no período.
    [HttpGet("top-clientes")]
    [ProducesResponseType(typeof(List<TopClienteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopClientes([FromQuery] RelatorioFiltroDto filtro, [FromQuery] int top = 5)
    {
        var ranking = await _relatorioService.GetTopClientesAsync(filtro, top); // Busca ranking dos clientes com maior faturamento
        return Ok(ranking); // Retorna ranking de clientes
    }

    // Retorna os produtos mais vendidos por quantidade no período.
    [HttpGet("produtos-mais-vendidos")]
    [ProducesResponseType(typeof(List<ProdutoMaisVendidoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProdutosMaisVendidos([FromQuery] RelatorioFiltroDto filtro, [FromQuery] int top = 10)
    {
        var ranking = await _relatorioService.GetProdutosMaisVendidosAsync(filtro, top); // Busca ranking dos produtos mais vendidos
        return Ok(ranking); // Retorna ranking de produtos
    }

    // Retorna o relatório completo: financeiro + top clientes + produtos mais vendidos.
    [HttpGet("completo")]
    [ProducesResponseType(typeof(RelatorioCompletoDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCompleto([FromQuery] RelatorioFiltroDto filtro)
    {
        var relatorio = await _relatorioService.GetRelatorioCompletoAsync(filtro); // Gera relatório completo com dados financeiros e rankings
        return Ok(relatorio); // Retorna relatório completo
    }
}
