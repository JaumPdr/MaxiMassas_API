namespace MaxiMassas.DTOs.ConsumoProprio;

public class ConsumoProprioCreateDto
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public DateTime? Data { get; set; }
    public string? Motivo { get; set; }
}

public class ConsumoProprioResponseDto
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public DateTime Data { get; set; }
    public string? Motivo { get; set; }
}
