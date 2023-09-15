using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EC.Migrations
{
    public partial class add_tbl_MassContractTemplateSigner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MassContractTemplateSigners",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    ContractTemplateSignerId = table.Column<long>(type: "bigint", nullable: false),
                    SignerName = table.Column<string>(type: "text", nullable: true),
                    SignerEmail = table.Column<string>(type: "text", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MassContractTemplateSigners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MassContractTemplateSigners_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MassContractTemplateSigners_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MassContractTemplateSigners_ContractTemplateSigners_Contrac~",
                        column: x => x.ContractTemplateSignerId,
                        principalTable: "ContractTemplateSigners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MassContractTemplateSigners_ContractTemplateSignerId",
                table: "MassContractTemplateSigners",
                column: "ContractTemplateSignerId");

            migrationBuilder.CreateIndex(
                name: "IX_MassContractTemplateSigners_CreatorUserId",
                table: "MassContractTemplateSigners",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MassContractTemplateSigners_LastModifierUserId",
                table: "MassContractTemplateSigners",
                column: "LastModifierUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MassContractTemplateSigners");
        }
    }
}
