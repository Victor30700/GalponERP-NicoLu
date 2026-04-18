using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalponERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPesoHistoricoToMovimientos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "StockActualKg",
                table: "Productos",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "StockActual",
                table: "Productos",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "PesoUnitarioKg",
                table: "Productos",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "Cantidad",
                table: "MovimientosInventario",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<decimal>(
                name: "PesoUnitarioHistorico",
                table: "MovimientosInventario",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            // Data Patch: Inicializar PesoUnitarioHistorico con el peso actual del producto (Sintaxis PostgreSQL)
            migrationBuilder.Sql(@"
                UPDATE ""MovimientosInventario""
                SET ""PesoUnitarioHistorico"" = p.""PesoUnitarioKg""
                FROM ""Productos"" p
                WHERE ""MovimientosInventario"".""ProductoId"" = p.""Id""
            ");

            // Data Patch: Corregir TipoUnidad en UnidadesMedida basado en abreviaturas comunes (Sintaxis PostgreSQL)
            migrationBuilder.Sql(@"
                UPDATE ""UnidadesMedida"" SET ""Tipo"" = 0 WHERE ""Abreviatura"" IN ('Kg', 'kg', 'Kilos');
                UPDATE ""UnidadesMedida"" SET ""Tipo"" = 2 WHERE ""Abreviatura"" IN ('Und', 'un', 'Unidad', 'Pza');
                UPDATE ""UnidadesMedida"" SET ""Tipo"" = 1 WHERE ""Abreviatura"" IN ('Lt', 'lt', 'Litro');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PesoUnitarioHistorico",
                table: "MovimientosInventario");

            migrationBuilder.AlterColumn<decimal>(
                name: "StockActualKg",
                table: "Productos",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,6)",
                oldPrecision: 18,
                oldScale: 6,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "StockActual",
                table: "Productos",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,6)",
                oldPrecision: 18,
                oldScale: 6,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "PesoUnitarioKg",
                table: "Productos",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,6)",
                oldPrecision: 18,
                oldScale: 6);

            migrationBuilder.AlterColumn<decimal>(
                name: "Cantidad",
                table: "MovimientosInventario",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,6)",
                oldPrecision: 18,
                oldScale: 6);
        }
    }
}
