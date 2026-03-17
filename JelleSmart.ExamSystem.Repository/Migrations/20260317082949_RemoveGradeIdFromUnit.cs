using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JelleSmart.ExamSystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGradeIdFromUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Units_Grades_GradeId",
                table: "Units");

            migrationBuilder.DropIndex(
                name: "IX_Units_GradeId",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "GradeId",
                table: "Units");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GradeId",
                table: "Units",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Units_GradeId",
                table: "Units",
                column: "GradeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Units_Grades_GradeId",
                table: "Units",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
