using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMSVinculacion.Infrastructure.Migrations
{
    public partial class VisitorsEdadSexo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Agregar columnas nuevas si no existen
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_SCHEMA='GAT' AND TABLE_NAME='Visitors' AND COLUMN_NAME='Edad')
                    ALTER TABLE [GAT].[Visitors] ADD [Edad] int NOT NULL DEFAULT 0;

                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_SCHEMA='GAT' AND TABLE_NAME='Visitors' AND COLUMN_NAME='Sexo')
                    ALTER TABLE [GAT].[Visitors] ADD [Sexo] varchar(20) NOT NULL DEFAULT '';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_SCHEMA='GAT' AND TABLE_NAME='Visitors' AND COLUMN_NAME='Edad')
                    ALTER TABLE [GAT].[Visitors] DROP COLUMN [Edad];

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_SCHEMA='GAT' AND TABLE_NAME='Visitors' AND COLUMN_NAME='Sexo')
                    ALTER TABLE [GAT].[Visitors] DROP COLUMN [Sexo];
            ");
        }
    }
}