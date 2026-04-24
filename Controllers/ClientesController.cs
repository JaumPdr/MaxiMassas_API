using MaxiMassas.DTOs.Cliente;
using MaxiMassas.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMassas.Controllers;

// Define como controller de API
[ApiController]

// Define rota base: api/clientes
[Route("api/[controller]")]

// Exige autenticação JWT em todos os endpoints
[Authorize]

// Define retorno padrão como JSON
[Produces("application/json")]
public class ClientesController : ControllerBase
{
    // Serviço responsável pelas regras de negócio dos clientes
    private readonly IClienteService _clienteService;

    // Serviço responsável pela consulta de CEP via ViaCEP
    private readonly IViaCepService _viaCepService;

    // Injeta os serviços utilizados no controller
    public ClientesController(IClienteService clienteService, IViaCepService viaCepService)
    {
        _clienteService = clienteService;
        _viaCepService = viaCepService;
    }

    // Consulta um CEP e retorna os dados do endereço.
    // Endpoint: GET api/clientes/buscar-cep/{cep}
    [HttpGet("buscar-cep/{cep}")]

    // Retorno esperado em caso de sucesso
    [ProducesResponseType(typeof(ViaCepResponseDto), StatusCodes.Status200OK)]

    // Retorno esperado em caso de CEP inválido
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BuscarCep(string cep)
    {
        try
        {
            // Consulta dados do CEP na API ViaCEP
            var resultado = await _viaCepService.BuscarCepAsync(cep);

            // Retorna dados do endereço
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            // Retorna erro caso CEP seja inválido
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // Lista todos os clientes cadastrados.
    // Endpoint: GET api/clientes
    [HttpGet]

    // Retorno esperado em caso de sucesso
    [ProducesResponseType(typeof(IEnumerable<ClienteResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        // Busca todos os clientes
        var clientes = await _clienteService.GetAllAsync();

        // Retorna lista de clientes
        return Ok(clientes);
    }

    // Busca um cliente pelo ID.
    // Endpoint: GET api/clientes/{id}
    [HttpGet("{id:int}")]

    // Retorno esperado em caso de sucesso
    [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status200OK)]

    // Retorno esperado caso cliente não exista
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            // Busca cliente pelo ID
            var cliente = await _clienteService.GetByIdAsync(id);

            // Retorna cliente encontrado
            return Ok(cliente);
        }
        catch (KeyNotFoundException ex)
        {
            // Retorna erro caso cliente não exista
            return NotFound(new { mensagem = ex.Message });
        }
    }

    // Retorna o histórico de compras e total gasto de um cliente.
    // Endpoint: GET api/clientes/{id}/historico
    [HttpGet("{id:int}/historico")]

    // Retorno esperado em caso de sucesso
    [ProducesResponseType(typeof(ClienteHistoricoDto), StatusCodes.Status200OK)]

    // Retorno esperado caso cliente não exista
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHistorico(int id)
    {
        try
        {
            // Busca histórico de compras do cliente
            var historico = await _clienteService.GetHistoricoAsync(id);

            // Retorna histórico do cliente
            return Ok(historico);
        }
        catch (KeyNotFoundException ex)
        {
            // Retorna erro caso cliente não exista
            return NotFound(new { mensagem = ex.Message });
        }
    }

    // Cadastra um novo cliente.
    // Endpoint: POST api/clientes
    [HttpPost]

    // Retorno esperado em caso de sucesso
    [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status201Created)]

    // Retorno esperado em caso de erro de validação
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ClienteCreateDto dto)
    {
        try
        {
            // Cria novo cliente
            var cliente = await _clienteService.CreateAsync(dto);

            // Retorna cliente criado
            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        }
        catch (InvalidOperationException ex)
        {
            // Retorna erro de validação/regra de negócio
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            // Retorna erro genérico
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // Atualiza os dados de um cliente.
    // Endpoint: PUT api/clientes/{id}
    [HttpPut("{id:int}")]

    // Retorno esperado em caso de sucesso
    [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status200OK)]

    // Retorno esperado caso cliente não exista
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    // Retorno esperado em caso de erro de validação
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] ClienteUpdateDto dto)
    {
        try
        {
            // Atualiza dados do cliente
            var cliente = await _clienteService.UpdateAsync(id, dto);

            // Retorna cliente atualizado
            return Ok(cliente);
        }
        catch (KeyNotFoundException ex)
        {
            // Retorna erro caso cliente não exista
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // Retorna erro de validação/regra de negócio
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            // Retorna erro genérico
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // Remove um cliente pelo ID.
    // Endpoint: DELETE api/clientes/{id}
    [HttpDelete("{id:int}")]

    // Retorno esperado em caso de sucesso
    [ProducesResponseType(StatusCodes.Status204NoContent)]

    // Retorno esperado caso cliente não exista
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            // Remove cliente pelo ID
            await _clienteService.DeleteAsync(id);

            // Retorna sucesso sem conteúdo
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            // Retorna erro caso cliente não exista
            return NotFound(new { mensagem = ex.Message });
        }
    }
}