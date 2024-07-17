using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace To_Do_List.Migrations
{
    public partial class NewFeatures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Tasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Tasks");
        }
    }
}
