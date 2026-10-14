using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmer_Project.Migrations
{
    /// <inheritdoc />
    public partial class initCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FarmersInfo",
                columns: table => new
                {
                    FarmersId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FarmName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Image = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CropsType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PlantType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmersInfo", x => x.FarmersId);
                });

            migrationBuilder.CreateTable(
                name: "FarmersArticles",
                columns: table => new
                {
                    ArticlesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FarmersId = table.Column<int>(type: "int", nullable: true),
                    ArticleType = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ArticleTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ArticleImagePath = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ArticleSummary = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmersArticles", x => x.ArticlesId);
                    table.ForeignKey(
                        name: "FK_FarmersArticles_FarmersInfo_FarmersId",
                        column: x => x.FarmersId,
                        principalTable: "FarmersInfo",
                        principalColumn: "FarmersId");
                });

            migrationBuilder.CreateTable(
                name: "FarmersArticlesDetails",
                columns: table => new
                {
                    ArticlesId = table.Column<int>(type: "int", nullable: false),
                    DetailId = table.Column<int>(type: "int", nullable: false),
                    SubTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SubImagePath = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubContent = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmersArticlesDetails", x => new { x.ArticlesId, x.DetailId });
                    table.ForeignKey(
                        name: "FK_FarmersArticlesDetails_FarmersArticles_ArticlesId",
                        column: x => x.ArticlesId,
                        principalTable: "FarmersArticles",
                        principalColumn: "ArticlesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FarmersArticles_FarmersId",
                table: "FarmersArticles",
                column: "FarmersId");

            migrationBuilder.CreateIndex(
                name: "IX_FarmersInfo_Email",
                table: "FarmersInfo",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FarmersInfo_FarmName",
                table: "FarmersInfo",
                column: "FarmName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FarmersArticlesDetails");

            migrationBuilder.DropTable(
                name: "FarmersArticles");

            migrationBuilder.DropTable(
                name: "FarmersInfo");
        }
    }
}
