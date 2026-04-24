using MaxiMassas.DTOs.Cliente;
using MaxiMassas.Entities;
using MaxiMassas.Repositories.Interfaces;
using MaxiMassas.Services.Interfaces;

namespace MaxiMassas.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IViaCepService _viaCepService;

    public ClienteService(IClienteRepository clienteRepository, IViaCepService viaCepService)
    {
        _clienteRepository = clienteRepository;
        _viaCepService     = viaCepService;
    }

    public async Task<IEnumerable<ClienteResponseDto>> GetAllAsync()
    {
        var clientes = await _clienteRepository.GetAllAsync();
        return clientes.Select(MapToDto);
    }

    public async Task<ClienteResponseDto> GetByIdAsync(int id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Cliente com ID {id} não encontrado.");
        return MapToDto(cliente);
    }

    public async Task<ClienteResponseDto> CreateAsync(ClienteCreateDto dto)
    {
        // Valida o CEP nos Correios via ViaCEP — lança exceção se inválido
        var dadosCep = await _viaCepService.BuscarCepAsync(dto.CEP);

        // Se o campo Endereco vier vazio, preenche automaticamente com os dados do CEP
        var enderecoFinal = string.IsNullOrWhiteSpace(dto.Endereco)
            ? dadosCep.EnderecoFormatado
            : dto.Endereco.Trim();

        var cliente = new Cliente
        {
            Nome       = dto.Nome.Trim(),
            Telefone   = dto.Telefone.Trim(),
            Endereco   = enderecoFinal,
            CEP        = dadosCep.Cep, // CEP formatado 00000-000 pelo ViaCEP
            Observacao = dto.Observacao?.Trim()
        };

        var criado = await _clienteRepository.CreateAsync(cliente);
        return MapToDto(criado);
    }

    public async Task<ClienteResponseDto> UpdateAsync(int id, ClienteUpdateDto dto)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Cliente com ID {id} não encontrado.");

        // Só consulta o ViaCEP se o CEP foi alterado (evita chamada desnecessária)
        if (!CepIgual(cliente.CEP, dto.CEP))
        {
            var dadosCep = await _viaCepService.BuscarCepAsync(dto.CEP);

            cliente.CEP      = dadosCep.Cep;
            cliente.Endereco = string.IsNullOrWhiteSpace(dto.Endereco)
                ? dadosCep.EnderecoFormatado
                : dto.Endereco.Trim();
        }
        else
        {
            cliente.Endereco = dto.Endereco.Trim();
        }

        cliente.Nome       = dto.Nome.Trim();
        cliente.Telefone   = dto.Telefone.Trim();
        cliente.Observacao = dto.Observacao?.Trim();

        var atualizado = await _clienteRepository.UpdateAsync(cliente);
        return MapToDto(atualizado);
    }

    public async Task DeleteAsync(int id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Cliente com ID {id} não encontrado.");
        await _clienteRepository.DeleteAsync(cliente);
    }

    public async Task<ClienteHistoricoDto> GetHistoricoAsync(int id)
    {
        var cliente = await _clienteRepository.GetByIdWithVendasAsync(id)
            ?? throw new KeyNotFoundException($"Cliente com ID {id} não encontrado.");

        var totalGasto = cliente.Vendas
            .Where(v => v.StatusPagamento == StatusPagamento.Pago)
            .Sum(v => CalcularValorTotal(v));

        var compras = cliente.Vendas
            .OrderByDescending(v => v.DataVenda)
            .Select(v => new VendaResumoDto
            {
                VendaId         = v.Id,
                DataVenda       = v.DataVenda,
                ValorTotal      = CalcularValorTotal(v),
                StatusPagamento = v.StatusPagamento.ToString(),
                FormaPagamento  = v.FormaPagamento.ToString()
            }).ToList();

        return new ClienteHistoricoDto
        {
            ClienteId    = cliente.Id,
            NomeCliente  = cliente.Nome,
            TotalGasto   = totalGasto,
            TotalCompras = cliente.Vendas.Count,
            Compras      = compras
        };
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static bool CepIgual(string cepAtual, string cepNovo)
    {
        static string Limpar(string s) => s.Replace("-", "").Trim();
        return Limpar(cepAtual) == Limpar(cepNovo);
    }

    private static decimal CalcularValorTotal(Venda venda)
    {
        var subtotal    = venda.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);
        var desconto    = venda.DescontoValor > 0
            ? venda.DescontoValor
            : subtotal * (venda.DescontoPercentual / 100);
        var comDesconto = subtotal - desconto;
        return (comDesconto * (1 + venda.TaxaAplicada)) + venda.ValorFrete;
    }

    private static ClienteResponseDto MapToDto(Cliente c) => new()
    {
        Id         = c.Id,
        Nome       = c.Nome,
        Telefone   = c.Telefone,
        Endereco   = c.Endereco,
        CEP        = c.CEP,
        Observacao = c.Observacao,
        CriadoEm  = c.CriadoEm
    };
}
