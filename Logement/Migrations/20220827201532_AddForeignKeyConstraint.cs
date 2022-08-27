using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logement.Migrations
{
    public partial class AddForeignKeyConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartments_TenantRentApartments_TenantRentApartmentId",
                table: "Apartments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_TenantRentApartments_tenantRentApartmentId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantRentApartments_AspNetUsers_TenantId1",
                table: "TenantRentApartments");

            migrationBuilder.DropIndex(
                name: "IX_TenantRentApartments_TenantId1",
                table: "TenantRentApartments");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_TenantRentApartmentId",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "TenantRentApartmentId",
                table: "Apartments");

            migrationBuilder.RenameColumn(
                name: "TenantId1",
                table: "TenantRentApartments",
                newName: "ApartmentId");

            migrationBuilder.RenameColumn(
                name: "tenantRentApartmentId",
                table: "Payments",
                newName: "TenantRentApartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_tenantRentApartmentId",
                table: "Payments",
                newName: "IX_Payments_TenantRentApartmentId");

            migrationBuilder.AlterColumn<long>(
                name: "TenantId",
                table: "TenantRentApartments",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "BailId",
                table: "TenantRentApartments",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "TenantRentApartments",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<long>(
                name: "TenantRentApartmentId",
                table: "Payments",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Payments",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<long>(
                name: "LessorId",
                table: "Payments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "FileModel",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<long>(
                name: "TemplateContractId",
                table: "Apartments",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Apartments",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<long>(
                name: "LessorId",
                table: "Apartments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_TenantRentApartments_ApartmentId",
                table: "TenantRentApartments",
                column: "ApartmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantRentApartments_TenantId",
                table: "TenantRentApartments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_LessorId",
                table: "Payments",
                column: "LessorId");

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_LessorId",
                table: "Apartments",
                column: "LessorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_AspNetUsers_LessorId",
                table: "Apartments",
                column: "LessorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_AspNetUsers_LessorId",
                table: "Payments",
                column: "LessorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_TenantRentApartments_TenantRentApartmentId",
                table: "Payments",
                column: "TenantRentApartmentId",
                principalTable: "TenantRentApartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantRentApartments_Apartments_ApartmentId",
                table: "TenantRentApartments",
                column: "ApartmentId",
                principalTable: "Apartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantRentApartments_AspNetUsers_TenantId",
                table: "TenantRentApartments",
                column: "TenantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartments_AspNetUsers_LessorId",
                table: "Apartments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_AspNetUsers_LessorId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_TenantRentApartments_TenantRentApartmentId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantRentApartments_Apartments_ApartmentId",
                table: "TenantRentApartments");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantRentApartments_AspNetUsers_TenantId",
                table: "TenantRentApartments");

            migrationBuilder.DropIndex(
                name: "IX_TenantRentApartments_ApartmentId",
                table: "TenantRentApartments");

            migrationBuilder.DropIndex(
                name: "IX_TenantRentApartments_TenantId",
                table: "TenantRentApartments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_LessorId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_LessorId",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "LessorId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "LessorId",
                table: "Apartments");

            migrationBuilder.RenameColumn(
                name: "ApartmentId",
                table: "TenantRentApartments",
                newName: "TenantId1");

            migrationBuilder.RenameColumn(
                name: "TenantRentApartmentId",
                table: "Payments",
                newName: "tenantRentApartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_TenantRentApartmentId",
                table: "Payments",
                newName: "IX_Payments_tenantRentApartmentId");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "TenantRentApartments",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "BailId",
                table: "TenantRentApartments",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TenantRentApartments",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "tenantRentApartmentId",
                table: "Payments",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Payments",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "FileModel",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "TemplateContractId",
                table: "Apartments",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Apartments",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "TenantRentApartmentId",
                table: "Apartments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantRentApartments_TenantId1",
                table: "TenantRentApartments",
                column: "TenantId1");

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_TenantRentApartmentId",
                table: "Apartments",
                column: "TenantRentApartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_TenantRentApartments_TenantRentApartmentId",
                table: "Apartments",
                column: "TenantRentApartmentId",
                principalTable: "TenantRentApartments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_TenantRentApartments_tenantRentApartmentId",
                table: "Payments",
                column: "tenantRentApartmentId",
                principalTable: "TenantRentApartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantRentApartments_AspNetUsers_TenantId1",
                table: "TenantRentApartments",
                column: "TenantId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
