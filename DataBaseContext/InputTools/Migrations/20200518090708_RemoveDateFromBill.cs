using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBaseContext.Migrations
{
    public partial class RemoveDateFromBill : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "Bills");

            migrationBuilder.AddColumn<int>(
                name: "BillEntityId",
                table: "Expence",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expence_BillEntityId",
                table: "Expence",
                column: "BillEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expence_Bills_BillEntityId",
                table: "Expence",
                column: "BillEntityId",
                principalTable: "Bills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expence_Bills_BillEntityId",
                table: "Expence");

            migrationBuilder.DropIndex(
                name: "IX_Expence_BillEntityId",
                table: "Expence");

            migrationBuilder.DropColumn(
                name: "BillEntityId",
                table: "Expence");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Bills",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
