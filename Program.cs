using System.Text;
using MaxiMassas.Config;
using MaxiMassas.Data;
using MaxiMassas.Entities;
using MaxiMassas.Repositories;
using MaxiMassas.Repositories.Interfaces;
using MaxiMassas.Services;
using MaxiMassas.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ─── Configurações ────────────────────────────────────────────────────────────
// Carrega as configurações do appsettings.json
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<FreteConfig>(builder.Configuration.GetSection("FreteConfig"));

// ─── Banco de Dados ───────────────────────────────────────────────────────────
// Configura conexão com o SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ─── Repositórios ─────────────────────────────────────────────────────────────
// Injeta os repositórios responsáveis pelo acesso aos dados
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IVendaRepository, VendaRepository>();
builder.Services.AddScoped<IEstoqueRepository, EstoqueRepository>();
builder.Services.AddScoped<IConsumoProprioRepository, ConsumoProprioRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// ─── Serviços ─────────────────────────────────────────────────────────────────
// Injeta as regras de negócio da aplicação
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IVendaService, VendaService>();
builder.Services.AddScoped<IEstoqueService, EstoqueService>();
builder.Services.AddScoped<IConsumoProprioService, ConsumoProprioService>();
builder.Services.AddScoped<IRelatorioService, RelatorioService>();

// ─── Autenticação JWT ─────────────────────────────────────────────────────────
// Obtém as configurações do JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
var chaveBytes = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

// Configura autenticação utilizando JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Permite autenticação sem HTTPS em ambiente local
    options.RequireHttpsMetadata = false;
    // Salva o token após autenticação
    options.SaveToken            = true;
    // Define as validações do token JWT
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey         = new SymmetricSecurityKey(chaveBytes),
        ValidateIssuer           = true,
        ValidIssuer              = jwtSettings.Issuer,
        ValidateAudience         = true,
        ValidAudience            = jwtSettings.Audience,
        ValidateLifetime         = true,
        // Remove tolerância de tempo na expiração do token
        ClockSkew = TimeSpan.Zero
    };

    // Retorna mensagem personalizada quando usuário não está autenticado
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode  = 401;
            context.Response.ContentType = "application/json";
            var msg = System.Text.Json.JsonSerializer.Serialize(new
            {
                mensagem = "Não autorizado. Faça POST /api/auth/login e use o token no botão Authorize do Swagger (formato: Bearer {token})."
            });
            return context.Response.WriteAsync(msg);
        }
    };
});

builder.Services.AddAuthorization();

// ─── Controllers ─────────────────────────────────────────────────────────────
// Adiciona suporte aos controllers da API
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Faz enums serem exibidos como texto no JSON
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// ─── Swagger / OpenAPI ────────────────────────────────────────────────────────
// Habilita geração automática da documentação da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "Maxi Massas Itápolis - API",
        Version     = "v1",
        Description = "API de gestão interna para controle de clientes, produtos, vendas, estoque e relatórios.",
        Contact     = new OpenApiContact { Name = "Maxi Massas", Email = "contato@maximassas.com.br" }
    });

    // Configuração do JWT dentro do Swagger
    var securityScheme = new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Description  = "Informe: Bearer {seu_token}",
        In           = ParameterLocation.Header,
        Type         = SecuritySchemeType.Http,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        Reference    = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };

    // Adiciona esquema de segurança JWT
    c.AddSecurityDefinition("Bearer", securityScheme);
    // Exige autenticação nas rotas protegidas
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });

    // Inclui comentários XML na documentação Swagger
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

// ─── CORS ─────────────────────────────────────────────────────────────────────
// Libera acesso da API para qualquer origem
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Cria a aplicação
var app = builder.Build();

// ─── Migrations + Seed do primeiro usuário admin ──────────────────────────────
using (var scope = app.Services.CreateScope())
{
    // Obtém contexto do banco
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Executa migrations automaticamente
    db.Database.Migrate();

    // Cria usuário admin caso não exista nenhum usuário
    if (!db.Usuarios.Any())
    {
        db.Usuarios.Add(new Usuario
        {
            Nome      = "Administrador",
            Email     = "admin@maximassas.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Ativo     = true,
            CriadoEm = DateTime.UtcNow
        });
        db.SaveChanges();
        Console.WriteLine(">>> Usuário admin criado: admin@maximassas.com / senha: admin123");
    }
}

// ─── Middleware Pipeline ──────────────────────────────────────────────────────
// Habilita Swagger em ambiente de desenvolvimento e produção
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Maxi Massas API v1");
        c.RoutePrefix = string.Empty; // Swagger na raiz: http://localhost:5000/
        c.DocumentTitle = "Maxi Massas Itápolis - API";
    });
}

// Habilita política de CORS
app.UseCors("AllowAll");

// Habilita autenticação JWT
app.UseAuthentication();

// Habilita autorização
app.UseAuthorization();

// Mapeia os controllers da aplicação
app.MapControllers();

// Inicia a aplicação
app.Run();
