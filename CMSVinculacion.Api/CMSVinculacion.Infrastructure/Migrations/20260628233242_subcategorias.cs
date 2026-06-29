using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMSVinculacion.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class subcategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Estado",
                schema: "CAT",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                schema: "CAT",
                table: "Categories",
                type: "varchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubCategories",
                schema: "CAT",
                columns: table => new
                {
                    SubCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(307)", maxLength: 307, nullable: true),
                    CreatedIp = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "varchar(307)", maxLength: 307, nullable: true),
                    UpdatedIp = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "varchar(307)", maxLength: 307, nullable: true),
                    DeletedIp = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategories", x => x.SubCategoryId);
                    table.ForeignKey(
                        name: "FK_SubCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "CAT",
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubCategories_CategoryId_Slug",
                schema: "CAT",
                table: "SubCategories",
                columns: new[] { "CategoryId", "Slug" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubCategories",
                schema: "CAT");

            migrationBuilder.DropColumn(
                name: "Estado",
                schema: "CAT",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                schema: "CAT",
                table: "Categories");
        }
    }
}
