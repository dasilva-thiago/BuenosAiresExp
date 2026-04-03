using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuenosAiresExp.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // No-op: o campo Address já foi adicionado na migration 20260321120000_AddAddressToLocation
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op para manter compatibilidade de histórico
        }
    }
}
