using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logement.Migrations
{
    public partial class Remove_MaritalStatus_RentStatus_PayementMethod_Colunm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "Tenant");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Tenant");

            migrationBuilder.DropColumn(
                name: "RentStatus",
                table: "Tenant");

            migrationBuilder.DropColumn(
                name: "ApartmentStatus",
                table: "Apartment");

            migrationBuilder.DropColumn(
                name: "ApartmentType",
                table: "Apartment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaritalStatus",
                table: "Tenant",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Tenant",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RentStatus",
                table: "Tenant",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApartmentStatus",
                table: "Apartment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApartmentType",
                table: "Apartment",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
