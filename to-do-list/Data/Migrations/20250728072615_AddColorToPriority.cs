using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace to_do_list.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColorToPriority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Priorities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Priorities");
        }
    }
}
