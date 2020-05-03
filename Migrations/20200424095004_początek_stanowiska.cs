using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestAPI.Migrations
{
    public partial class początek_stanowiska : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 85, nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 85, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 85, nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 85, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 85, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    Department = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OcenaArchiwum",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DataOd = table.Column<DateTime>(nullable: false),
                    DataDo = table.Column<DateTime>(nullable: false),
                    OcenaV = table.Column<int>(nullable: false),
                    PracownikID = table.Column<int>(nullable: false),
                    Pracownik = table.Column<string>(nullable: true),
                    KwalifikacjaID = table.Column<int>(nullable: false),
                    Kwalifikacja = table.Column<string>(nullable: true),
                    WprowadzajacyID = table.Column<string>(nullable: true),
                    Wprowadzajacy = table.Column<string>(nullable: true),
                    StempelCzasu = table.Column<DateTime>(nullable: false),
                    Komentarz = table.Column<string>(nullable: true),
                    DataUsuniecia = table.Column<DateTime>(nullable: false),
                    UsuwajacyID = table.Column<string>(nullable: true),
                    UsuwajacyNazwa = table.Column<string>(nullable: true),
                    UsuniecieKomentarz = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OcenaArchiwum", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Stanowiska",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nazwa = table.Column<string>(nullable: true),
                    Opis = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Utworzono = table.Column<DateTime>(nullable: false),
                    Zmieniono = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stanowiska", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Wydzialy",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 85, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(maxLength: 85, nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 85, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(maxLength: 85, nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(maxLength: 85, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 85, nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(maxLength: 85, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 85, nullable: false),
                    RoleId = table.Column<string>(maxLength: 85, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 85, nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 85, nullable: false),
                    Name = table.Column<string>(maxLength: 85, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kwalifikacje",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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
                name: "PoczatkoweWydzialy",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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

            migrationBuilder.CreateTable(
                name: "Pracownicy",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NrPersonalny = table.Column<string>(nullable: true),
                    Nazwisko = table.Column<string>(nullable: true),
                    Imie = table.Column<string>(nullable: true),
                    WydzialID = table.Column<int>(nullable: false),
                    Firma = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    StanowiskoID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pracownicy", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Pracownicy_Stanowiska_StanowiskoID",
                        column: x => x.StanowiskoID,
                        principalTable: "Stanowiska",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pracownicy_Wydzialy_WydzialID",
                        column: x => x.WydzialID,
                        principalTable: "Wydzialy",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KwalifikacjeStanowiska",
                columns: table => new
                {
                    KwalifikacjaID = table.Column<int>(nullable: false),
                    StanowiskoID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KwalifikacjeStanowiska", x => new { x.KwalifikacjaID, x.StanowiskoID });
                    table.ForeignKey(
                        name: "FK_KwalifikacjeStanowiska_Kwalifikacje_KwalifikacjaID",
                        column: x => x.KwalifikacjaID,
                        principalTable: "Kwalifikacje",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KwalifikacjeStanowiska_Stanowiska_StanowiskoID",
                        column: x => x.StanowiskoID,
                        principalTable: "Stanowiska",
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
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DataOd = table.Column<DateTime>(nullable: false),
                    DataDo = table.Column<DateTime>(nullable: true),
                    OcenaV = table.Column<int>(nullable: false),
                    PracownikID = table.Column<int>(nullable: false),
                    KwalifikacjaID = table.Column<int>(nullable: false),
                    WprowadzajacyID = table.Column<string>(nullable: true),
                    StempelCzasu = table.Column<DateTime>(nullable: false),
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
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kwalifikacje_WydzialID",
                table: "Kwalifikacje",
                column: "WydzialID");

            migrationBuilder.CreateIndex(
                name: "IX_KwalifikacjeStanowiska_StanowiskoID",
                table: "KwalifikacjeStanowiska",
                column: "StanowiskoID");

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
                name: "IX_PoczatkoweWydzialy_WydzialID",
                table: "PoczatkoweWydzialy",
                column: "WydzialID");

            migrationBuilder.CreateIndex(
                name: "IX_Pracownicy_StanowiskoID",
                table: "Pracownicy",
                column: "StanowiskoID");

            migrationBuilder.CreateIndex(
                name: "IX_Pracownicy_WydzialID",
                table: "Pracownicy",
                column: "WydzialID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "KwalifikacjeStanowiska");

            migrationBuilder.DropTable(
                name: "KwalifikacjeWydzialy");

            migrationBuilder.DropTable(
                name: "OcenaArchiwum");

            migrationBuilder.DropTable(
                name: "Oceny");

            migrationBuilder.DropTable(
                name: "PoczatkoweWydzialy");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Kwalifikacje");

            migrationBuilder.DropTable(
                name: "Pracownicy");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Stanowiska");

            migrationBuilder.DropTable(
                name: "Wydzialy");
        }
    }
}
