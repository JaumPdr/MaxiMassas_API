namespace MaxiMassas.Config;

// Classe responsável pelas configurações do JWT
public class JwtSettings
{
    // Chave secreta utilizada para gerar e validar tokens
    public string SecretKey { get; set; } = string.Empty;
    // Emissor do token JWT
    public string Issuer { get; set; } = string.Empty;
    // Público alvo do token JWT
    public string Audience { get; set; } = string.Empty;
    // Tempo de expiração do token em horas
    public int ExpirationHours { get; set; } = 8;
}

// Classe responsável pelas configurações de frete
public class FreteConfig
{
    // Valor padrão do frete para entregas Uai Rango
    public decimal ValorFreteUaiRango { get; set; } = 5.00m;
    // Valor mínimo para liberar frete grátis
    public decimal ValorMinimoFreteGratis { get; set; } = 80.00m;
}