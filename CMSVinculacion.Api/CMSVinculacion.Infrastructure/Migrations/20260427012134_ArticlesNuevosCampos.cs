using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMSVinculacion.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ArticlesNuevosCampos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Emoji",
                schema: "CON",
                table: "Articles",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Excerpt",
                schema: "CON",
                table: "Articles",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Featured",
                schema: "CON",
                table: "Articles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReadingTime",
                schema: "CON",
                table: "Articles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Emoji",
                schema: "CON",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "Excerpt",
                schema: "CON",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "Featured",
                schema: "CON",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ReadingTime",
                schema: "CON",
                table: "Articles");
        }
    }
}
