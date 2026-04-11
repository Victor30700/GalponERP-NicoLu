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
            // 1. Añadir columna como nulable inicialmente
            migrationBuilder.AddColumn<Guid>(
                name: "GalponId",
                table: "Lotes",
                type: "uuid",
                nullable: true);

            // 2. Asegurar que existe al menos un galpón y obtener su ID
            // Si no hay galpones, insertamos uno genérico
            migrationBuilder.Sql(@"
                DO $$
                DECLARE
                    default_galpon_id uuid;
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM ""Galpones"") THEN
                        default_galpon_id := gen_random_uuid();
                        INSERT INTO ""Galpones"" (""Id"", ""Nombre"", ""Capacidad"", ""Ubicacion"", ""IsActive"", ""FechaCreacion"")
                        VALUES (default_galpon_id, 'Galpón por Defecto', 1000, 'Ubicación Inicial', true, now());
                    ELSE
                        SELECT ""Id"" INTO default_galpon_id FROM ""Galpones"" LIMIT 1;
                    END IF;

                    -- 3. Actualizar lotes existentes para que apunten al galpón encontrado/creado
                    UPDATE ""Lotes"" SET ""GalponId"" = default_galpon_id WHERE ""GalponId"" IS NULL;
                END $$;
            ");

            // 4. Cambiar a NOT NULL ahora que todos tienen valor
            migrationBuilder.AlterColumn<Guid>(
                name: "GalponId",
                table: "Lotes",
                type: "uuid",
                nullable: false);

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
