namespace MaxiMassas.DTOs.Estoque;

public class EstoqueResponseDto
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public DateTime AtualizadoEm { get; set; }
    public bool AlertaBaixo { get; set; }
}

public class EstoqueReposicaoDto
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
}

public class EstoqueAjusteDto
{
    public int ProdutoId { get; set; }
    public int NovaQuantidade { get; set; }
    public string? Motivo { get; set; }
}
