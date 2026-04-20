namespace MaxiMassas.DTOs.Produto;

public class ProdutoCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public decimal Peso { get; set; }
    public decimal PrecoVenda { get; set; }
    public decimal Custo { get; set; }
    public string? TipoVariacao { get; set; }
}

public class ProdutoUpdateDto
{
    public string Nome { get; set; } = string.Empty;
    public decimal Peso { get; set; }
    public decimal PrecoVenda { get; set; }
    public decimal Custo { get; set; }
    public string? TipoVariacao { get; set; }
    public string? MotivoAlteracao { get; set; }
}

public class ProdutoResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Peso { get; set; }
    public decimal PrecoVenda { get; set; }
    public decimal Custo { get; set; }
    public string? TipoVariacao { get; set; }
    public bool Ativo { get; set; }
    public DateTime CriadoEm { get; set; }
    public int QuantidadeEstoque { get; set; }
    public bool AlertaEstoqueBaixo { get; set; }
}

public class HistoricoPrecoDto
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public decimal PrecoVendaAnterior { get; set; }
    public decimal CustoAnterior { get; set; }
    public decimal PrecoVendaNovo { get; set; }
    public decimal CustoNovo { get; set; }
    public DateTime AlteradoEm { get; set; }
    public string? Motivo { get; set; }
}
