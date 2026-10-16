using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Invora.FieldOps.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitAssignmentAndUniqueUsername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedUserId",
                table: "Visits",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Visits_AssignedUserId",
                table: "Visits",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Users_AssignedUserId",
                table: "Visits",
                column: "AssignedUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Users_AssignedUserId",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_Visits_AssignedUserId",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AssignedUserId",
                table: "Visits");
        }
    }
}
