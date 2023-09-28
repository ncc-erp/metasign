using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EC.Migrations
{
    public partial class add_Columns_For_MassWordAutoFill_MassTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MassField",
                table: "ContractTemplates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MassWordContent",
                table: "ContractTemplates",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MassField",
                table: "ContractTemplates");

            migrationBuilder.DropColumn(
                name: "MassWordContent",
                table: "ContractTemplates");
        }
    }
}
