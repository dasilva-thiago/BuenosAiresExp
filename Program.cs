using BuenosAiresExp.Services;
using BuenosAiresExp.Views;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BuenosAiresExp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            using (var context = AppDbContextFactory.Create())
            {
                try
                {
                    if (context.Database.GetMigrations().Any())
                    {
                        context.Database.Migrate();
                    }
                    else
                    {
                        context.Database.EnsureCreated();
                    }

                    EnsureLegacySchema(context);
                }
                catch
                {
                    // Schema incompativel: apaga e recria o banco com schema correto
                    context.Database.EnsureDeleted();
                    if (context.Database.GetMigrations().Any())
                    {
                        context.Database.Migrate();
                    }
                    else
                    {
                        context.Database.EnsureCreated();
                    }

                    EnsureLegacySchema(context);
                }
            }

            ApplicationConfiguration.Initialize();
            Application.Run(new HomeForm());
        }

        private static void EnsureLegacySchema(DbContext context)
        {
            var connection = context.Database.GetDbConnection();
            var shouldCloseConnection = connection.State != ConnectionState.Open;

            if (shouldCloseConnection)
            {
                connection.Open();
            }

            try
            {
                using var pragma = connection.CreateCommand();
                pragma.CommandText = "PRAGMA table_info('Locations');";

                using var reader = pragma.ExecuteReader();
                var hasAddressColumn = false;

                while (reader.Read())
                {
                    var columnName = reader["name"]?.ToString();
                    if (string.Equals(columnName, "Address", StringComparison.OrdinalIgnoreCase))
                    {
                        hasAddressColumn = true;
                        break;
                    }
                }

                reader.Close();

                if (!hasAddressColumn)
                {
                    using var alter = connection.CreateCommand();
                    alter.CommandText = "ALTER TABLE Locations ADD COLUMN Address TEXT NOT NULL DEFAULT '';";
                    alter.ExecuteNonQuery();
                }
            }
            finally
            {
                if (shouldCloseConnection)
                {
                    connection.Close();
                }
            }
        }
    }
}
