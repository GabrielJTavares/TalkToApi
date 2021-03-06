using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TalkToApi.Migrations
{
    public partial class bancoToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RefreshToken = table.Column<string>(type: "TEXT", nullable: true),
                    UsuarioId = table.Column<string>(type: "TEXT", nullable: true),
                    Utilizado = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExpirtationToken = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpirtationRefreshToken = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Criado = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Atualizado = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tokens_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_UsuarioId",
                table: "Tokens",
                column: "UsuarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tokens");
        }
    }
}
