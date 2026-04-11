using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalponERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGalponIdToLote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GalponId",
                table: "Lotes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Lotes_GalponId",
                table: "Lotes",
                column: "GalponId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lotes_Galpones_GalponId",
                table: "Lotes",
                column: "GalponId",
                principalTable: "Galpones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lotes_Galpones_GalponId",
                table: "Lotes");

            migrationBuilder.DropIndex(
                name: "IX_Lotes_GalponId",
                table: "Lotes");

            migrationBuilder.DropColumn(
                name: "GalponId",
                table: "Lotes");
        }
    }
}
