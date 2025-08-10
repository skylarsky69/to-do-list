using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace to_do_list.Data.Migrations
{
  
    public partial class AddColorToPriority : Migration
    {
       
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Priorities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Priorities");
        }
    }
}
