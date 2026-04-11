using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalponERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogosDinamicos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Crear las nuevas tablas primero
            migrationBuilder.CreateTable(
                name: "CategoriasProductos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioCreacionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioModificacionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasProductos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnidadesMedida",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Abreviatura = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioCreacionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioModificacionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadesMedida", x => x.Id);
                });

            // 2. Añadir las nuevas columnas a Productos como NULLABLES inicialmente
            migrationBuilder.AddColumn<Guid>(
                name: "CategoriaProductoId",
                table: "Productos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EquivalenciaEnKg",
                table: "Productos",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 1m); // Default 1 para no romper cálculos

            migrationBuilder.AddColumn<Guid>(
                name: "UnidadMedidaId",
                table: "Productos",
                type: "uuid",
                nullable: true);

            // 3. SEEDING de Catálogos por defecto
            var catAlimentoId = Guid.NewGuid();
            var catMedicamentoId = Guid.NewGuid();
            var catInsumoId = Guid.NewGuid();
            var catOtroId = Guid.NewGuid();

            var unitKgId = Guid.NewGuid();
            var unitUnidadId = Guid.NewGuid();
            var unitLitroId = Guid.NewGuid();
            var unitSacoId = Guid.NewGuid();

            migrationBuilder.Sql($@"
                INSERT INTO ""CategoriasProductos"" (""Id"", ""Nombre"", ""IsActive"", ""FechaCreacion"") VALUES 
                ('{catAlimentoId}', 'Alimento', true, now()),
                ('{catMedicamentoId}', 'Medicamento', true, now()),
                ('{catInsumoId}', 'Insumo', true, now()),
                ('{catOtroId}', 'Otro', true, now());

                INSERT INTO ""UnidadesMedida"" (""Id"", ""Nombre"", ""Abreviatura"", ""IsActive"", ""FechaCreacion"") VALUES 
                ('{unitKgId}', 'Kilogramo', 'Kg', true, now()),
                ('{unitUnidadId}', 'Unidad', 'Und', true, now()),
                ('{unitLitroId}', 'Litro', 'L', true, now()),
                ('{unitSacoId}', 'Saco', 'Sc', true, now());
            ");

            // 4. MIGRACIÓN DE DATOS EXISTENTES
            // Mapear Tipo -> CategoriaProductoId
            migrationBuilder.Sql($@"
                UPDATE ""Productos"" SET ""CategoriaProductoId"" = '{catAlimentoId}' WHERE ""Tipo"" = 'Alimento';
                UPDATE ""Productos"" SET ""CategoriaProductoId"" = '{catMedicamentoId}' WHERE ""Tipo"" = 'Medicamento';
                UPDATE ""Productos"" SET ""CategoriaProductoId"" = '{catInsumoId}' WHERE ""Tipo"" = 'Insumo';
                UPDATE ""Productos"" SET ""CategoriaProductoId"" = '{catOtroId}' WHERE ""CategoriaProductoId"" IS NULL;
            ");

            // Mapear UnidadMedida -> UnidadMedidaId
            migrationBuilder.Sql($@"
                UPDATE ""Productos"" SET ""UnidadMedidaId"" = '{unitKgId}', ""EquivalenciaEnKg"" = 1.0 WHERE ""UnidadMedida"" = 'Kg';
                UPDATE ""Productos"" SET ""UnidadMedidaId"" = '{unitUnidadId}', ""EquivalenciaEnKg"" = 1.0 WHERE ""UnidadMedida"" = 'Unidad';
                UPDATE ""Productos"" SET ""UnidadMedidaId"" = '{unitLitroId}', ""EquivalenciaEnKg"" = 1.0 WHERE ""UnidadMedida"" = 'Litro';
                UPDATE ""Productos"" SET ""UnidadMedidaId"" = '{unitSacoId}', ""EquivalenciaEnKg"" = 40.0 WHERE ""UnidadMedida"" = 'Saco';
                UPDATE ""Productos"" SET ""UnidadMedidaId"" = '{unitUnidadId}' WHERE ""UnidadMedidaId"" IS NULL;
            ");

            // 5. Establecer columnas como NOT NULL y crear índices/FKs
            migrationBuilder.AlterColumn<Guid>(
                name: "CategoriaProductoId",
                table: "Productos",
                nullable: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "UnidadMedidaId",
                table: "Productos",
                nullable: false);

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "UnidadMedida",
                table: "Productos");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CategoriaProductoId",
                table: "Productos",
                column: "CategoriaProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_UnidadMedidaId",
                table: "Productos",
                column: "UnidadMedidaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_CategoriasProductos_CategoriaProductoId",
                table: "Productos",
                column: "CategoriaProductoId",
                principalTable: "CategoriasProductos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_UnidadesMedida_UnidadMedidaId",
                table: "Productos",
                column: "UnidadMedidaId",
                principalTable: "UnidadesMedida",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_CategoriasProductos_CategoriaProductoId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_UnidadesMedida_UnidadMedidaId",
                table: "Productos");

            migrationBuilder.DropTable(
                name: "CategoriasProductos");

            migrationBuilder.DropTable(
                name: "UnidadesMedida");

            migrationBuilder.DropIndex(
                name: "IX_Productos_CategoriaProductoId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_UnidadMedidaId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "CategoriaProductoId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "EquivalenciaEnKg",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "UnidadMedidaId",
                table: "Productos");

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Productos",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UnidadMedida",
                table: "Productos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
