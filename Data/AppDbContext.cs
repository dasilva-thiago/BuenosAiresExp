using BuenosAiresExp.Models;
using Microsoft.EntityFrameworkCore;

namespace BuenosAiresExp.Data
{
    internal class AppDbContext : DbContext
    {
        public DbSet<Location> Locations { get; set; }
        public DbSet<Itinerary> Itineraries { get; set; }
        public DbSet<ItineraryItem> ItineraryItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var exeDir = Path.GetDirectoryName(Environment.ProcessPath)
                         ?? AppContext.BaseDirectory;
            var dbPath = Path.Combine(exeDir, "buenos_aires.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

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
        }
    }
}
