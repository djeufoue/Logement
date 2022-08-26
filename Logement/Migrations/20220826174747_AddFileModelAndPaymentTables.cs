using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logement.Migrations
{
    public partial class AddFileModelAndPaymentTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartment_TenantRentStatus_TenantRentStatusId1",
                table: "Apartment");

            migrationBuilder.DropTable(
                name: "TenantRentStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tenant",
                table: "Tenant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Apartment",
                table: "Apartment");

            migrationBuilder.DropIndex(
                name: "IX_Apartment_TenantRentStatusId1",
                table: "Apartment");

            migrationBuilder.DropColumn(
                name: "Contract",
                table: "Apartment");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Apartment");

            migrationBuilder.DropColumn(
                name: "TenantRentStatusId1",
                table: "Apartment");

            migrationBuilder.RenameTable(
                name: "Tenant",
                newName: "Tenants");

            migrationBuilder.RenameTable(
                name: "Apartment",
                newName: "Apartments");

            migrationBuilder.RenameColumn(
                name: "TenantRentStatusId",
                table: "Apartments",
                newName: "TenantRentApartmentId");

            migrationBuilder.RenameColumn(
                name: "Area",
                table: "Apartments",
                newName: "TemplateContractId");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Apartments",
                type: "decimal(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "DepositePrice",
                table: "Apartments",
                type: "decimal(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "RoomArea",
                table: "Apartments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tenants",
                table: "Tenants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Apartments",
                table: "Apartments",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "FileModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantRentApartments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    NumberOfMonthsToPay = table.Column<int>(type: "int", nullable: false),
                    BailId = table.Column<int>(type: "int", nullable: false),
                    AmountRemainingForRent = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(14,2)", precision: 14, scale: 2, nullable: false),
                    DepositePrice = table.Column<decimal>(type: "decimal(14,2)", precision: 14, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantRentApartments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantRentApartments_FileModel_BailId",
                        column: x => x.BailId,
                        principalTable: "FileModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantRentApartments_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenantRentApartmentId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(14,2)", precision: 14, scale: 2, nullable: false),
                    DatePaid = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_TenantRentApartments_tenantRentApartmentId",
                        column: x => x.tenantRentApartmentId,
                        principalTable: "TenantRentApartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_TemplateContractId",
                table: "Apartments",
                column: "TemplateContractId");

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_TenantRentApartmentId",
                table: "Apartments",
                column: "TenantRentApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_tenantRentApartmentId",
                table: "Payments",
                column: "tenantRentApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantRentApartments_BailId",
                table: "TenantRentApartments",
                column: "BailId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantRentApartments_TenantId",
                table: "TenantRentApartments",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_FileModel_TemplateContractId",
                table: "Apartments",
                column: "TemplateContractId",
                principalTable: "FileModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_TenantRentApartments_TenantRentApartmentId",
                table: "Apartments",
                column: "TenantRentApartmentId",
                principalTable: "TenantRentApartments",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartments_FileModel_TemplateContractId",
                table: "Apartments");

            migrationBuilder.DropForeignKey(
                name: "FK_Apartments_TenantRentApartments_TenantRentApartmentId",
                table: "Apartments");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "TenantRentApartments");

            migrationBuilder.DropTable(
                name: "FileModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tenants",
                table: "Tenants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Apartments",
                table: "Apartments");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_TemplateContractId",
                table: "Apartments");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_TenantRentApartmentId",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "RoomArea",
                table: "Apartments");

            migrationBuilder.RenameTable(
                name: "Tenants",
                newName: "Tenant");

            migrationBuilder.RenameTable(
                name: "Apartments",
                newName: "Apartment");

            migrationBuilder.RenameColumn(
                name: "TenantRentApartmentId",
                table: "Apartment",
                newName: "TenantRentStatusId");

            migrationBuilder.RenameColumn(
                name: "TemplateContractId",
                table: "Apartment",
                newName: "Area");

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "Apartment",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(14,2)",
                oldPrecision: 14,
                oldScale: 2);

            migrationBuilder.AlterColumn<int>(
                name: "DepositePrice",
                table: "Apartment",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(14,2)",
                oldPrecision: 14,
                oldScale: 2);

            migrationBuilder.AddColumn<string>(
                name: "Contract",
                table: "Apartment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Apartment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "TenantRentStatusId1",
                table: "Apartment",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tenant",
                table: "Tenant",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Apartment",
                table: "Apartment",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TenantRentStatus",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId1 = table.Column<int>(type: "int", nullable: false),
                    AmountRemainingForRent = table.Column<int>(type: "int", nullable: false),
                    Bail = table.Column<int>(type: "int", nullable: false),
                    LastPayementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumberOfMonthsToPay = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantRentStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantRentStatus_Tenant_TenantId1",
                        column: x => x.TenantId1,
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Apartment_TenantRentStatusId1",
                table: "Apartment",
                column: "TenantRentStatusId1");

            migrationBuilder.CreateIndex(
                name: "IX_TenantRentStatus_TenantId1",
                table: "TenantRentStatus",
                column: "TenantId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Apartment_TenantRentStatus_TenantRentStatusId1",
                table: "Apartment",
                column: "TenantRentStatusId1",
                principalTable: "TenantRentStatus",
                principalColumn: "Id");
        }
    }
}
