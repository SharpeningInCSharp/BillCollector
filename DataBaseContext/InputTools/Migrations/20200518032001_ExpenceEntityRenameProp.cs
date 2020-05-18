using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBaseContext.Migrations
{
    public partial class ExpenceEntityRenameProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentitiGuid",
                table: "Expence");

            migrationBuilder.AddColumn<Guid>(
                name: "IdentityGuid",
                table: "Expence",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityGuid",
                table: "Expence");

            migrationBuilder.AddColumn<Guid>(
                name: "IdentitiGuid",
                table: "Expence",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
