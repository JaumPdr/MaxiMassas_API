namespace MaxiMassas.DTOs.Cliente;

public class ClienteCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
    public string? Observacao { get; set; }
}

public class ClienteUpdateDto
{
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
    public string? Observacao { get; set; }
}

public class ClienteResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
    public string? Observacao { get; set; }
    public DateTime CriadoEm { get; set; }
}

public class ClienteHistoricoDto
{
    public int ClienteId { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public decimal TotalGasto { get; set; }
    public int TotalCompras { get; set; }
    public List<VendaResumoDto> Compras { get; set; } = new();
}

public class VendaResumoDto
{
    public int VendaId { get; set; }
    public DateTime DataVenda { get; set; }
    public decimal ValorTotal { get; set; }
    public string StatusPagamento { get; set; } = string.Empty;
    public string FormaPagamento { get; set; } = string.Empty;
}
