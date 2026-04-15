using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalponERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNombreToLote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Lotes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Lotes");
        }
    }
}
