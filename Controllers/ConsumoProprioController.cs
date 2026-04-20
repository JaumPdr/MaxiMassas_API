using MaxiMassas.DTOs.ConsumoProprio;
using MaxiMassas.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMassas.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class ConsumoProprioController : ControllerBase
{
    private readonly IConsumoProprioService _consumoService;

    public ConsumoProprioController(IConsumoProprioService consumoService)
    {
        _consumoService = consumoService;
    }

    /// <summary>Lista todos os registros de consumo próprio.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ConsumoProprioResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var consumos = await _consumoService.GetAllAsync();
        return Ok(consumos);
    }

    /// <summary>Busca um registro de consumo próprio pelo ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ConsumoProprioResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var consumo = await _consumoService.GetByIdAsync(id);
            return Ok(consumo);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    /// <summary>Lista todos os consumos de um produto específico.</summary>
    [HttpGet("produto/{produtoId:int}")]
    [ProducesResponseType(typeof(IEnumerable<ConsumoProprioResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByProduto(int produtoId)
    {
        try
        {
            var consumos = await _consumoService.GetByProdutoIdAsync(produtoId);
            return Ok(consumos);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    /// <summary>Registra um consumo próprio e desconta do estoque automaticamente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ConsumoProprioResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] ConsumoProprioCreateDto dto)
    {
        try
        {
            var consumo = await _consumoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = consumo.Id }, consumo);
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

    /// <summary>Remove um registro de consumo próprio.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _consumoService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }
}
