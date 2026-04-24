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
    private readonly IProdutoService _produtoService; // Serviço responsável pelas regras de negócio dos produtos

    // Injeta o serviço de produtos
    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    //Lista todos os produtos ativos com quantidade em estoque
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProdutoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var produtos = await _produtoService.GetAllAsync(); // Busca todos os produtos ativos
        return Ok(produtos); // Retorna lista de produtos
    }

    //Busca um produto pelo ID
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var produto = await _produtoService.GetByIdAsync(id); // Busca produto pelo ID
            return Ok(produto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message }); // Retorna erro caso produto não exista
        }
    }

    //Retorna o histórico de alterações de preço de um produto
    [HttpGet("{id:int}/historico-preco")]
    [ProducesResponseType(typeof(IEnumerable<HistoricoPrecoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHistoricoPreco(int id)
    {
        try
        {
            var historico = await _produtoService.GetHistoricoPrecoAsync(id); // Busca histórico de alterações de preço
            return Ok(historico);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message }); // Retorna erro caso produto não exista
        }
    }

    //Cadastra um novo produto. Cria estoque zerado automaticamente
    [HttpPost]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ProdutoCreateDto dto)
    {
        try
        {
            var produto = await _produtoService.CreateAsync(dto); // Cria novo produto e estoque inicial zerado
            return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto); // Retorna produto criado
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message }); // Retorna erro de validação/regra de negócio
        }
    }

    //Atualiza os dados de um produto. Grava histórico ao alterar preço ou custo
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] ProdutoUpdateDto dto)
    {
        try
        {
            var produto = await _produtoService.UpdateAsync(id, dto); // Atualiza dados do produto
            return Ok(produto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message }); // Retorna erro caso produto não exista
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message }); // Retorna erro de validação/regra de negócio
        }
    }

    //Desativa (soft-delete) um produto
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _produtoService.DeleteAsync(id); // Realiza exclusão lógica do produto
            return NoContent(); // Retorna sucesso sem conteúdo
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message }); // Retorna erro caso produto não exista
        }
    }
}
