using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalponERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnsureMissingColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='Productos' AND column_name='PesoUnitarioKg') THEN
                        ALTER TABLE ""Productos"" ADD COLUMN ""PesoUnitarioKg"" numeric(18,4) NOT NULL DEFAULT 1.0;
                    END IF;
                END $$;
            ");

            migrationBuilder.AddColumn<decimal>(
                name: "StockActualKg",
                table: "Productos",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockActualKg",
                table: "Productos");
        }
    }
}
