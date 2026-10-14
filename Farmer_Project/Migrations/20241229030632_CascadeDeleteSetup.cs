using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmer_Project.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmersArticles_FarmersInfo_FarmersId",
                table: "FarmersArticles");

            migrationBuilder.AddForeignKey(
                name: "FK_FarmersArticles_FarmersInfo_FarmersId",
                table: "FarmersArticles",
                column: "FarmersId",
                principalTable: "FarmersInfo",
                principalColumn: "FarmersId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmersArticles_FarmersInfo_FarmersId",
                table: "FarmersArticles");

            migrationBuilder.AddForeignKey(
                name: "FK_FarmersArticles_FarmersInfo_FarmersId",
                table: "FarmersArticles",
                column: "FarmersId",
                principalTable: "FarmersInfo",
                principalColumn: "FarmersId");
        }
    }
}
