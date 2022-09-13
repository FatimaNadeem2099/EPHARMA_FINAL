using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EPHARMA.Data.Migrations
{
    public partial class AddingDoctorWeeklyTimeSheetTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DoctorWeeklyTimeSheets",
                columns: table => new
                {
                    DoctorWeeklyTimeSheetId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorId = table.Column<int>(nullable: false),
                    WeekDay = table.Column<string>(nullable: true),
                    TimeRange = table.Column<string>(nullable: true),
                    Available = table.Column<bool>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorWeeklyTimeSheets", x => x.DoctorWeeklyTimeSheetId);
                    table.ForeignKey(
                        name: "FK_DoctorWeeklyTimeSheets_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "DoctorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorWeeklyTimeSheets_DoctorId",
                table: "DoctorWeeklyTimeSheets",
                column: "DoctorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoctorWeeklyTimeSheets");
        }
    }
}
