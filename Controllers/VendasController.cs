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
    // Serviço responsável pelas regras de negócio das vendas
    private readonly IVendaService _vendaService;

    // Injeta o serviço de vendas
    public VendasController(IVendaService vendaService)
    {
        _vendaService = vendaService;
    }

    // Lista todas as vendas com cliente, itens e valores calculados.
    // Endpoint: GET api/vendas
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VendaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        // Busca todas as vendas cadastradas
        var vendas = await _vendaService.GetAllAsync();

        // Retorna a lista de vendas
        return Ok(vendas);
    }

    // Busca uma venda completa pelo ID.
    // Endpoint: GET api/vendas/{id}
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VendaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            // Busca venda pelo ID
            var venda = await _vendaService.GetByIdAsync(id);

            // Retorna a venda encontrada
            return Ok(venda);
        }
        catch (KeyNotFoundException ex)
        {
            // Retorna erro caso a venda não exista
            return NotFound(new { mensagem = ex.Message });
        }
    }

    // Registra uma nova venda com cálculos automáticos.
    // Endpoint: POST api/vendas
    [HttpPost]
    [ProducesResponseType(typeof(VendaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] VendaCreateDto dto)
    {
        try
        {
            // Cria uma nova venda
            var venda = await _vendaService.CreateAsync(dto);

            // Retorna a venda criada
            return CreatedAtAction(nameof(GetById), new { id = venda.Id }, venda);
        }
        catch (KeyNotFoundException ex)
        {
            // Retorna erro caso cliente ou produto não exista
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // Retorna erro de regra de negócio
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            // Retorna erro genérico
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // Atualiza dados da venda como pagamento, status e entrega.
    // Endpoint: PUT api/vendas/{id}
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(VendaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] VendaUpdateDto dto)
    {
        try
        {
            // Atualiza os dados da venda
            var venda = await _vendaService.UpdateAsync(id, dto);

            // Retorna a venda atualizada
            return Ok(venda);
        }
        catch (KeyNotFoundException ex)
        {
            // Retorna erro caso a venda não exista
            return NotFound(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            // Retorna erro de validação ou erro genérico
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // Remove uma venda e devolve os itens ao estoque.
    // Endpoint: DELETE api/vendas/{id}
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            // Remove a venda e restaura o estoque
            await _vendaService.DeleteAsync(id);

            // Retorna sucesso sem conteúdo
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            // Retorna erro caso a venda não exista
            return NotFound(new { mensagem = ex.Message });
        }
    }
}