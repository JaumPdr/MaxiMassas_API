using MaxiMassas.DTOs.Cliente;

namespace MaxiMassas.Services.Interfaces;

public interface IViaCepService
{
    /// <summary>
    /// Consulta o ViaCEP e retorna os dados do endereço.
    /// Lança InvalidOperationException se o CEP for inválido ou não encontrado.
    /// </summary>
    Task<ViaCepResponseDto> BuscarCepAsync(string cep);

    /// <summary>
    /// Valida se o CEP existe. Retorna false se inválido ou não encontrado.
    /// Não lança exceção.
    /// </summary>
    Task<bool> CepExisteAsync(string cep);
}
