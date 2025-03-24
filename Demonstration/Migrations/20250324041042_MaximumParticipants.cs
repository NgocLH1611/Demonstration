using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demonstration.Migrations
{
    /// <inheritdoc />
    public partial class MaximumParticipants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaximumParticipants",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 10);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaximumParticipants",
                table: "Tasks");
        }
    }
}
