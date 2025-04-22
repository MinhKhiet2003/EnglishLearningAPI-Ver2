using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishLearningAPI.Migrations
{
    /// <inheritdoc />
    public partial class add_column_salt_user : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Salt",
                table: "user",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Salt",
                table: "user");
        }
    }
}
