using Microsoft.EntityFrameworkCore.Migrations;

namespace EPHARMA.Data.Migrations
{
    public partial class AddingCommentToReviewTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "comment",
                table: "Reviews",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "comment",
                table: "Reviews");
        }
    }
}
