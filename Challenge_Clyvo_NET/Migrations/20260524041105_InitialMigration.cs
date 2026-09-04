using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Challenge_Clyvo_NET.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_NET_PESSOA",
                columns: table => new
                {
                    ID_PESSOA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_PESSOA = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CPF = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DT_NASCIMENTO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_NET_PESSOA", x => x.ID_PESSOA);
                });

            migrationBuilder.CreateTable(
                name: "T_NET_CONTATO",
                columns: table => new
                {
                    ID_CONTATO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NUM_CONTATO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    EMAIL_CONTATO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ID_PESSOA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_NET_CONTATO", x => x.ID_CONTATO);
                    table.ForeignKey(
                        name: "FK_T_NET_CONTATO_T_NET_PESSOA_ID_PESSOA",
                        column: x => x.ID_PESSOA,
                        principalTable: "T_NET_PESSOA",
                        principalColumn: "ID_PESSOA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_NET_RESPONSAVEL",
                columns: table => new
                {
                    ID_RESPONSAVEL = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ENDERECO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ID_PESSOA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_NET_RESPONSAVEL", x => x.ID_RESPONSAVEL);
                    table.ForeignKey(
                        name: "FK_T_NET_RESPONSAVEL_T_NET_PESSOA_ID_PESSOA",
                        column: x => x.ID_PESSOA,
                        principalTable: "T_NET_PESSOA",
                        principalColumn: "ID_PESSOA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_NET_VETERINARIO",
                columns: table => new
                {
                    ID_VETERINARIO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ESPECIALIDADE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ID_PESSOA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_NET_VETERINARIO", x => x.ID_VETERINARIO);
                    table.ForeignKey(
                        name: "FK_T_NET_VETERINARIO_T_NET_PESSOA_ID_PESSOA",
                        column: x => x.ID_PESSOA,
                        principalTable: "T_NET_PESSOA",
                        principalColumn: "ID_PESSOA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_NET_ANIMAL",
                columns: table => new
                {
                    ID_ANIMAL = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_ANIMAL = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    IDADE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ESPECIE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    RACA = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    SEXO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DT_NASCIMENTO_ANIMAL = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    PESO = table.Column<double>(type: "BINARY_DOUBLE", precision: 4, scale: 2, nullable: false),
                    ID_RESPONSAVEL = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_NET_ANIMAL", x => x.ID_ANIMAL);
                    table.ForeignKey(
                        name: "FK_T_NET_ANIMAL_T_NET_RESPONSAVEL_ID_RESPONSAVEL",
                        column: x => x.ID_RESPONSAVEL,
                        principalTable: "T_NET_RESPONSAVEL",
                        principalColumn: "ID_RESPONSAVEL",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_NET_CONSULTA",
                columns: table => new
                {
                    ID_CONSULTA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DT_AGENDAMENTO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DT_CONSULTA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ID_ANIMAL = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_VETERINARIO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_NET_CONSULTA", x => x.ID_CONSULTA);
                    table.ForeignKey(
                        name: "FK_T_NET_CONSULTA_T_NET_ANIMAL_ID_ANIMAL",
                        column: x => x.ID_ANIMAL,
                        principalTable: "T_NET_ANIMAL",
                        principalColumn: "ID_ANIMAL",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_NET_CONSULTA_T_NET_VETERINARIO_ID_VETERINARIO",
                        column: x => x.ID_VETERINARIO,
                        principalTable: "T_NET_VETERINARIO",
                        principalColumn: "ID_VETERINARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_NET_ANIMAL_ID_RESPONSAVEL",
                table: "T_NET_ANIMAL",
                column: "ID_RESPONSAVEL");

            migrationBuilder.CreateIndex(
                name: "IX_T_NET_CONSULTA_ID_ANIMAL",
                table: "T_NET_CONSULTA",
                column: "ID_ANIMAL");

            migrationBuilder.CreateIndex(
                name: "IX_T_NET_CONSULTA_ID_VETERINARIO",
                table: "T_NET_CONSULTA",
                column: "ID_VETERINARIO");

            migrationBuilder.CreateIndex(
                name: "IX_T_NET_CONTATO_ID_PESSOA",
                table: "T_NET_CONTATO",
                column: "ID_PESSOA");

            migrationBuilder.CreateIndex(
                name: "IX_T_NET_RESPONSAVEL_ID_PESSOA",
                table: "T_NET_RESPONSAVEL",
                column: "ID_PESSOA");

            migrationBuilder.CreateIndex(
                name: "IX_T_NET_VETERINARIO_ID_PESSOA",
                table: "T_NET_VETERINARIO",
                column: "ID_PESSOA");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_NET_CONSULTA");

            migrationBuilder.DropTable(
                name: "T_NET_CONTATO");

            migrationBuilder.DropTable(
                name: "T_NET_ANIMAL");

            migrationBuilder.DropTable(
                name: "T_NET_VETERINARIO");

            migrationBuilder.DropTable(
                name: "T_NET_RESPONSAVEL");

            migrationBuilder.DropTable(
                name: "T_NET_PESSOA");
        }
    }
}
