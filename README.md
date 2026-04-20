# Maxi Massas Itápolis — API REST

Sistema de gestão interna para controle de clientes, produtos, vendas, estoque e relatórios.

---

## Tecnologias

| Tecnologia | Versão |
|---|---|
| .NET | 8.0 LTS |
| ASP.NET Core Web API | 8.0 |
| Entity Framework Core | 8.0 |
| SQL Server | 2022 / Express 2022 |
| Swagger (Swashbuckle) | 6.5 |
| BCrypt.Net-Next | 4.0.3 |
| JWT Bearer | 8.0 |

---

## Estrutura de Pastas

```
MaxiMassas/
├── Controllers/          # Endpoints REST
├── Services/
│   └── Interfaces/       # Contratos dos serviços
├── Repositories/
│   └── Interfaces/       # Contratos dos repositórios
├── Entities/             # Entidades do banco de dados
├── DTOs/                 # Objetos de entrada e saída
│   ├── Auth/
│   ├── Cliente/
│   ├── Produto/
│   ├── Venda/
│   ├── Estoque/
│   ├── ConsumoProprio/
│   └── Relatorio/
├── Data/                 # DbContext (AppDbContext)
├── Config/               # Configurações (JWT, Frete)
├── Migrations/           # Migrações do EF Core
├── Program.cs            # Ponto de entrada + DI + Middleware
└── appsettings.json      # Configurações da aplicação
```

---

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server 2022 ou SQL Server Express 2022
- (Opcional) [dotnet-ef global tool](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

---

## Configuração

### 1. String de Conexão

Edite `appsettings.json` com a sua string de conexão:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=MaxiMassasDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

> Para SQL Server com autenticação de usuário e senha:
> ```
> Server=localhost;Database=MaxiMassasDB;User Id=sa;Password=SuaSenha;TrustServerCertificate=True;
> ```

### 2. Chave JWT

Em produção, substitua `SecretKey` por uma string longa e aleatória:

```json
"JwtSettings": {
  "SecretKey": "SuaChaveSuperSecretaComMaisDe32Caracteres!",
  "Issuer": "MaxiMassasAPI",
  "Audience": "MaxiMassasClientes",
  "ExpirationHours": 8
}
```

### 3. Frete

```json
"FreteConfig": {
  "ValorFreteUaiRango": 5.00,
  "ValorMinimoFreteGratis": 80.00
}
```

---

## Instalação e Execução

### Passo 1 — Restaurar pacotes

```bash
cd MaxiMassas
dotnet restore
```

### Passo 2 — Instalar EF Core Tools (se necessário)

```bash
dotnet tool install --global dotnet-ef
```

### Passo 3 — Criar/Atualizar banco de dados

> O projeto já possui a migration `InitialCreate`. Basta executar:

```bash
dotnet ef database update
```

> Ou, se preferir recriar as migrations do zero:
> ```bash
> dotnet ef migrations add InitialCreate
> dotnet ef database update
> ```

### Passo 4 — Executar a aplicação

```bash
dotnet run
```

A API estará disponível em:

- **Swagger UI:** `http://localhost:5000` (raiz)
- **API Base:** `http://localhost:5000/api`

---

## Primeiro Acesso

### 1. Criar primeiro usuário

O endpoint de registro exige autenticação, **exceto na primeira vez**. Para criar o primeiro usuário, use o endpoint público de login ou temporariamente remova o `[Authorize]` do endpoint de registro.

> Dica rápida: adicione um seed no `Program.cs` para criar o primeiro admin automaticamente.

Exemplo de seed (adicione antes de `app.Run()`):

```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Usuarios.Any())
    {
        db.Usuarios.Add(new MaxiMassas.Entities.Usuario
        {
            Nome = "Administrador",
            Email = "admin@maximassas.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        });
        db.SaveChanges();
    }
}
```

### 2. Fazer login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@maximassas.com",
  "senha": "admin123"
}
```

Resposta:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "nome": "Administrador",
  "email": "admin@maximassas.com",
  "expiracao": "2024-01-01T20:00:00Z"
}
```

### 3. Usar o token

No Swagger: clique em **Authorize** e informe `Bearer {seu_token}`.

Em requisições HTTP: `Authorization: Bearer {seu_token}`

---

## Endpoints Disponíveis

### Auth
| Método | Rota | Descrição | Auth |
|---|---|---|---|
| POST | `/api/auth/login` | Login e geração de token | ❌ |
| POST | `/api/auth/registrar` | Cadastrar novo usuário | ✅ |
| GET | `/api/auth/usuarios` | Listar usuários | ✅ |

