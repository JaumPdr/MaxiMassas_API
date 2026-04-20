using MaxiMassas.Entities;
using Microsoft.EntityFrameworkCore;

namespace MaxiMassas.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<HistoricoPreco> HistoricoPrecos { get; set; }
    public DbSet<Venda> Vendas { get; set; }
    public DbSet<VendaItem> VendaItens { get; set; }
    public DbSet<Estoque> Estoques { get; set; }
    public DbSet<ConsumoProprio> ConsumosProprios { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cliente
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Telefone).HasMaxLength(20);
            entity.Property(e => e.Endereco).HasMaxLength(300);
            entity.Property(e => e.CEP).HasMaxLength(10);
            entity.Property(e => e.Observacao).HasMaxLength(500);
        });

        // Produto
        modelBuilder.Entity<Produto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Peso).HasColumnType("decimal(10,3)");
            entity.Property(e => e.PrecoVenda).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Custo).HasColumnType("decimal(10,2)");
            entity.Property(e => e.TipoVariacao).HasMaxLength(100);
        });

        // HistoricoPreco
        modelBuilder.Entity<HistoricoPreco>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PrecoVendaAnterior).HasColumnType("decimal(10,2)");
            entity.Property(e => e.CustoAnterior).HasColumnType("decimal(10,2)");
            entity.Property(e => e.PrecoVendaNovo).HasColumnType("decimal(10,2)");
            entity.Property(e => e.CustoNovo).HasColumnType("decimal(10,2)");
            entity.HasOne(e => e.Produto)
                  .WithMany(p => p.HistoricoPrecos)
                  .HasForeignKey(e => e.ProdutoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Venda
        modelBuilder.Entity<Venda>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DescontoValor).HasColumnType("decimal(10,2)");
            entity.Property(e => e.DescontoPercentual).HasColumnType("decimal(5,2)");
            entity.Property(e => e.TaxaAplicada).HasColumnType("decimal(5,4)");
            entity.Property(e => e.ValorFrete).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Observacao).HasMaxLength(500);
            entity.HasOne(e => e.Cliente)
                  .WithMany(c => c.Vendas)
                  .HasForeignKey(e => e.ClienteId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // VendaItem
        modelBuilder.Entity<VendaItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PrecoUnitario).HasColumnType("decimal(10,2)");
            entity.HasOne(e => e.Venda)
                  .WithMany(v => v.Itens)
                  .HasForeignKey(e => e.VendaId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Produto)
                  .WithMany(p => p.VendaItens)
                  .HasForeignKey(e => e.ProdutoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Estoque
        modelBuilder.Entity<Estoque>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Produto)
                  .WithOne(p => p.Estoque)
                  .HasForeignKey<Estoque>(e => e.ProdutoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ConsumoProprio
        modelBuilder.Entity<ConsumoProprio>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Motivo).HasMaxLength(300);
            entity.HasOne(e => e.Produto)
                  .WithMany(p => p.ConsumosProprios)
                  .HasForeignKey(e => e.ProdutoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.SenhaHash).IsRequired();
        });
    }
}
