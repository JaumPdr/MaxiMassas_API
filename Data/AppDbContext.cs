using MaxiMassas.Entities;
using Microsoft.EntityFrameworkCore;

namespace MaxiMassas.Data;

// Classe responsável pela comunicação com o banco de dados
public class AppDbContext : DbContext
{
    // Construtor que recebe as configurações do contexto
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    // Tabela de clientes
    public DbSet<Cliente> Clientes { get; set; }
    // Tabela de produtos
    public DbSet<Produto> Produtos { get; set; }
    // Tabela de histórico de preços
    public DbSet<HistoricoPreco> HistoricoPrecos { get; set; }
    // Tabela de vendas
    public DbSet<Venda> Vendas { get; set; }
    // Tabela de itens das vendas
    public DbSet<VendaItem> VendaItens { get; set; }
    // Tabela de estoque
    public DbSet<Estoque> Estoques { get; set; }
    // Tabela de consumos próprios
    public DbSet<ConsumoProprio> ConsumosProprios { get; set; }
    // Tabela de usuários
    public DbSet<Usuario> Usuarios { get; set; }

    // Método responsável por configurar as entidades e relacionamentos
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade Cliente
        modelBuilder.Entity<Cliente>(entity =>
        {
            // Define a chave primária
            entity.HasKey(e => e.Id);
            // Campo obrigatório com tamanho máximo de 150 caracteres
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(150);
            // Campo telefone com limite de 20 caracteres
            entity.Property(e => e.Telefone).HasMaxLength(20);
            // Campo endereço com limite de 300 caracteres
            entity.Property(e => e.Endereco).HasMaxLength(300);
            // Campo CEP com limite de 10 caracteres
            entity.Property(e => e.CEP).HasMaxLength(10);
            // Campo observação com limite de 500 caracteres
            entity.Property(e => e.Observacao).HasMaxLength(500);
        });

        // Configuração da entidade Produto
        modelBuilder.Entity<Produto>(entity =>
        {
            // Define a chave primária
            entity.HasKey(e => e.Id);
            // Campo obrigatório para nome do produto
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(150);
            // Define precisão decimal do peso
            entity.Property(e => e.Peso).HasColumnType("decimal(10,3)");
            // Define precisão decimal do preço de venda
            entity.Property(e => e.PrecoVenda).HasColumnType("decimal(10,2)");
            // Define precisão decimal do custo
            entity.Property(e => e.Custo).HasColumnType("decimal(10,2)");
            // Tipo de variação do produto
            entity.Property(e => e.TipoVariacao).HasMaxLength(100);
        });

        // Configuração da entidade HistoricoPreco
        modelBuilder.Entity<HistoricoPreco>(entity =>
        {
            // Define a chave primária
            entity.HasKey(e => e.Id);
            // Valor anterior do preço de venda
            entity.Property(e => e.PrecoVendaAnterior).HasColumnType("decimal(10,2)");
            // Valor anterior do custo
            entity.Property(e => e.CustoAnterior).HasColumnType("decimal(10,2)");
            // Novo valor do preço de venda
            entity.Property(e => e.PrecoVendaNovo).HasColumnType("decimal(10,2)");
            // Novo valor do custo
            entity.Property(e => e.CustoNovo).HasColumnType("decimal(10,2)");
            // Relacionamento com Produto
            entity.HasOne(e => e.Produto)
                  .WithMany(p => p.HistoricoPrecos)
                  .HasForeignKey(e => e.ProdutoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração da entidade Venda
        modelBuilder.Entity<Venda>(entity =>
        {
            // Define a chave primária
            entity.HasKey(e => e.Id);
            // Valor de desconto aplicado
            entity.Property(e => e.DescontoValor).HasColumnType("decimal(10,2)");
            // Percentual de desconto aplicado
            entity.Property(e => e.DescontoPercentual).HasColumnType("decimal(5,2)");
            // Taxa aplicada na venda
            entity.Property(e => e.TaxaAplicada).HasColumnType("decimal(5,4)");
            // Valor do frete
            entity.Property(e => e.ValorFrete).HasColumnType("decimal(10,2)");
            // Observações da venda
            entity.Property(e => e.Observacao).HasMaxLength(500);
            // Relacionamento com Cliente
            entity.HasOne(e => e.Cliente)
                  .WithMany(c => c.Vendas)
                  .HasForeignKey(e => e.ClienteId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuração da entidade VendaItem
        modelBuilder.Entity<VendaItem>(entity =>
        {
            // Define a chave primária
            entity.HasKey(e => e.Id);
            // Preço unitário do item
            entity.Property(e => e.PrecoUnitario).HasColumnType("decimal(10,2)");
            // Relacionamento com Venda
            entity.HasOne(e => e.Venda)
                  .WithMany(v => v.Itens)
                  .HasForeignKey(e => e.VendaId)
                  .OnDelete(DeleteBehavior.Cascade);
            // Relacionamento com Produto
            entity.HasOne(e => e.Produto)
                  .WithMany(p => p.VendaItens)
                  .HasForeignKey(e => e.ProdutoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuração da entidade Estoque
        modelBuilder.Entity<Estoque>(entity =>
        {
            // Define a chave primária
            entity.HasKey(e => e.Id);
            // Relacionamento 1 para 1 com Produto
            entity.HasOne(e => e.Produto)
                  .WithOne(p => p.Estoque)
                  .HasForeignKey<Estoque>(e => e.ProdutoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração da entidade ConsumoProprio
        modelBuilder.Entity<ConsumoProprio>(entity =>
        {
            // Define a chave primária
            entity.HasKey(e => e.Id);
            // Motivo do consumo próprio
            entity.Property(e => e.Motivo).HasMaxLength(300);
            // Relacionamento com Produto
            entity.HasOne(e => e.Produto)
                  .WithMany(p => p.ConsumosProprios)
                  .HasForeignKey(e => e.ProdutoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuração da entidade Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            // Define a chave primária
            entity.HasKey(e => e.Id);
            // Nome obrigatório
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(150);
            // Email obrigatório
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            // Garante que o email seja único
            entity.HasIndex(e => e.Email).IsUnique();
            // Campo obrigatório para senha criptografada
            entity.Property(e => e.SenhaHash).IsRequired();
        });
    }
}
