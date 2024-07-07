using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logement.Migrations
{
    public partial class AddTenancyTenancyMemberAndRenTermTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ApartmentAdderId",
                table: "Apartments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApartmentName",
                table: "Apartments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinimunTenancyPeriod",
                table: "Apartments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Tenancies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApartmentId = table.Column<long>(type: "bigint", nullable: false),
                    AdderId = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DateAdded = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LeaseStartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LeaseExpiryDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateLeaseTerminate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DateLeaseDeleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsLeaseDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenancies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tenancies_Apartments_ApartmentId",
                        column: x => x.ApartmentId,
                        principalTable: "Apartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tenancies_AspNetUsers_AdderId",
                        column: x => x.AdderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RentTerms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenancyId = table.Column<int>(type: "int", nullable: false),
                    RentStatus = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(14,2)", precision: 14, scale: 2, nullable: false),
                    IsFirstPaymemt = table.Column<bool>(type: "bit", nullable: false),
                    IsRentPayForThisMonth = table.Column<bool>(type: "bit", nullable: false),
                    DepositPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentTerms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentTerms_Tenancies_TenancyId",
                        column: x => x.TenancyId,
                        principalTable: "Tenancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenancyMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenancyId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AdderId = table.Column<long>(type: "bigint", nullable: false),
                    DateAdded = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateDelete = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Role = table.Column<int>(type: "int", nullable: false),
                    SendEmail = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenancyMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenancyMembers_AspNetUsers_AdderId",
                        column: x => x.AdderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TenancyMembers_AspNetUsers_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TenancyMembers_Tenancies_TenancyId",
                        column: x => x.TenancyId,
                        principalTable: "Tenancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_ApartmentAdderId",
                table: "Apartments",
                column: "ApartmentAdderId");

            migrationBuilder.CreateIndex(
                name: "IX_RentTerms_TenancyId",
                table: "RentTerms",
                column: "TenancyId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenancies_AdderId",
                table: "Tenancies",
                column: "AdderId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenancies_ApartmentId",
                table: "Tenancies",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TenancyMembers_AdderId",
                table: "TenancyMembers",
                column: "AdderId");

            migrationBuilder.CreateIndex(
                name: "IX_TenancyMembers_TenancyId",
                table: "TenancyMembers",
                column: "TenancyId");

            migrationBuilder.CreateIndex(
                name: "IX_TenancyMembers_TenantId",
                table: "TenancyMembers",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_AspNetUsers_ApartmentAdderId",
                table: "Apartments",
                column: "ApartmentAdderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartments_AspNetUsers_ApartmentAdderId",
                table: "Apartments");

            migrationBuilder.DropTable(
                name: "RentTerms");

            migrationBuilder.DropTable(
                name: "TenancyMembers");

            migrationBuilder.DropTable(
                name: "Tenancies");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_ApartmentAdderId",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "ApartmentAdderId",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "ApartmentName",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "MinimunTenancyPeriod",
                table: "Apartments");
        }
    }
}
