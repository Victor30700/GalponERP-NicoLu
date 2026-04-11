using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalponERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIntegridadContableYPagos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstadoPago",
                table: "Ventas",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "CostoTotalFinal",
                table: "Lotes",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FCRFinal",
                table: "Lotes",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PorcentajeMortalidadFinal",
                table: "Lotes",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UtilidadNetaFinal",
                table: "Lotes",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoPago",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "CostoTotalFinal",
                table: "Lotes");

            migrationBuilder.DropColumn(
                name: "FCRFinal",
                table: "Lotes");

            migrationBuilder.DropColumn(
                name: "PorcentajeMortalidadFinal",
                table: "Lotes");

            migrationBuilder.DropColumn(
                name: "UtilidadNetaFinal",
                table: "Lotes");
        }
    }
}
