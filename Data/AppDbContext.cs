using BuenosAiresExp.Models;
using Microsoft.EntityFrameworkCore;

namespace BuenosAiresExp.Data
{
    internal class AppDbContext : DbContext
    {
        public DbSet<Location> Locations { get; set; }
        // DbSet é como uma coleção (uma lista) de objetos Location que o Entity Framework salva e busca no banco de dados.
        // <Location> diz que cada item dessa coleção é um objeto da classe Location.
        // Locations é o nome dessa coleção e, por padrão, vira o nome da tabela correspondente no banco de dados.

        // Adicionei as seguintes linhas para criar as tabelas Itineraries e ItineraryItems no banco de dados, usando as classes correspondentes como modelo.
        // Roteiros serão armazenados na tabela Itineraries, e cada item do roteiro (que liga um roteiro a uma localização específica) será armazenado na tabela ItineraryItems.
        public DbSet<Itinerary> Itineraries { get; set; }
        public DbSet<ItineraryItem> ItineraryItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // protected override void: está sobrescrevendo um método da classe base DbContext.
        // Esse método é usado para configurar como o contexto vai se conectar ao banco de dados.
        {
            // Usa o diretório base da aplicação para garantir que runtime e ferramentas apontem para o mesmo arquivo .db
            var dbPath = Path.Combine(AppContext.BaseDirectory, "buenos_aires.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
            // UseSqlite configura o Entity Framework para usar o banco de dados SQLite.
            // "Data Source=buenos_aires.db" é o nome/caminho do arquivo do banco de dados que será criado/usado.
        }
        // Usei migration para ler AppDbContext e criar 20260313175206_InitialCreate, que é o código que cria a tabela Locations no banco de dados.
        // Update-Database é o comando que executa essa criação no banco de dados real, usando o código da migration.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ItineraryItem>()
                .HasOne(ri => ri.Itinerary)
                .WithMany(r => r.Items)
                .HasForeignKey(ri => ri.ItineraryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItineraryItem>()
                .HasOne(ri => ri.Location)
                .WithMany()
                .HasForeignKey(ri => ri.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // cascade: se um roteiro for deletado, todos os itens relacionados a ele também serão deletados.
            // restrict: se uma localização estiver associada a algum item de roteiro, ela não pode ser deletada até que os itens sejam removidos ou atualizados.
            // Essas configurações garantem que a integridade dos dados seja mantida, evitando que itens de roteiro fiquem órfãos ou que localizações sejam deletadas acidentalmente enquanto ainda estão em uso.
            // dois deletes: um para a relação entre ItineraryItem e Itinerary, e outro para a relação entre ItineraryItem e Location. O primeiro é cascade, o segundo é restrict.
        }
    }
}
