namespace MaxiMassas.Entities;

public class Estoque
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;

    public Produto Produto { get; set; } = null!;
}
