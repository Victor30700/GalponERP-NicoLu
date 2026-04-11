using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalponERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVentaToWeightBased : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PrecioUnitario",
                table: "Ventas",
                newName: "PrecioPorKilo");

            migrationBuilder.AddColumn<decimal>(
                name: "PesoTotalVendido",
                table: "Ventas",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PesoTotalVendido",
                table: "Ventas");

            migrationBuilder.RenameColumn(
                name: "PrecioPorKilo",
                table: "Ventas",
                newName: "PrecioUnitario");
        }
    }
}
