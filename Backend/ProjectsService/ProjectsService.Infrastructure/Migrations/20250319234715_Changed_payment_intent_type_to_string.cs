using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectsService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Changed_payment_intent_type_to_string : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Projects");

            migrationBuilder.AddColumn<string>(
                name: "PaymentIntentId",
                table: "Projects",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentIntentId",
                table: "Projects");

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentId",
                table: "Projects",
                type: "uuid",
                nullable: true);
        }
    }
}
