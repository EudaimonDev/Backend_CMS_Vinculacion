using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMSVinculacion.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RelacionNM_Auth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Categories_CategoryId",
                schema: "CON",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_CategoryId",
                schema: "CON",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                schema: "CON",
                table: "Articles");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                schema: "SEG",
                table: "Users",
                type: "varchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiry",
                schema: "SEG",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublicVisible",
                schema: "CAT",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ArticleCategory",
                schema: "CON",
                columns: table => new
                {
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleCategory", x => new { x.ArticleId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_ArticleCategory_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalSchema: "CON",
                        principalTable: "Articles",
                        principalColumn: "ArticleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleCategory_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "CAT",
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCategory_CategoryId",
                schema: "CON",
                table: "ArticleCategory",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleCategory",
                schema: "CON");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                schema: "SEG",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiry",
                schema: "SEG",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsPublicVisible",
                schema: "CAT",
                table: "Categories");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                schema: "CON",
                table: "Articles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CategoryId",
                schema: "CON",
                table: "Articles",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Categories_CategoryId",
                schema: "CON",
                table: "Articles",
                column: "CategoryId",
                principalSchema: "CAT",
                principalTable: "Categories",
                principalColumn: "CategoryId");
        }
    }
}
