namespace MaxiMassas.Config;

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationHours { get; set; } = 8;
}

public class FreteConfig
{
    public decimal ValorFreteUaiRango { get; set; } = 5.00m;
    public decimal ValorMinimoFreteGratis { get; set; } = 80.00m;
}
