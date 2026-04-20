namespace MaxiMassas.Entities;

public class ConsumoProprio
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public DateTime Data { get; set; } = DateTime.UtcNow;
    public string? Motivo { get; set; }

    public Produto Produto { get; set; } = null!;
}
