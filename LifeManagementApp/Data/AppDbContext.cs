using System.IO;
using Microsoft.EntityFrameworkCore;
using LifeManagementApp.Models;

namespace LifeManagementApp.Data
{
    /// <summary>
    /// AppDbContext toimii siltana C#-koodin ja paikallisen SQLite-tietokannan välillä.
    /// Se perii EF Coren DbContext-luokan ja hallitsee tietokantakyselyitä sekä tallennusta.
    /// </summary>
    public class AppDbContext : DbContext
    {
        // DbSet edustaa tietokannan taulua "TodoItems".
        // Tämän kautta suoritetaan LINQ-haut, lisäykset, muokkaukset ja poistot.
        public DbSet<TodoItem> TodoItems => Set<TodoItem>();

        // Kategoriataulu (Categories) - Osa-alue 3 (8p) relaatio: Category 1-N TodoItem
        public DbSet<Category> Categories => Set<Category>();

        public AppDbContext()
        {
            // Code First -alustus:
            // Database.EnsureCreated() varmistaa, että SQLite-tiedosto ja taulut
            // luodaan laitteelle automaattisesti, mikäli niitä ei vielä ole olemassa.
            // Tämä poistaa erillisten migraatiotiedostojen ajotarpeen mobiililaitteella.
            Database.EnsureCreated();
        }

        /// <summary>
        /// Määritellään käytettävä tietokantamoottori (SQLite) ja tietokantatiedoston tallennuspolku.
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // FileSystem.AppDataDirectory on MAUI:n tarjoama alustariippumaton kansio.
            // Se osoittaa Windowsilla käyttäjän AppDataan ja Androidilla sovelluksen suojattuun sisäiseen muistiin.
            // Näin polkua ei tarvitse kovakoodata erikseen eri käyttöjärjestelmille.
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "lifemanagement.db3");

            // Määritetään Entity Framework käyttämään SQLiteä annetulla polulla.
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Määritellään 1-N relaatio:
            // Yhdellä kategorialla on monta TodoItemiä, yhdellä TodoItemillä on yksi kategoria
            modelBuilder.Entity<TodoItem>()
                .HasOne(t => t.Category)
                .WithMany(c => c.TodoItems)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Lisätään oletuskategoriat (Seed Data)
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Koulu" },
                new Category { Id = 2, Name = "Koti" },
                new Category { Id = 3, Name = "Vapaa-aika" }
            );
        }
    }
}