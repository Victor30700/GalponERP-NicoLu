using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalponERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAutomaticAuditing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Ventas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "Ventas",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioCreacionId",
                table: "Ventas",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioModificacionId",
                table: "Ventas",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioCreacionId",
                table: "Usuarios",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioModificacionId",
                table: "Usuarios",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Productos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "Productos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioCreacionId",
                table: "Productos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioModificacionId",
                table: "Productos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "PesajesLote",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "PesajesLote",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioCreacionId",
                table: "PesajesLote",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioModificacionId",
                table: "PesajesLote",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "MovimientosInventario",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "MovimientosInventario",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioCreacionId",
                table: "MovimientosInventario",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioModificacionId",
                table: "MovimientosInventario",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Mortalidades",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "Mortalidades",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioCreacionId",
                table: "Mortalidades",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioModificacionId",
                table: "Mortalidades",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Lotes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "Lotes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioCreacionId",
                table: "Lotes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioModificacionId",
                table: "Lotes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "GastosOperativos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "GastosOperativos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioCreacionId",
                table: "GastosOperativos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioModificacionId",
                table: "GastosOperativos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Galpones",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "Galpones",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioCreacionId",
                table: "Galpones",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioModificacionId",
                table: "Galpones",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Clientes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "Clientes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioCreacionId",
                table: "Clientes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioModificacionId",
                table: "Clientes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "CalendarioSanitario",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "CalendarioSanitario",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioCreacionId",
                table: "CalendarioSanitario",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioModificacionId",
                table: "CalendarioSanitario",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacionId",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacionId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacionId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "PesajesLote");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "PesajesLote");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "PesajesLote");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacionId",
                table: "PesajesLote");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacionId",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Mortalidades");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "Mortalidades");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "Mortalidades");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacionId",
                table: "Mortalidades");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Lotes");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "Lotes");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "Lotes");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacionId",
                table: "Lotes");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "GastosOperativos");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "GastosOperativos");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "GastosOperativos");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacionId",
                table: "GastosOperativos");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Galpones");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "Galpones");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "Galpones");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacionId",
                table: "Galpones");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacionId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "CalendarioSanitario");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "CalendarioSanitario");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "CalendarioSanitario");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacionId",
                table: "CalendarioSanitario");
        }
    }
}
