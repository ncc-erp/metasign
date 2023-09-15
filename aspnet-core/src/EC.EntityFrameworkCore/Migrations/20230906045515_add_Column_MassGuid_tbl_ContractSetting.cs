using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EC.Migrations
{
    public partial class add_Column_MassGuid_tbl_ContractSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SignerMassGuid",
                table: "ContractSettings",
                type: "uuid",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignerMassGuid",
                table: "ContractSettings");
        }
    }
}
