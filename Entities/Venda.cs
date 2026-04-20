namespace MaxiMassas.Entities;

public enum FormaPagamento
{
    Dinheiro = 0,
    Debito = 1,
    Credito = 2,
    Pix = 3
}

public enum PlataformaOrigem
{
    Direto = 0,
    UaiRango = 1,
    WhatsApp = 2,
    Instagram = 3,
    Outro = 4
}

public enum StatusPagamento
{
    Pendente = 0,
    Pago = 1
}

public enum FormaEntrega
{
    Retirada = 0,
    Entrega = 1
}

public class Venda
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public DateTime DataVenda { get; set; } = DateTime.UtcNow;
    public FormaPagamento FormaPagamento { get; set; }
    public PlataformaOrigem PlataformaOrigem { get; set; }
    public decimal DescontoValor { get; set; } = 0;
    public decimal DescontoPercentual { get; set; } = 0;
    public StatusPagamento StatusPagamento { get; set; } = StatusPagamento.Pendente;
    public FormaEntrega FormaEntrega { get; set; }
    public DateTime? DataEntrega { get; set; }
    public TimeSpan? HorarioEntrega { get; set; }
    public decimal TaxaAplicada { get; set; } = 0;
    public decimal ValorFrete { get; set; } = 0;
    public string? Observacao { get; set; }

    public Cliente Cliente { get; set; } = null!;
    public ICollection<VendaItem> Itens { get; set; } = new List<VendaItem>();
}
