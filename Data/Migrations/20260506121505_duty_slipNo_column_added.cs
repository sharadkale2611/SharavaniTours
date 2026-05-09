using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharavaniTours.Data.Migrations
{
    /// <inheritdoc />
    public partial class duty_slipNo_column_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DutySlipNo",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DutySlipNo",
                table: "Trips");
        }
    }
}
