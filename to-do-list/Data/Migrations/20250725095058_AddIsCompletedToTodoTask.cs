using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace to_do_list.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCompletedToTodoTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "TodoTasks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "TodoTasks");
        }
    }
}
