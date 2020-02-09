using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestAPI.Migrations
{
    public partial class Krok2_Oceny : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Department",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Wydzialy",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(nullable: true),
                    IDParent = table.Column<int>(nullable: false),
                    Position = table.Column<int>(nullable: false),
                    IsBrygada = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wydzialy", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Kwalifikacje",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(nullable: true),
                    Opis = table.Column<string>(nullable: true),
                    Link = table.Column<string>(nullable: true),
                    WydzialID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kwalifikacje", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Kwalifikacje_Wydzialy_WydzialID",
                        column: x => x.WydzialID,
                        principalTable: "Wydzialy",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pracownicy",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NrPersonalny = table.Column<string>(nullable: true),
                    Nazwisko = table.Column<string>(nullable: true),
                    Imie = table.Column<string>(nullable: true),
                    WydzialID = table.Column<int>(nullable: false),
                    Firma = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pracownicy", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Pracownicy_Wydzialy_WydzialID",
                        column: x => x.WydzialID,
                        principalTable: "Wydzialy",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KwalifikacjeWydzialy",
                columns: table => new
                {
                    KwalifikacjaID = table.Column<int>(nullable: false),
                    WydzialID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KwalifikacjeWydzialy", x => new { x.KwalifikacjaID, x.WydzialID });
                    table.ForeignKey(
                        name: "FK_KwalifikacjeWydzialy_Kwalifikacje_KwalifikacjaID",
                        column: x => x.KwalifikacjaID,
                        principalTable: "Kwalifikacje",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KwalifikacjeWydzialy_Wydzialy_WydzialID",
                        column: x => x.WydzialID,
                        principalTable: "Wydzialy",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Oceny",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataOd = table.Column<DateTime>(nullable: false),
                    DataDo = table.Column<DateTime>(nullable: true),
                    OcenaV = table.Column<int>(nullable: false),
                    PracownikID = table.Column<int>(nullable: false),
                    KwalifikacjaID = table.Column<int>(nullable: false),
                    WprowadzajacyID = table.Column<string>(nullable: true),
                    StemelCzasu = table.Column<DateTime>(nullable: false),
                    Komentarz = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oceny", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Oceny_Kwalifikacje_KwalifikacjaID",
                        column: x => x.KwalifikacjaID,
                        principalTable: "Kwalifikacje",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Oceny_Pracownicy_PracownikID",
                        column: x => x.PracownikID,
                        principalTable: "Pracownicy",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Oceny_AspNetUsers_WprowadzajacyID",
                        column: x => x.WprowadzajacyID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kwalifikacje_WydzialID",
                table: "Kwalifikacje",
                column: "WydzialID");

            migrationBuilder.CreateIndex(
                name: "IX_KwalifikacjeWydzialy_WydzialID",
                table: "KwalifikacjeWydzialy",
                column: "WydzialID");

            migrationBuilder.CreateIndex(
                name: "IX_Oceny_KwalifikacjaID",
                table: "Oceny",
                column: "KwalifikacjaID");

            migrationBuilder.CreateIndex(
                name: "IX_Oceny_PracownikID",
                table: "Oceny",
                column: "PracownikID");

            migrationBuilder.CreateIndex(
                name: "IX_Oceny_WprowadzajacyID",
                table: "Oceny",
                column: "WprowadzajacyID");

            migrationBuilder.CreateIndex(
                name: "IX_Pracownicy_WydzialID",
                table: "Pracownicy",
                column: "WydzialID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KwalifikacjeWydzialy");

            migrationBuilder.DropTable(
                name: "Oceny");

            migrationBuilder.DropTable(
                name: "Kwalifikacje");

            migrationBuilder.DropTable(
                name: "Pracownicy");

            migrationBuilder.DropTable(
                name: "Wydzialy");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "AspNetUsers");
        }
    }
}
