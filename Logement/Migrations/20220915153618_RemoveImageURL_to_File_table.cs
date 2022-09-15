using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logement.Migrations
{
    public partial class RemoveImageURL_to_File_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "Apartments");

            migrationBuilder.AlterColumn<long>(
                name: "Size",
                table: "FileModel",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "FileURL",
                table: "FileModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "ApartmentPhotos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileURL",
                table: "FileModel");

            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "ApartmentPhotos");

            migrationBuilder.AlterColumn<int>(
                name: "Size",
                table: "FileModel",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "Apartments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
