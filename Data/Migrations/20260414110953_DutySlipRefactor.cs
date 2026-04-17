using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharavaniTours.Data.Migrations
{
    /// <inheritdoc />
    public partial class DutySlipRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndKM",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ParkingCharges",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "StartKM",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "TollCharges",
                table: "Trips");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Trips",
                newName: "BookedDate");

            migrationBuilder.AddColumn<int>(
                name: "ClientUserId",
                table: "Trips",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DutySlips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TripId = table.Column<int>(type: "int", nullable: false),
                    StartKM = table.Column<int>(type: "int", nullable: false),
                    EndKM = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TollCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ParkingCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReportingTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReportingAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DutyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NextDayInstruction = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DutySlips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DutySlips_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trips_ClientUserId",
                table: "Trips",
                column: "ClientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DutySlips_TripId",
                table: "DutySlips",
                column: "TripId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_ClientUser_ClientUserId",
                table: "Trips",
                column: "ClientUserId",
                principalTable: "ClientUser",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_ClientUser_ClientUserId",
                table: "Trips");

            migrationBuilder.DropTable(
                name: "DutySlips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_ClientUserId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ClientUserId",
                table: "Trips");

            migrationBuilder.RenameColumn(
                name: "BookedDate",
                table: "Trips",
                newName: "StartTime");

            migrationBuilder.AddColumn<int>(
                name: "EndKM",
                table: "Trips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Trips",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "ParkingCharges",
                table: "Trips",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "StartKM",
                table: "Trips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TollCharges",
                table: "Trips",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
