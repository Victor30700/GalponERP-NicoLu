using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalponERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantillasSanitarias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductoIdRecomendado",
                table: "CalendarioSanitario",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PlantillasSanitarias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioCreacionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioModificacionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantillasSanitarias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActividadesPlantillas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlantillaId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoActividad = table.Column<int>(type: "integer", nullable: false),
                    DiaDeAplicacion = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProductoIdRecomendado = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioCreacionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioModificacionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActividadesPlantillas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActividadesPlantillas_PlantillasSanitarias_PlantillaId",
                        column: x => x.PlantillaId,
                        principalTable: "PlantillasSanitarias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActividadesPlantillas_Productos_ProductoIdRecomendado",
                        column: x => x.ProductoIdRecomendado,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioSanitario_ProductoIdRecomendado",
                table: "CalendarioSanitario",
                column: "ProductoIdRecomendado");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesPlantillas_PlantillaId",
                table: "ActividadesPlantillas",
                column: "PlantillaId");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesPlantillas_ProductoIdRecomendado",
                table: "ActividadesPlantillas",
                column: "ProductoIdRecomendado");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarioSanitario_Productos_ProductoIdRecomendado",
                table: "CalendarioSanitario",
                column: "ProductoIdRecomendado",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarioSanitario_Productos_ProductoIdRecomendado",
                table: "CalendarioSanitario");

            migrationBuilder.DropTable(
                name: "ActividadesPlantillas");

            migrationBuilder.DropTable(
                name: "PlantillasSanitarias");

            migrationBuilder.DropIndex(
                name: "IX_CalendarioSanitario_ProductoIdRecomendado",
                table: "CalendarioSanitario");

            migrationBuilder.DropColumn(
                name: "ProductoIdRecomendado",
                table: "CalendarioSanitario");
        }
    }
}