### Clientes
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/clientes` | Listar todos |
| GET | `/api/clientes/{id}` | Buscar por ID |
| GET | `/api/clientes/{id}/historico` | Histórico de compras + total gasto |
| POST | `/api/clientes` | Cadastrar |
| PUT | `/api/clientes/{id}` | Atualizar |
| DELETE | `/api/clientes/{id}` | Remover |

### Produtos
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/produtos` | Listar todos (com estoque) |
| GET | `/api/produtos/{id}` | Buscar por ID |
| GET | `/api/produtos/{id}/historico-preco` | Histórico de preços |
| POST | `/api/produtos` | Cadastrar (cria estoque zerado) |
| PUT | `/api/produtos/{id}` | Atualizar (grava histórico se preço mudar) |
| DELETE | `/api/produtos/{id}` | Desativar (soft-delete) |

### Vendas
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/vendas` | Listar todas |
| GET | `/api/vendas/{id}` | Buscar por ID |
| POST | `/api/vendas` | Registrar (desconta estoque) |
| PUT | `/api/vendas/{id}` | Atualizar |
| DELETE | `/api/vendas/{id}` | Cancelar (devolve estoque) |

### Estoque
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/estoque` | Listar tudo |
| GET | `/api/estoque/produto/{produtoId}` | Estoque de um produto |
| GET | `/api/estoque/alertas` | Produtos com qtd ≤ 3 |
| POST | `/api/estoque/repor` | Reposição manual (soma) |
| PUT | `/api/estoque/ajustar` | Ajuste para valor exato |

### Consumo Próprio
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/consumoproprio` | Listar todos |
| GET | `/api/consumoproprio/{id}` | Buscar por ID |
| GET | `/api/consumoproprio/produto/{produtoId}` | Por produto |
| POST | `/api/consumoproprio` | Registrar (desconta estoque) |
| DELETE | `/api/consumoproprio/{id}` | Remover |

### Relatórios
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/relatorios/financeiro` | Financeiro com filtro customizado |
| GET | `/api/relatorios/financeiro/diario` | Financeiro do dia |
| GET | `/api/relatorios/financeiro/semanal` | Financeiro da semana |
| GET | `/api/relatorios/financeiro/mensal` | Financeiro do mês |
| GET | `/api/relatorios/top-clientes` | Top clientes por valor gasto |
| GET | `/api/relatorios/produtos-mais-vendidos` | Produtos mais vendidos |
| GET | `/api/relatorios/completo` | Relatório completo |

---

## Regras de Negócio

### Taxas de Pagamento
| Forma | Taxa |
|---|---|
| Dinheiro | 0% |
| Pix | 0% |
| Débito | 1,37% |
| Crédito | 3,15% |
| UAI Rango (plataforma) | 13,5% |

### Horários de Entrega
| Tipo | Horário Permitido |
|---|---|
| Retirada | 10h às 20h |
| Entrega | 18h às 20h |

### Frete
| Plataforma | Regra |
|---|---|
| UAI Rango | R$ 5,00 fixo |
| Outros | Grátis acima do valor mínimo configurável |
| Retirada | Sempre grátis |

### Estoque
- Descontado automaticamente ao registrar venda
- Descontado automaticamente ao registrar consumo próprio
- Devolvido automaticamente ao cancelar venda
- Alerta quando quantidade ≤ 3 unidades

---

## Exemplos de Requisição

### Registrar Venda

```json
POST /api/vendas
{
  "clienteId": 1,
  "formaPagamento": 2,
  "plataformaOrigem": 0,
  "descontoPercentual": 10,
  "statusPagamento": 1,
  "formaEntrega": 0,
  "dataEntrega": "2024-12-20T00:00:00",
  "horarioEntrega": "14:00:00",
  "itens": [
    { "produtoId": 1, "quantidade": 2, "precoUnitario": 25.90 },
    { "produtoId": 3, "quantidade": 1, "precoUnitario": 18.50 }
  ]
}
```

### Repor Estoque

```json
POST /api/estoque/repor
{
  "produtoId": 1,
  "quantidade": 20
}
```

### Relatório Financeiro com Período Customizado

```
GET /api/relatorios/financeiro?DataInicio=2024-01-01&DataFim=2024-01-31
```

---

## Variáveis de Ambiente (Produção)

Recomenda-se usar variáveis de ambiente em vez de `appsettings.json` em produção:

```bash
ConnectionStrings__DefaultConnection="Server=...;Database=MaxiMassasDB;..."
JwtSettings__SecretKey="SuaChaveSuperSecreta"
```
