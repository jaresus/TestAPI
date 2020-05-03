using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestAPI.Migrations
{
    public partial class szkolenie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SzkolenieCel",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WydzialID = table.Column<int>(nullable: false),
                    KwalifikacjaID = table.Column<int>(nullable: false),
                    Cel = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SzkolenieCel", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SzkolenieCel_Kwalifikacje_KwalifikacjaID",
                        column: x => x.KwalifikacjaID,
                        principalTable: "Kwalifikacje",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SzkolenieCel_Wydzialy_WydzialID",
                        column: x => x.WydzialID,
                        principalTable: "Wydzialy",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SzkolenieCel_KwalifikacjaID",
                table: "SzkolenieCel",
                column: "KwalifikacjaID");

            migrationBuilder.CreateIndex(
                name: "IX_SzkolenieCel_WydzialID",
                table: "SzkolenieCel",
                column: "WydzialID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SzkolenieCel");
        }
    }
}
