using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EC.Migrations
{
    public partial class add_signatureType_to_ContractSigning : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SignatureType",
                table: "ContractSigning",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignatureType",
                table: "ContractSigning");
        }
    }
}
