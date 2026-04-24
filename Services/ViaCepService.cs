using System.Net.Http.Json;
using System.Text.RegularExpressions;
using MaxiMassas.DTOs.Cliente;
using MaxiMassas.Services.Interfaces;

namespace MaxiMassas.Services;

public class ViaCepService : IViaCepService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ViaCepService> _logger;

    // Aceita formatos: 14500000, 14500-000
    private static readonly Regex RegexCep = new(@"^\d{5}-?\d{3}$", RegexOptions.Compiled);

    public ViaCepService(HttpClient httpClient, ILogger<ViaCepService> logger)
    {
        _httpClient = httpClient;
        _logger     = logger;
    }

    public async Task<ViaCepResponseDto> BuscarCepAsync(string cep)
    {
        var cepLimpo = LimparCep(cep);

        ValidarFormatoCep(cepLimpo);

        try
        {
            var response = await _httpClient.GetAsync($"{cepLimpo}/json/");

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"CEP '{FormatarCep(cepLimpo)}' não encontrado.");

            var dados = await response.Content.ReadFromJsonAsync<ViaCepResponseDto>();

            if (dados == null || dados.Erro)
                throw new InvalidOperationException($"CEP '{FormatarCep(cepLimpo)}' não encontrado nos Correios.");

            // Normaliza o CEP para o formato 00000-000
            dados.Cep = FormatarCep(cepLimpo);

            return dados;
        }
        catch (InvalidOperationException)
        {
            throw; // Repropaga erros de negócio
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Falha na comunicação com o ViaCEP para o CEP {Cep}", cepLimpo);
            throw new InvalidOperationException(
                "Não foi possível consultar o serviço de CEP dos Correios. Tente novamente em instantes.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao consultar CEP {Cep}", cepLimpo);
            throw new InvalidOperationException("Erro ao consultar o CEP. Verifique o valor informado.");
        }
    }

    public async Task<bool> CepExisteAsync(string cep)
    {
        try
        {
            await BuscarCepAsync(cep);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static string LimparCep(string cep)
        => cep.Replace("-", "").Replace(" ", "").Trim();

    private static string FormatarCep(string cepSemMascara)
        => cepSemMascara.Length == 8
            ? $"{cepSemMascara[..5]}-{cepSemMascara[5..]}"
            : cepSemMascara;

    private static void ValidarFormatoCep(string cepLimpo)
    {
        if (string.IsNullOrWhiteSpace(cepLimpo) || cepLimpo.Length != 8 || !cepLimpo.All(char.IsDigit))
            throw new InvalidOperationException(
                $"CEP '{cepLimpo}' possui formato inválido. Informe 8 dígitos numéricos (ex: 14500-000).");
    }
}
