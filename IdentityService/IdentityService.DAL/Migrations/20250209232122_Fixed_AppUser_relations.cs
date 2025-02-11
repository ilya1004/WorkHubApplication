using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Fixed_AppUser_relations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_EmployerProfiles_EmployerProfileId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_FreelancerProfiles_FreelancerProfileId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_EmployerProfileId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_FreelancerProfileId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmployerProfileId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FreelancerProfileId",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployerProfiles_Users_UserId",
                table: "EmployerProfiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FreelancerProfiles_Users_UserId",
                table: "FreelancerProfiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployerProfiles_Users_UserId",
                table: "EmployerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_FreelancerProfiles_Users_UserId",
                table: "FreelancerProfiles");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployerProfileId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FreelancerProfileId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmployerProfileId",
                table: "Users",
                column: "EmployerProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_FreelancerProfileId",
                table: "Users",
                column: "FreelancerProfileId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_EmployerProfiles_EmployerProfileId",
                table: "Users",
                column: "EmployerProfileId",
                principalTable: "EmployerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_FreelancerProfiles_FreelancerProfileId",
                table: "Users",
                column: "FreelancerProfileId",
                principalTable: "FreelancerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
