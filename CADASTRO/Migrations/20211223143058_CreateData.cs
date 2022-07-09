using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CADASTRO.Migrations
{
    public partial class CreateData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DadosSensiveis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Cpf = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Endereco = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefone = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DadosSensiveis", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Lideres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lideres", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Partidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sigla = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NumeroPartido = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partidos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderKey = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LoginProvider = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DeputadosEstaduais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Estado = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nome = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DadosSensiveisId = table.Column<int>(type: "int", nullable: false),
                    PartidoId = table.Column<int>(type: "int", nullable: true),
                    Foto = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProjetosDeLei = table.Column<int>(type: "int", nullable: false),
                    Processo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeputadosEstaduais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeputadosEstaduais_DadosSensiveis_DadosSensiveisId",
                        column: x => x.DadosSensiveisId,
                        principalTable: "DadosSensiveis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeputadosEstaduais_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DeputadosFederais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Estado = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nome = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DadosSensiveisId = table.Column<int>(type: "int", nullable: false),
                    PartidoId = table.Column<int>(type: "int", nullable: true),
                    Foto = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProjetosDeLei = table.Column<int>(type: "int", nullable: false),
                    Processo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeputadosFederais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeputadosFederais_DadosSensiveis_DadosSensiveisId",
                        column: x => x.DadosSensiveisId,
                        principalTable: "DadosSensiveis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeputadosFederais_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Governadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Estado = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nome = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DadosSensiveisId = table.Column<int>(type: "int", nullable: false),
                    PartidoId = table.Column<int>(type: "int", nullable: true),
                    Foto = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProjetosDeLei = table.Column<int>(type: "int", nullable: false),
                    Processo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Governadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Governadores_DadosSensiveis_DadosSensiveisId",
                        column: x => x.DadosSensiveisId,
                        principalTable: "DadosSensiveis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Governadores_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MinistrosDeEstado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Pasta = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nome = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DadosSensiveisId = table.Column<int>(type: "int", nullable: false),
                    PartidoId = table.Column<int>(type: "int", nullable: true),
                    Foto = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProjetosDeLei = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinistrosDeEstado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MinistrosDeEstado_DadosSensiveis_DadosSensiveisId",
                        column: x => x.DadosSensiveisId,
                        principalTable: "DadosSensiveis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MinistrosDeEstado_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Prefeitos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Estado = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cidade = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nome = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DadosSensiveisId = table.Column<int>(type: "int", nullable: false),
                    PartidoId = table.Column<int>(type: "int", nullable: true),
                    Foto = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProjetosDeLei = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prefeitos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prefeitos_DadosSensiveis_DadosSensiveisId",
                        column: x => x.DadosSensiveisId,
                        principalTable: "DadosSensiveis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prefeitos_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Presidentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DadosSensiveisId = table.Column<int>(type: "int", nullable: false),
                    PartidoId = table.Column<int>(type: "int", nullable: true),
                    Foto = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProjetosDeLei = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presidentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Presidentes_DadosSensiveis_DadosSensiveisId",
                        column: x => x.DadosSensiveisId,
                        principalTable: "DadosSensiveis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Presidentes_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Senadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Estado = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nome = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DadosSensiveisId = table.Column<int>(type: "int", nullable: false),
                    PartidoId = table.Column<int>(type: "int", nullable: true),
                    Foto = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProjetosDeLei = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Senadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Senadores_DadosSensiveis_DadosSensiveisId",
                        column: x => x.DadosSensiveisId,
                        principalTable: "DadosSensiveis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Senadores_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Vereadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Estado = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cidade = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nome = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DadosSensiveisId = table.Column<int>(type: "int", nullable: false),
                    PartidoId = table.Column<int>(type: "int", nullable: true),
                    Foto = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProjetosDeLei = table.Column<int>(type: "int", nullable: false),
                    Processo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vereadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vereadores_DadosSensiveis_DadosSensiveisId",
                        column: x => x.DadosSensiveisId,
                        principalTable: "DadosSensiveis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vereadores_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "b0447b1c-5906-4d7a-9487-12b95ccfae00", "a02921b7-d714-4cc4-a9c5-69c6c1447f14", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "58f4c99a-fabd-4946-bc52-34416a856353", 0, "d3631804-984a-4046-972f-6cc71228b1e4", null, false, false, null, null, "ADMIN", "AQAAAAEAACcQAAAAENswB4IyRHdfCl+O5SPkO0S0rdM0JchwYmMvW8lpdw0+DUDVq6XY+NEZVR9IzHWyQA==", null, false, "08fa86bb-b244-4651-a128-8eb9694ffbcd", false, "Admin" });

            migrationBuilder.InsertData(
                table: "DadosSensiveis",
                columns: new[] { "Id", "Cpf", "Endereco", "Telefone" },
                values: new object[,]
                {
                    { 16, "11235789611", "Av. Y, nº 40, Belo Horizonte", "6132155964" },
                    { 15, "99658742312", "Av. Inventada 10, nº 22, Criciúma", "4853326964" },
                    { 14, "00215985512", "Praça Sem Nome s/nº, São Paulo", "1125229897" },
                    { 13, "14744522369", "Rua Inventada 10, nº 50, Taboão da Serra", "1122325247" },
                    { 12, "88563101270", "Rua Y, Brasília", "6120254511" },
                    { 11, "11236254987", "Rua x, Brasília", "6132574589" },
                    { 10, "96658821247", "Praça X, nº 11, Iracemápolis", "1745632857" },
                    { 1, "12345678900", "Rua Inventada 1, nº 39, São Paulo", "1133964628" },
                    { 8, "74119126965", "São Bernardo do Campo", "1133031156" },
                    { 7, "89636974158", "Brasília", "6133036390" },
                    { 6, "74114736965", "Av. João Baptista Parra, nº 673, Vitória", "2733031156" },
                    { 5, "85236974158", "Av. L. Viana Filho, nº 6462, Salvador", "7133036390" },
                    { 4, "98765432100", "Praça Sem Nome s/nº, Campo Grande", "6732156984" },
                    { 3, "98765412345", "Rua Inventada 2, nº 50, Maceió", "8298741235" },
                    { 2, "98765432100", "Praça Floriano s/nº, Rio de Janeiro", "2138142735" },
                    { 9, "26533296417", "Rua Inventada 5, nº 10, São Paranapanema", "1169325478" }
                });

            migrationBuilder.InsertData(
                table: "Lideres",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 8, "Wellington Roberto" },
                    { 7, "Efraim Filho" },
                    { 6, "Nivaldo Albuquerque" },
                    { 4, "Cacá Leão" },
                    { 3, "Rodrigo de Castro" },
                    { 2, "Bohn Gass" },
                    { 5, "Wolney Queiroz" },
                    { 1, "Isnaldo Bulhões Jr." }
                });

            migrationBuilder.InsertData(
                table: "Partidos",
                columns: new[] { "Id", "Nome", "NumeroPartido", "Sigla" },
                values: new object[,]
                {
                    { 1, "Movimento Democrático Brasileiro", 15, "MDB" },
                    { 2, "Partido dos Trabalhadores", 13, "PT" },
                    { 3, "Partido da Social Democracia Brasileira", 45, "PSDB" },
                    { 4, "Progressistas", 11, "PP" },
                    { 5, "Partido Democrático Trabalhista", 12, "PDT" },
                    { 6, "Partido Trabalhista Brasileiro", 14, "PTB" },
                    { 7, "Democratas", 25, "DEM" },
                    { 8, "Partido Liberal", 22, "PL" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "b0447b1c-5906-4d7a-9487-12b95ccfae00", "58f4c99a-fabd-4946-bc52-34416a856353" });

            migrationBuilder.InsertData(
                table: "DeputadosEstaduais",
                columns: new[] { "Id", "DadosSensiveisId", "Estado", "Foto", "Nome", "PartidoId", "Processo", "ProjetosDeLei" },
                values: new object[,]
                {
                    { 2, 14, "São Paulo", "CoronelTelhada.jpg", "Coronel Telhada", 4, false, 31 },
                    { 1, 13, "São Paulo", "AnaliceFernandes.jpg", "Analice Fernandes", 3, false, 29 }
                });

            migrationBuilder.InsertData(
                table: "DeputadosFederais",
                columns: new[] { "Id", "DadosSensiveisId", "Estado", "Foto", "Nome", "PartidoId", "Processo", "ProjetosDeLei" },
                values: new object[,]
                {
                    { 2, 16, "Minas Gerais", "AecioNeves.jpg", "Aécio Neves da Cunha", 3, true, 36 },
                    { 1, 15, "Santa Catarina", "GeovaniaDeSa.jpg", "Geovania de Sá", 3, false, 45 }
                });

            migrationBuilder.InsertData(
                table: "Governadores",
                columns: new[] { "Id", "DadosSensiveisId", "Estado", "Foto", "Nome", "PartidoId", "Processo", "ProjetosDeLei" },
                values: new object[,]
                {
                    { 2, 4, "Mato Grosso do Sul", "ReinaldoAzambuja.jpg", "Reinaldo Azambuja", 3, false, 26 },
                    { 1, 3, "Alagoas", "RenanFilho.jpg", "Renan Filho", 1, false, 11 }
                });

            migrationBuilder.InsertData(
                table: "MinistrosDeEstado",
                columns: new[] { "Id", "DadosSensiveisId", "Foto", "Nome", "PartidoId", "Pasta", "ProjetosDeLei" },
                values: new object[,]
                {
                    { 1, 11, "TerezaCristina.jpg", "Tereza Cristina", 7, "Agricultura, Pecuária e Abastecimento", 22 },
                    { 2, 12, "OnyxLorenzoni.jpg", "Onyx Lorenzoni", 7, "Trabalho e Previdência", 36 }
                });

            migrationBuilder.InsertData(
                table: "Prefeitos",
                columns: new[] { "Id", "Cidade", "DadosSensiveisId", "Estado", "Foto", "Nome", "PartidoId", "ProjetosDeLei" },
                values: new object[,]
                {
                    { 1, "Paranapanema", 9, "São Paulo", "RodolfoFanganiello.jpg", "Rodolfo Hessel Fanganiello", 4, 33 },
                    { 2, "Iracemápolis", 10, "São Paulo", "NelitaMichel.jpg", "Nelita Cristina Michel Franceschini", 8, 26 }
                });

            migrationBuilder.InsertData(
                table: "Presidentes",
                columns: new[] { "Id", "DadosSensiveisId", "Foto", "Nome", "PartidoId", "ProjetosDeLei" },
                values: new object[,]
                {
                    { 2, 8, "LulaDaSilva.jpg", "Luiz Inácio Lula da Silva", 2, 36 },
                    { 1, 7, "JairBolsonaro.jpg", "Jair Bolsonaro", 8, 35 }
                });

            migrationBuilder.InsertData(
                table: "Senadores",
                columns: new[] { "Id", "DadosSensiveisId", "Estado", "Foto", "Nome", "PartidoId", "ProjetosDeLei" },
                values: new object[,]
                {
                    { 1, 5, "Bahia", "JaquesWagner.jpg", "Jaques Wagner", 2, 23 },
                    { 2, 6, "Espírito Santo", "RoseDeFreitas.jpg", "Rose de Freitas", 1, 26 }
                });

            migrationBuilder.InsertData(
                table: "Vereadores",
                columns: new[] { "Id", "Cidade", "DadosSensiveisId", "Estado", "Foto", "Nome", "PartidoId", "Processo", "ProjetosDeLei" },
                values: new object[,]
                {
                    { 1, "São Paulo", 1, "São Paulo", "AdilsonAmadeu.jpg", "Adilson Amadeu", 7, true, 15 },
                    { 2, "Rio de Janeiro", 2, "Rio de Janeiro", "LindberghFarias.jpg", "Lindbergh Farias", 2, true, 26 }
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
                name: "IX_DeputadosEstaduais_DadosSensiveisId",
                table: "DeputadosEstaduais",
                column: "DadosSensiveisId");

            migrationBuilder.CreateIndex(
                name: "IX_DeputadosEstaduais_PartidoId",
                table: "DeputadosEstaduais",
                column: "PartidoId");

            migrationBuilder.CreateIndex(
                name: "IX_DeputadosFederais_DadosSensiveisId",
                table: "DeputadosFederais",
                column: "DadosSensiveisId");

            migrationBuilder.CreateIndex(
                name: "IX_DeputadosFederais_PartidoId",
                table: "DeputadosFederais",
                column: "PartidoId");

            migrationBuilder.CreateIndex(
                name: "IX_Governadores_DadosSensiveisId",
                table: "Governadores",
                column: "DadosSensiveisId");

            migrationBuilder.CreateIndex(
                name: "IX_Governadores_PartidoId",
                table: "Governadores",
                column: "PartidoId");

            migrationBuilder.CreateIndex(
                name: "IX_MinistrosDeEstado_DadosSensiveisId",
                table: "MinistrosDeEstado",
                column: "DadosSensiveisId");

            migrationBuilder.CreateIndex(
                name: "IX_MinistrosDeEstado_PartidoId",
                table: "MinistrosDeEstado",
                column: "PartidoId");

            migrationBuilder.CreateIndex(
                name: "IX_Prefeitos_DadosSensiveisId",
                table: "Prefeitos",
                column: "DadosSensiveisId");

            migrationBuilder.CreateIndex(
                name: "IX_Prefeitos_PartidoId",
                table: "Prefeitos",
                column: "PartidoId");

            migrationBuilder.CreateIndex(
                name: "IX_Presidentes_DadosSensiveisId",
                table: "Presidentes",
                column: "DadosSensiveisId");

            migrationBuilder.CreateIndex(
                name: "IX_Presidentes_PartidoId",
                table: "Presidentes",
                column: "PartidoId");

            migrationBuilder.CreateIndex(
                name: "IX_Senadores_DadosSensiveisId",
                table: "Senadores",
                column: "DadosSensiveisId");

            migrationBuilder.CreateIndex(
                name: "IX_Senadores_PartidoId",
                table: "Senadores",
                column: "PartidoId");

            migrationBuilder.CreateIndex(
                name: "IX_Vereadores_DadosSensiveisId",
                table: "Vereadores",
                column: "DadosSensiveisId");

            migrationBuilder.CreateIndex(
                name: "IX_Vereadores_PartidoId",
                table: "Vereadores",
                column: "PartidoId");
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
                name: "DeputadosEstaduais");

            migrationBuilder.DropTable(
                name: "DeputadosFederais");

            migrationBuilder.DropTable(
                name: "Governadores");

            migrationBuilder.DropTable(
                name: "Lideres");

            migrationBuilder.DropTable(
                name: "MinistrosDeEstado");

            migrationBuilder.DropTable(
                name: "Prefeitos");

            migrationBuilder.DropTable(
                name: "Presidentes");

            migrationBuilder.DropTable(
                name: "Senadores");

            migrationBuilder.DropTable(
                name: "Vereadores");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "DadosSensiveis");

            migrationBuilder.DropTable(
                name: "Partidos");
        }
    }
}
