using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestAPI.Migrations
{
    public partial class poczatkowe_stanowiska : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PoczatkoweStanowiska",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<string>(nullable: true),
                    StanowiskoID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoczatkoweStanowiska", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PoczatkoweStanowiska_Stanowiska_StanowiskoID",
                        column: x => x.StanowiskoID,
                        principalTable: "Stanowiska",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PoczatkoweStanowiska_StanowiskoID",
                table: "PoczatkoweStanowiska",
                column: "StanowiskoID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PoczatkoweStanowiska");
        }
    }
}
