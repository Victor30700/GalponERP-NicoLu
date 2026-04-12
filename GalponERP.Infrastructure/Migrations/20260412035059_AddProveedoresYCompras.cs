using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalponERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProveedoresYCompras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompraId",
                table: "MovimientosInventario",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RazonSocial = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NitRuc = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Direccion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioCreacionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioModificacionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComprasInventario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProveedorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EstadoPago = table.Column<int>(type: "integer", nullable: false, defaultValue: 2),
                    UsuarioIdRegistro = table.Column<Guid>(type: "uuid", nullable: false),
                    Nota = table.Column<string>(type: "text", nullable: true),
                    TotalCompra = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalPagado = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioCreacionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioModificacionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprasInventario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComprasInventario_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_CompraId",
                table: "MovimientosInventario",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasInventario_Fecha",
                table: "ComprasInventario",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasInventario_ProveedorId",
                table: "ComprasInventario",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_NitRuc",
                table: "Proveedores",
                column: "NitRuc",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosInventario_ComprasInventario_CompraId",
                table: "MovimientosInventario",
                column: "CompraId",
                principalTable: "ComprasInventario",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosInventario_ComprasInventario_CompraId",
                table: "MovimientosInventario");

            migrationBuilder.DropTable(
                name: "ComprasInventario");

            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.DropIndex(
                name: "IX_MovimientosInventario_CompraId",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "CompraId",
                table: "MovimientosInventario");
        }
    }
}
