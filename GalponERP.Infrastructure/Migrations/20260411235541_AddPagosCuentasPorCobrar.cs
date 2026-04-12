using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalponERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPagosCuentasPorCobrar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EstadoPago",
                table: "Ventas",
                type: "integer",
                nullable: false,
                defaultValue: 2,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.CreateTable(
                name: "PagosVentas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VentaId = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MetodoPago = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioCreacionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioModificacionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosVentas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagosVentas_Ventas_VentaId",
                        column: x => x.VentaId,
                        principalTable: "Ventas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PagosVentas_FechaPago",
                table: "PagosVentas",
                column: "FechaPago");

            migrationBuilder.CreateIndex(
                name: "IX_PagosVentas_VentaId",
                table: "PagosVentas",
                column: "VentaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PagosVentas");

            migrationBuilder.AlterColumn<int>(
                name: "EstadoPago",
                table: "Ventas",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 2);
        }
    }
}
