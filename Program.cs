using BuenosAiresExp.Services;
using BuenosAiresExp.Views;
using Microsoft.EntityFrameworkCore;

namespace BuenosAiresExp
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {


            // QuestPDF utilizado para gerar os PDFs dos roteiros. A licença Community é gratuita e adequada para projetos de código aberto ou uso pessoal.
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            // Garante que o banco de dados tenha o schema mais recente (incluindo Locations)
            using (var context = AppDbContextFactory.Create())
            {
                context.Database.Migrate();
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new HomeForm());
        }
    }
}
