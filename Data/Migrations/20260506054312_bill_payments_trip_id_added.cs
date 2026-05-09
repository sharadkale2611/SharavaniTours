using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharavaniTours.Data.Migrations
{
    /// <inheritdoc />
    public partial class bill_payments_trip_id_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TripId",
                table: "BillPayments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BillPayments_TripId",
                table: "BillPayments",
                column: "TripId");

            migrationBuilder.AddForeignKey(
                name: "FK_BillPayments_Trips_TripId",
                table: "BillPayments",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillPayments_Trips_TripId",
                table: "BillPayments");

            migrationBuilder.DropIndex(
                name: "IX_BillPayments_TripId",
                table: "BillPayments");

            migrationBuilder.DropColumn(
                name: "TripId",
                table: "BillPayments");
        }
    }
}
