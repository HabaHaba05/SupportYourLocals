using Microsoft.EntityFrameworkCore.Migrations;

namespace SuppLocals.Migrations.VendorsDbTableMigrations
{
    public partial class UpdatedVendorsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "County",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Municipality",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "County",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Municipality",
                table: "Vendors");
        }
    }
}
