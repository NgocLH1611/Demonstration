using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demonstration.Migrations
{
    /// <inheritdoc />
    public partial class EnrolledParticipants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnrolledParticipants",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnrolledParticipants",
                table: "Tasks");
        }
    }
}
