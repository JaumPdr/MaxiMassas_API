using MaxiMassas.Entities;

namespace MaxiMassas.DTOs.Venda;

public class VendaCreateDto
{
    public int ClienteId { get; set; }
    public FormaPagamento FormaPagamento { get; set; }
    public PlataformaOrigem PlataformaOrigem { get; set; }
    public decimal DescontoValor { get; set; } = 0;
    public decimal DescontoPercentual { get; set; } = 0;
    public StatusPagamento StatusPagamento { get; set; } = StatusPagamento.Pendente;
    public FormaEntrega FormaEntrega { get; set; }
    public DateTime? DataEntrega { get; set; }
    public TimeSpan? HorarioEntrega { get; set; }
    public string? Observacao { get; set; }
    public List<VendaItemCreateDto> Itens { get; set; } = new();
}

public class VendaItemCreateDto
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}

public class VendaUpdateDto
{
    public FormaPagamento FormaPagamento { get; set; }
    public StatusPagamento StatusPagamento { get; set; }
    public decimal DescontoValor { get; set; } = 0;
    public decimal DescontoPercentual { get; set; } = 0;
    public DateTime? DataEntrega { get; set; }
    public TimeSpan? HorarioEntrega { get; set; }
    public string? Observacao { get; set; }
}

public class VendaResponseDto
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public DateTime DataVenda { get; set; }
    public string FormaPagamento { get; set; } = string.Empty;
    public string PlataformaOrigem { get; set; } = string.Empty;
    public decimal DescontoValor { get; set; }
    public decimal DescontoPercentual { get; set; }
    public string StatusPagamento { get; set; } = string.Empty;
    public string FormaEntrega { get; set; } = string.Empty;
    public DateTime? DataEntrega { get; set; }
    public TimeSpan? HorarioEntrega { get; set; }
    public decimal TaxaAplicada { get; set; }
    public decimal ValorFrete { get; set; }
    public decimal SubTotal { get; set; }
    public decimal ValorDesconto { get; set; }
    public decimal ValorComTaxa { get; set; }
    public decimal ValorTotal { get; set; }
    public string? Observacao { get; set; }
    public List<VendaItemResponseDto> Itens { get; set; } = new();
}

public class VendaItemResponseDto
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal SubTotal { get; set; }
}
