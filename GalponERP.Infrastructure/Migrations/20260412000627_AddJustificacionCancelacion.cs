using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalponERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJustificacionCancelacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JustificacionCancelacion",
                table: "Lotes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JustificacionCancelacion",
                table: "Lotes");
        }
    }
}
