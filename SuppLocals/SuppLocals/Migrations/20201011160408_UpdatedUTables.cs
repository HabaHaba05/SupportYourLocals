using Microsoft.EntityFrameworkCore.Migrations;

namespace SuppLocals.Migrations
{
    public partial class UpdatedUTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Users",
                newName: "Image");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Users",
                newName: "Content");
        }
    }
}
