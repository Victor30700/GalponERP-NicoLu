using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalponERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCalendarioSanitarioFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "UmbralMinimo",
                table: "Productos",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CostoTotal",
                table: "MovimientosInventario",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Proveedor",
                table: "MovimientosInventario",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsManual",
                table: "CalendarioSanitario",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Justificacion",
                table: "CalendarioSanitario",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "CalendarioSanitario",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Otros");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UmbralMinimo",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "CostoTotal",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "Proveedor",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "EsManual",
                table: "CalendarioSanitario");

            migrationBuilder.DropColumn(
                name: "Justificacion",
                table: "CalendarioSanitario");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "CalendarioSanitario");
        }
    }
}
