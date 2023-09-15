using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EC.Migrations
{
    public partial class add_valueInput_to_ContractTemplateSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ValueInput",
                table: "ContractTemplateSettings",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValueInput",
                table: "ContractTemplateSettings");
        }
    }
}
