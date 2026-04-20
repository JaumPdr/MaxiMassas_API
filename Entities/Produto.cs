namespace MaxiMassas.Entities;

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Peso { get; set; }
    public decimal PrecoVenda { get; set; }
    public decimal Custo { get; set; }
    public string? TipoVariacao { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public Estoque? Estoque { get; set; }
    public ICollection<VendaItem> VendaItens { get; set; } = new List<VendaItem>();
    public ICollection<ConsumoProprio> ConsumosProprios { get; set; } = new List<ConsumoProprio>();
    public ICollection<HistoricoPreco> HistoricoPrecos { get; set; } = new List<HistoricoPreco>();
}
