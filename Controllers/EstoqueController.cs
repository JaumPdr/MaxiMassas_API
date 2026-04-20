using MaxiMassas.DTOs.Estoque;
using MaxiMassas.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMassas.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class EstoqueController : ControllerBase
{
    private readonly IEstoqueService _estoqueService;

    public EstoqueController(IEstoqueService estoqueService)
    {
        _estoqueService = estoqueService;
    }

    /// <summary>Lista o estoque de todos os produtos com alerta quando quantidade &lt;= 3.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EstoqueResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var estoques = await _estoqueService.GetAllAsync();
        return Ok(estoques);
    }

    /// <summary>Retorna o estoque de um produto específico.</summary>
    [HttpGet("produto/{produtoId:int}")]
    [ProducesResponseType(typeof(EstoqueResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByProduto(int produtoId)
    {
        try
        {
            var estoque = await _estoqueService.GetByProdutoIdAsync(produtoId);
            return Ok(estoque);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    /// <summary>Retorna todos os produtos com estoque baixo (quantidade &lt;= 3). Alerta de reposição.</summary>
    [HttpGet("alertas")]
    [ProducesResponseType(typeof(IEnumerable<EstoqueResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAlertas()
    {
        var alertas = await _estoqueService.GetAlertasBaixoAsync();
        return Ok(new
        {
            totalAlertas = alertas.Count(),
            produtos = alertas
        });
    }

    /// <summary>Repõe estoque de um produto (soma à quantidade atual).</summary>
    [HttpPost("repor")]
    [ProducesResponseType(typeof(EstoqueResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Repor([FromBody] EstoqueReposicaoDto dto)
    {
        try
        {
            var estoque = await _estoqueService.ReporAsync(dto);
            return Ok(estoque);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>Ajusta o estoque de um produto para um valor exato (correção manual).</summary>
    [HttpPut("ajustar")]
    [ProducesResponseType(typeof(EstoqueResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Ajustar([FromBody] EstoqueAjusteDto dto)
    {
        try
        {
            var estoque = await _estoqueService.AjustarAsync(dto);
            return Ok(estoque);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
