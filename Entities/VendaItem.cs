namespace MaxiMassas.Entities;

public class VendaItem
{
    public int Id { get; set; }
    public int VendaId { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }

    public Venda Venda { get; set; } = null!;
    public Produto Produto { get; set; } = null!;
}
