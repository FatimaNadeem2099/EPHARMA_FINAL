using Microsoft.EntityFrameworkCore.Migrations;

namespace EPHARMA.Data.Migrations
{
    public partial class AddingAppointmentDayToAppointmentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppointmentDay",
                table: "Appointments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppointmentDay",
                table: "Appointments");
        }
    }
}
