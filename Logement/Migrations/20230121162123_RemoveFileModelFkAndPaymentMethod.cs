using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logement.Migrations
{
    public partial class RemoveFileModelFkAndPaymentMethod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartments_FileModel_TemplateContractId",
                table: "Apartments");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_TemplateContractId",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "TemplateContractId",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "paymentMethod",
                table: "Apartments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TemplateContractId",
                table: "Apartments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "paymentMethod",
                table: "Apartments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_TemplateContractId",
                table: "Apartments",
                column: "TemplateContractId");

            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_FileModel_TemplateContractId",
                table: "Apartments",
                column: "TemplateContractId",
                principalTable: "FileModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
