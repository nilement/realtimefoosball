using Microsoft.EntityFrameworkCore.Migrations;

namespace ToughBattle.Migrations
{
    public partial class HasGroupStage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasGroupStage",
                table: "Tournaments",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasGroupStage",
                table: "Tournaments");
        }
    }
}
