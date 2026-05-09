using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharavaniTours.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDutySlipNo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DutySlipNo",
                table: "DutySlips",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DutySlipNo",
                table: "DutySlips");
        }
    }
}
