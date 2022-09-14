using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logement.Migrations
{
    public partial class AddImageURL_to_Apartment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "Apartments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "Apartments");
        }
    }
}
