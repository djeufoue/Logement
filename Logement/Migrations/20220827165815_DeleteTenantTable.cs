using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logement.Migrations
{
    public partial class DeleteTenantTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantRentApartments_Tenants_TenantId",
                table: "TenantRentApartments");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_TenantRentApartments_TenantId",
                table: "TenantRentApartments");

            migrationBuilder.AddColumn<long>(
                name: "TenantId1",
                table: "TenantRentApartments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MaritalStatus",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantFirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantLastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TenantRentApartments_TenantId1",
                table: "TenantRentApartments",
                column: "TenantId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantRentApartments_AspNetUsers_TenantId1",
                table: "TenantRentApartments",
                column: "TenantId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantRentApartments_AspNetUsers_TenantId1",
                table: "TenantRentApartments");

            migrationBuilder.DropIndex(
                name: "IX_TenantRentApartments_TenantId1",
                table: "TenantRentApartments");

            migrationBuilder.DropColumn(
                name: "TenantId1",
                table: "TenantRentApartments");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TenantFirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TenantLastName",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaritalStatus = table.Column<int>(type: "int", nullable: true),
                    TenantFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenantLastName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantRentApartments_TenantId",
                table: "TenantRentApartments",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantRentApartments_Tenants_TenantId",
                table: "TenantRentApartments",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
