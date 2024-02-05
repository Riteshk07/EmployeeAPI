using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeAPI.Provider.Migrations
{
    /// <inheritdoc />
    public partial class UpdateActiveCreateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TaskCreatedDateTime",
                table: "Todos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TaskUpdatedDateTime",
                table: "Todos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginDateTime",
                table: "Logins",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RecentActiveDateTime",
                table: "Employees",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UserCreatedDateTime",
                table: "Employees",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UserUpdatedDateTime",
                table: "Employees",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DepartmentCreatedDateTime",
                table: "Departments",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskCreatedDateTime",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "TaskUpdatedDateTime",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "LastLoginDateTime",
                table: "Logins");

            migrationBuilder.DropColumn(
                name: "RecentActiveDateTime",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "UserCreatedDateTime",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "UserUpdatedDateTime",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DepartmentCreatedDateTime",
                table: "Departments");
        }
    }
}
