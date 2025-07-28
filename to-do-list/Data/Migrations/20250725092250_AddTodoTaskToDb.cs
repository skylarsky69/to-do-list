using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace to_do_list.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTodoTaskToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_UserId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Categories_CategoryId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Priorities_PriorityId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks");

            migrationBuilder.RenameTable(
                name: "Tasks",
                newName: "TodoTasks");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_UserId",
                table: "TodoTasks",
                newName: "IX_TodoTasks_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_PriorityId",
                table: "TodoTasks",
                newName: "IX_TodoTasks_PriorityId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_CategoryId",
                table: "TodoTasks",
                newName: "IX_TodoTasks_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TodoTasks",
                table: "TodoTasks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoTasks_AspNetUsers_UserId",
                table: "TodoTasks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TodoTasks_Categories_CategoryId",
                table: "TodoTasks",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TodoTasks_Priorities_PriorityId",
                table: "TodoTasks",
                column: "PriorityId",
                principalTable: "Priorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoTasks_AspNetUsers_UserId",
                table: "TodoTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoTasks_Categories_CategoryId",
                table: "TodoTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoTasks_Priorities_PriorityId",
                table: "TodoTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TodoTasks",
                table: "TodoTasks");

            migrationBuilder.RenameTable(
                name: "TodoTasks",
                newName: "Tasks");

            migrationBuilder.RenameIndex(
                name: "IX_TodoTasks_UserId",
                table: "Tasks",
                newName: "IX_Tasks_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TodoTasks_PriorityId",
                table: "Tasks",
                newName: "IX_Tasks_PriorityId");

            migrationBuilder.RenameIndex(
                name: "IX_TodoTasks_CategoryId",
                table: "Tasks",
                newName: "IX_Tasks_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AspNetUsers_UserId",
                table: "Tasks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Categories_CategoryId",
                table: "Tasks",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Priorities_PriorityId",
                table: "Tasks",
                column: "PriorityId",
                principalTable: "Priorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
