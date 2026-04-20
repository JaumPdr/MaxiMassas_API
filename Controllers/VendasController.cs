using MaxiMassas.DTOs.Venda;
using MaxiMassas.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMassas.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class VendasController : ControllerBase
{
    private readonly IVendaService _vendaService;

    public VendasController(IVendaService vendaService)
    {
        _vendaService = vendaService;
    }

    /// <summary>
    /// Lista todas as vendas com itens, cliente e valores calculados.
    /// Taxas aplicadas: Débito 1,37% | Crédito 3,15% | UAI Rango plataforma 13,5% | UAI Rango entrega 10%.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VendaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var vendas = await _vendaService.GetAllAsync();
        return Ok(vendas);
    }

    /// <summary>Busca uma venda completa pelo ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VendaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var venda = await _vendaService.GetByIdAsync(id);
            return Ok(venda);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Registra uma nova venda com múltiplos produtos.
    /// Aplica desconto (valor ou percentual), calcula taxa e frete automaticamente.
    /// Horário retirada: 10h-20h | Horário entrega: 18h-20h.
    /// Desconta estoque automaticamente.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(VendaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] VendaCreateDto dto)
    {
        try
        {
            var venda = await _vendaService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = venda.Id }, venda);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>Atualiza forma de pagamento, status, desconto e dados de entrega de uma venda.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(VendaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] VendaUpdateDto dto)
    {
        try
        {
            var venda = await _vendaService.UpdateAsync(id, dto);
            return Ok(venda);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>Remove uma venda e devolve as quantidades ao estoque.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _vendaService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }
}
