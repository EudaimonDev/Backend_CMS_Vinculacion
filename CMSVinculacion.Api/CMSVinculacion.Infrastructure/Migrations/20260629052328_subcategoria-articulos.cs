using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMSVinculacion.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class subcategoriaarticulos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubCategoryId",
                schema: "CON",
                table: "Articles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_SubCategoryId",
                schema: "CON",
                table: "Articles",
                column: "SubCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_SubCategories_SubCategoryId",
                schema: "CON",
                table: "Articles",
                column: "SubCategoryId",
                principalSchema: "CAT",
                principalTable: "SubCategories",
                principalColumn: "SubCategoryId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_SubCategories_SubCategoryId",
                schema: "CON",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_SubCategoryId",
                schema: "CON",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                schema: "CON",
                table: "Articles");
        }
    }
}
