using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Fixed_EmployerProfile_FK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_FreelancerProfiles_EmployerProfileId",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_EmployerProfiles_EmployerProfileId",
                table: "Users",
                column: "EmployerProfileId",
                principalTable: "EmployerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_EmployerProfiles_EmployerProfileId",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_FreelancerProfiles_EmployerProfileId",
                table: "Users",
                column: "EmployerProfileId",
                principalTable: "FreelancerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
