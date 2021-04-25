using System;
using System.Linq;
using IntroducaoEfCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IntroducaoEfCore.Data
{
    public class ApplicationContext : DbContext
    {
        private static readonly ILoggerFactory _logger = LoggerFactory.Create(p => p.AddConsole());
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLoggerFactory(_logger)
                .EnableSensitiveDataLogging()
                .UseSqlServer("Data source=(localdb)\\mssqllocaldb;Initial Catalog=IntroducaoEfCore;Integrated Security=true",
                    p => p.EnableRetryOnFailure(
                        maxRetryCount: 2, 
                        maxRetryDelay: TimeSpan.FromSeconds(5), 
                        errorNumbersToAdd: null
                    )
                    .MigrationsHistoryTable("TabelaMigracoes")
                );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //Forma 3 e melhor
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);            

            // Forma 2
            // modelBuilder.ApplyConfiguration(new Configurations.ClienteConfiguration());
            // modelBuilder.ApplyConfiguration(new Configurations.PedidoConfiguration());
            // modelBuilder.ApplyConfiguration(new Configurations.ProdutoConfiguration());
            // modelBuilder.ApplyConfiguration(new Configurations.PedidoItemConfiguration());

            //FORMA 1
            // modelBuilder.Entity<Cliente>( p => 
            // {
            //     p.ToTable("Clientes");
            //     p.HasKey(p => p.Id);
            //     p.Property(p => p.Nome).HasColumnType("VARCHAR(80)").IsRequired();
            //     p.Property(p => p.Telefone).HasColumnType("CHAR(11)");
            //     p.Property(p => p.CEP).HasColumnType("CHAR(8)").IsRequired();
            //     p.Property(p => p.Estado).HasColumnType("CHAR(2)").IsRequired();
            //     p.Property(p => p.Cidade).HasMaxLength(60).IsRequired();

            //     p.HasIndex(i => i.Telefone).HasDatabaseName("idx_cliente_telefone");
            // });

            // modelBuilder.Entity<Produto>(p => 
            // {
            //     p.ToTable("Produtos");
            //     p.HasKey(p => p.Id);
            //     p.Property(p => p.CodigoBarras).HasColumnType("VARCHAR(14)").IsRequired();
            //     p.Property(p => p.Descricao).HasColumnType("VARCHAR(60)");
            //     p.Property(p => p.Valor).IsRequired();
            //     p.Property(p => p.TipoProduto).HasConversion<string>();
            // });

            // modelBuilder.Entity<Pedido>(p => 
            // {
            //     p.ToTable("Pedidos");
            //     p.HasKey(p => p.Id);
            //     p.Property(p => p.IniciadoEm).HasDefaultValueSql("GETDATE()").ValueGeneratedOnAdd();
            //     p.Property(p => p.Status).HasConversion<string>();
            //     p.Property(p => p.TipoFrete).HasConversion<int>();
            //     p.Property(p => p.Observacao).HasColumnType("VARCHAR(512)");

            //     p.HasMany(p => p.Itens)
            //         .WithOne(p => p.Pedido)
            //         .OnDelete(DeleteBehavior.Cascade);
            // });

            // modelBuilder.Entity<PedidoItem>(p => 
            // {
            //     p.ToTable("PedidoItens");
            //     p.HasKey(p => p.Id);
            //     p.Property(p => p.Quantidade).HasDefaultValue(1).IsRequired();
            //     p.Property(p => p.Valor).IsRequired();
            //     p.Property(p => p.Desconto).IsRequired();                
            // });

            MapearPropriedadesEsquecidas(modelBuilder);
        }
    
    
        private void MapearPropriedadesEsquecidas(ModelBuilder modelBuilder) 
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                //No exemplo abaixo estamos rastreando strings que não tenham sido mapeadas o maxlength ou um tipo no banco de dados            

                //Primeiramente é obtido todas as propriedades das entidades mapeadas do tipo string
                var properties = entity.GetProperties().Where(p => p.ClrType == typeof(string));

             
                foreach (var property in properties)
                {
                    //Para cada string encontrada, verifica se ela tem um tipo e se tem um maxlength definido
                    if (string.IsNullOrEmpty(property.GetColumnType()) && !property.GetMaxLength().HasValue) 
                    {
                        //Caso não tenha, seta o maxlength ou o columntype, que produzirá o mesmo resultado.
                        //property.SetMaxLength(100);
                        property.SetColumnType("VARCHAR(100)");
                    }
                }
            }
        }

    }
}