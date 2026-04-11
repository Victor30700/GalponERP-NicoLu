using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalponERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Ventas_Fecha",
                table: "Ventas",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_Fecha",
                table: "MovimientosInventario",
                column: "Fecha");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ventas_Fecha",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_MovimientosInventario_Fecha",
                table: "MovimientosInventario");
        }
    }
}
