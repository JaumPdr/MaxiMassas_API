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
    private readonly IConsumoProprioService _consumoService; // Serviço responsável pelas regras de negócio de consumo próprio

    // Injeta o serviço de consumo próprio
    public ConsumoProprioController(IConsumoProprioService consumoService)
    {
        _consumoService = consumoService;
    }

    //Lista todos os registros de consumo próprio
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ConsumoProprioResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var consumos = await _consumoService.GetAllAsync(); // Busca todos os registros de consumo próprio
        return Ok(consumos);
    }

    //Busca um registro de consumo próprio pelo ID
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ConsumoProprioResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var consumo = await _consumoService.GetByIdAsync(id); // Busca consumo próprio pelo ID
            return Ok(consumo);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message }); // Retorna erro caso registro não exista
        }
    }

    //Lista todos os consumos de um produto específico
    [HttpGet("produto/{produtoId:int}")]
    [ProducesResponseType(typeof(IEnumerable<ConsumoProprioResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByProduto(int produtoId)
    {
        try
        {
            var consumos = await _consumoService.GetByProdutoIdAsync(produtoId); // Busca consumos relacionados ao produto
            return Ok(consumos);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message }); // Retorna erro caso produto não exista
        }
    }

    //Registra um consumo próprio e desconta do estoque automaticamente
    [HttpPost]
    [ProducesResponseType(typeof(ConsumoProprioResponseDto), StatusCodes.Status201Created)] // Retorno esperado em caso de sucesso
    [ProducesResponseType(StatusCodes.Status400BadRequest)] // Retorno esperado em caso de erro de validação
    [ProducesResponseType(StatusCodes.Status404NotFound)] // Retorno esperado caso produto não exista
    public async Task<IActionResult> Create([FromBody] ConsumoProprioCreateDto dto)
    {
        try
        {
            var consumo = await _consumoService.CreateAsync(dto); // Registra novo consumo próprio
            return CreatedAtAction(nameof(GetById), new { id = consumo.Id }, consumo); // Retorna registro criado
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

    //Remove um registro de consumo próprio
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)] // Retorno esperado em caso de sucesso
    [ProducesResponseType(StatusCodes.Status404NotFound)] // Retorno esperado caso registro não exista
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _consumoService.DeleteAsync(id); // Remove registro de consumo próprio
            return NoContent(); // Retorna sucesso sem conteúdo
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message }); // Retorna erro caso registro não exista
        }
    }
}
