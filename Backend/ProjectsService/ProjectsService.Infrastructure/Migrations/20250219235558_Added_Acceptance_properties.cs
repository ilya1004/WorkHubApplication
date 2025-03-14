using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectsService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Added_Acceptance_properties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AcceptanceConfirmed",
                table: "Lifecycles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AcceptanceRequested",
                table: "Lifecycles",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptanceConfirmed",
                table: "Lifecycles");

            migrationBuilder.DropColumn(
                name: "AcceptanceRequested",
                table: "Lifecycles");
        }
    }
}
