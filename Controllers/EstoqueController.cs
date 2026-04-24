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
    private readonly IEstoqueService _estoqueService; // Serviço responsável pelas regras de negócio do estoque

    // Injeta o serviço de estoque
    public EstoqueController(IEstoqueService estoqueService)
    {
        _estoqueService = estoqueService;
    }

    //Lista o estoque de todos os produtos com alerta quando quantidade <= 3
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EstoqueResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var estoques = await _estoqueService.GetAllAsync(); // Busca todos os registros de estoque
        return Ok(estoques);
    }

    //Retorna o estoque de um produto específico
    [HttpGet("produto/{produtoId:int}")]
    [ProducesResponseType(typeof(EstoqueResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByProduto(int produtoId)
    {
        try
        {
            var estoque = await _estoqueService.GetByProdutoIdAsync(produtoId); // Busca estoque do produto informado
            return Ok(estoque);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message }); // Retorna estoque encontrado
        }
    }

    //Retorna todos os produtos com estoque baixo (quantidade <= 3). Alerta de reposição
    [HttpGet("alertas")] // Endpoint: GET api/estoque/alertas
    [ProducesResponseType(typeof(IEnumerable<EstoqueResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAlertas()
    {
        var alertas = await _estoqueService.GetAlertasBaixoAsync(); // Busca produtos com estoque baixo
        // Retorna quantidade de alertas e lista de produtos
        return Ok(new
        {
            totalAlertas = alertas.Count(),
            produtos = alertas
        });
    }

    //Repõe estoque de um produto (soma à quantidade atual)
    [HttpPost("repor")]
    [ProducesResponseType(typeof(EstoqueResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Repor([FromBody] EstoqueReposicaoDto dto)
    {
        try
        {
            var estoque = await _estoqueService.ReporAsync(dto); // Adiciona quantidade ao estoque atual
            return Ok(estoque);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message }); // Retorna erro caso produto não exista
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message }); // Retorna erro de regra de negócio
        }
    }

    //Ajusta o estoque de um produto para um valor exato (correção manual)
    [HttpPut("ajustar")]
    [ProducesResponseType(typeof(EstoqueResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Ajustar([FromBody] EstoqueAjusteDto dto)
    {
        try
        {
            var estoque = await _estoqueService.AjustarAsync(dto); // Define uma quantidade exata para o estoque
            return Ok(estoque); // Retorna estoque atualizado
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message }); // Retorna erro caso produto não exista
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message }); // Retorna erro de regra de negócio
        }
    }
}
