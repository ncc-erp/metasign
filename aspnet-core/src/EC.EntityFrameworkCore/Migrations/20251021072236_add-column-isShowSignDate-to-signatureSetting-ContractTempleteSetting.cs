using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EC.Migrations
{
    public partial class addcolumnisShowSignDatetosignatureSettingContractTempleteSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsShowSignDate",
                table: "SignerSignatureSettings",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsShowSignDate",
                table: "ContractTemplateSettings",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShowSignDate",
                table: "SignerSignatureSettings");

            migrationBuilder.DropColumn(
                name: "IsShowSignDate",
                table: "ContractTemplateSettings");
        }
    }
}
