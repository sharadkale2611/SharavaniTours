using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharavaniTours.Data.Migrations
{
    /// <inheritdoc />
    public partial class TripDutySlipBillingFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DutyType",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndKM",
                table: "Trips",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NextDayInstruction",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ParkingCharges",
                table: "Trips",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMode",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReportingAddress",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReportingTime",
                table: "Trips",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartKM",
                table: "Trips",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TollCharges",
                table: "Trips",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DutyType",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "EndKM",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "NextDayInstruction",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ParkingCharges",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "PaymentMode",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ReportingAddress",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ReportingTime",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "StartKM",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "TollCharges",
                table: "Trips");
        }
    }
}
