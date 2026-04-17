using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharavaniTours.Data.Migrations
{
    /// <inheritdoc />
    public partial class ClientUserUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientUser_Clients_ClientId",
                table: "ClientUser");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_ClientUser_ClientUserId",
                table: "Trips");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientUser",
                table: "ClientUser");

            migrationBuilder.RenameTable(
                name: "ClientUser",
                newName: "ClientUsers");

            migrationBuilder.RenameIndex(
                name: "IX_ClientUser_ClientId",
                table: "ClientUsers",
                newName: "IX_ClientUsers_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientUsers",
                table: "ClientUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientUsers_Clients_ClientId",
                table: "ClientUsers",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_ClientUsers_ClientUserId",
                table: "Trips",
                column: "ClientUserId",
                principalTable: "ClientUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientUsers_Clients_ClientId",
                table: "ClientUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_ClientUsers_ClientUserId",
                table: "Trips");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientUsers",
                table: "ClientUsers");

            migrationBuilder.RenameTable(
                name: "ClientUsers",
                newName: "ClientUser");

            migrationBuilder.RenameIndex(
                name: "IX_ClientUsers_ClientId",
                table: "ClientUser",
                newName: "IX_ClientUser_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientUser",
                table: "ClientUser",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientUser_Clients_ClientId",
                table: "ClientUser",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_ClientUser_ClientUserId",
                table: "Trips",
                column: "ClientUserId",
                principalTable: "ClientUser",
                principalColumn: "Id");
        }
    }
}
