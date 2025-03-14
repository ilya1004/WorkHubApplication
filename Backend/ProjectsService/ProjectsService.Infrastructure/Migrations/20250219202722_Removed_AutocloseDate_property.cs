using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectsService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Removed_AutocloseDate_property : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoCloseDate",
                table: "Lifecycles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AutoCloseDate",
                table: "Lifecycles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
