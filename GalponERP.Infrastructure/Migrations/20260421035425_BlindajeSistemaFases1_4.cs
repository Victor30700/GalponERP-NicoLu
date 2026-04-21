using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalponERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BlindajeSistemaFases1_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LecturaMedidor",
                table: "RegistroBienestar",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PeriodoRetiroDias",
                table: "Productos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "InventarioLoteId",
                table: "MovimientosInventario",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFinRetiro",
                table: "Lotes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Tipo",
                table: "CategoriasProductos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "InventarioLotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CodigoLote = table.Column<string>(type: "text", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StockActual = table.Column<decimal>(type: "numeric", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioCreacionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioModificacionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioLotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventarioLotes_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_InventarioLoteId",
                table: "MovimientosInventario",
                column: "InventarioLoteId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioLotes_ProductoId",
                table: "InventarioLotes",
                column: "ProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosInventario_InventarioLotes_InventarioLoteId",
                table: "MovimientosInventario",
                column: "InventarioLoteId",
                principalTable: "InventarioLotes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosInventario_InventarioLotes_InventarioLoteId",
                table: "MovimientosInventario");

            migrationBuilder.DropTable(
                name: "InventarioLotes");

            migrationBuilder.DropIndex(
                name: "IX_MovimientosInventario_InventarioLoteId",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "LecturaMedidor",
                table: "RegistroBienestar");

            migrationBuilder.DropColumn(
                name: "PeriodoRetiroDias",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "InventarioLoteId",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "FechaFinRetiro",
                table: "Lotes");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "CategoriasProductos");
        }
    }
}
