using MaxiMassas.DTOs.Produto;
using MaxiMassas.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMassas.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    /// <summary>Lista todos os produtos ativos com quantidade em estoque.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProdutoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var produtos = await _produtoService.GetAllAsync();
        return Ok(produtos);
    }

    /// <summary>Busca um produto pelo ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var produto = await _produtoService.GetByIdAsync(id);
            return Ok(produto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    /// <summary>Retorna o histórico de alterações de preço de um produto.</summary>
    [HttpGet("{id:int}/historico-preco")]
    [ProducesResponseType(typeof(IEnumerable<HistoricoPrecoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHistoricoPreco(int id)
    {
        try
        {
            var historico = await _produtoService.GetHistoricoPrecoAsync(id);
            return Ok(historico);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    /// <summary>Cadastra um novo produto. Cria estoque zerado automaticamente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ProdutoCreateDto dto)
    {
        try
        {
            var produto = await _produtoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>Atualiza os dados de um produto. Grava histórico ao alterar preço ou custo.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] ProdutoUpdateDto dto)
    {
        try
        {
            var produto = await _produtoService.UpdateAsync(id, dto);
            return Ok(produto);
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

    /// <summary>Desativa (soft-delete) um produto.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _produtoService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }
}
