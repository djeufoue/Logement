using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logement.Migrations
{
    public partial class AddPaymentHistoriesAndTenantPaymentStatusesTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "TenantRentApartmentViewModel");

            migrationBuilder.DropIndex(
                name: "IX_TenantRentApartments_BailId",
                table: "TenantRentApartments");

            migrationBuilder.DropColumn(
                name: "AmountPaidInAdvance",
                table: "TenantRentApartments");

            migrationBuilder.DropColumn(
                name: "AmountRemainingForRent",
                table: "TenantRentApartments");

            migrationBuilder.DropColumn(
                name: "NumberOfMonthsToPay",
                table: "TenantRentApartments");

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                table: "FileModel",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    NunberOfMonthPaid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentHistories_AspNetUsers_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantPaymentStatuses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    TenantRentApartmentId = table.Column<long>(type: "bigint", nullable: false),
                    NumberOfMonthsToPay = table.Column<int>(type: "int", nullable: false),
                    AmountRemainingForRent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RentStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantPaymentStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantPaymentStatuses_TenantRentApartments_TenantRentApartmentId",
                        column: x => x.TenantRentApartmentId,
                        principalTable: "TenantRentApartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantRentApartments_BailId",
                table: "TenantRentApartments",
                column: "BailId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentHistories_TenantId",
                table: "PaymentHistories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantPaymentStatuses_TenantRentApartmentId",
                table: "TenantPaymentStatuses",
                column: "TenantRentApartmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentHistories");

            migrationBuilder.DropTable(
                name: "TenantPaymentStatuses");

            migrationBuilder.DropIndex(
                name: "IX_TenantRentApartments_BailId",
                table: "TenantRentApartments");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "FileModel");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaidInAdvance",
                table: "TenantRentApartments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountRemainingForRent",
                table: "TenantRentApartments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfMonthsToPay",
                table: "TenantRentApartments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LessorId = table.Column<long>(type: "bigint", nullable: false),
                    TenantRentApartmentId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(14,2)", precision: 14, scale: 2, nullable: false),
                    DatePaid = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_AspNetUsers_LessorId",
                        column: x => x.LessorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_TenantRentApartments_TenantRentApartmentId",
                        column: x => x.TenantRentApartmentId,
                        principalTable: "TenantRentApartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantRentApartmentViewModel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AmountPaidByTenant = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountPaidInAdvance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountRemainingForRent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ApartmentId = table.Column<long>(type: "bigint", nullable: false),
                    BailId = table.Column<long>(type: "bigint", nullable: false),
                    Contract = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepositePrice = table.Column<decimal>(type: "decimal(14,2)", precision: 14, scale: 2, nullable: false),
                    EndOfContract = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NumberOfMonthsToPay = table.Column<int>(type: "int", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(14,2)", precision: 14, scale: 2, nullable: false),
                    StartOfContract = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantRentApartmentViewModel", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantRentApartments_BailId",
                table: "TenantRentApartments",
                column: "BailId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_LessorId",
                table: "Payments",
                column: "LessorId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TenantRentApartmentId",
                table: "Payments",
                column: "TenantRentApartmentId");
        }
    }
}
