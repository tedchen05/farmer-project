using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmer_Project.Migrations
{
    /// <inheritdoc />
    public partial class TypeText_change : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SubContent",
                table: "FarmersArticlesDetails",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "ArticleSummary",
                table: "FarmersArticles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SubContent",
                table: "FarmersArticlesDetails",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ArticleSummary",
                table: "FarmersArticles",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
