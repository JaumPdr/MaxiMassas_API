namespace MaxiMassas.Entities;

public class HistoricoPreco
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public decimal PrecoVendaAnterior { get; set; }
    public decimal CustoAnterior { get; set; }
    public decimal PrecoVendaNovo { get; set; }
    public decimal CustoNovo { get; set; }
    public DateTime AlteradoEm { get; set; } = DateTime.UtcNow;
    public string? Motivo { get; set; }

    public Produto Produto { get; set; } = null!;
}
