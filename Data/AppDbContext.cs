using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using BuenosAiresExp.Models;

namespace BuenosAiresExp.Data
{
    internal class AppDbContext: DbContext
    {
        public DbSet<Location> Locations { get; set; }
        // DbSet é como uma coleção (uma lista) de objetos Location que o Entity Framework salva e busca no banco de dados.
        // <Location> diz que cada item dessa coleção é um objeto da classe Location.
        // Locations é o nome dessa coleção e, por padrão, vira o nome da tabela correspondente no banco de dados.

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
    }
}
