using Microsoft.EntityFrameworkCore.Migrations;

namespace TestAPI.Migrations
{
    public partial class wydzialyPoczatkowe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PoczatkoweWydzialy",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(nullable: true),
                    WydzialID = table.Column<int>(nullable: false),
                    Typ = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoczatkoweWydzialy", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PoczatkoweWydzialy_Wydzialy_WydzialID",
                        column: x => x.WydzialID,
                        principalTable: "Wydzialy",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PoczatkoweWydzialy_WydzialID",
                table: "PoczatkoweWydzialy",
                column: "WydzialID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PoczatkoweWydzialy");
        }
    }
}
