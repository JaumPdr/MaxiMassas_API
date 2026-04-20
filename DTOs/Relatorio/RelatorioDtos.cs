namespace MaxiMassas.DTOs.Relatorio;

public class RelatorioFinanceiroDto
{
    public decimal Faturamento { get; set; }
    public decimal CustoTotal { get; set; }
    public decimal Lucro { get; set; }
    public decimal PercentualLucro { get; set; }
    public int TotalVendas { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
}

public class TopClienteDto
{
    public int ClienteId { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public decimal TotalGasto { get; set; }
    public int TotalCompras { get; set; }
    public int Posicao { get; set; }
}

public class ProdutoMaisVendidoDto
{
    public int ProdutoId { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public int QuantidadeVendida { get; set; }
    public decimal ReceitaGerada { get; set; }
    public int Posicao { get; set; }
}

public class RelatorioCompletoDto
{
    public RelatorioFinanceiroDto Financeiro { get; set; } = new();
    public List<TopClienteDto> TopClientes { get; set; } = new();
    public List<ProdutoMaisVendidoDto> ProdutosMaisVendidos { get; set; } = new();
}

public class RelatorioFiltroDto
{
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string Periodo { get; set; } = "mensal"; // diario, semanal, mensal
}
