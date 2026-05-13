using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMSVinculacion.Infrastructure.Migrations
{
    public partial class CategoriesImageUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_SCHEMA='CAT' AND TABLE_NAME='Categories' AND COLUMN_NAME='ImageUrl')
                    ALTER TABLE [CAT].[Categories] ADD [ImageUrl] nvarchar(500) NULL;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_SCHEMA='CAT' AND TABLE_NAME='Categories' AND COLUMN_NAME='ImageUrl')
                    ALTER TABLE [CAT].[Categories] DROP COLUMN [ImageUrl];
            ");
        }
    }
}