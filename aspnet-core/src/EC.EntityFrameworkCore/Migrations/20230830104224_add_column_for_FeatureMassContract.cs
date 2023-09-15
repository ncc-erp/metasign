using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EC.Migrations
{
    public partial class add_column_for_FeatureMassContract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MassType",
                table: "ContractTemplates",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<Guid>(
                name: "MassGuid",
                table: "Contracts",
                type: "uuid",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MassType",
                table: "ContractTemplates");

            migrationBuilder.DropColumn(
                name: "MassGuid",
                table: "Contracts");
        }
    }
}
