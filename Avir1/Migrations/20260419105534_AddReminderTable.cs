using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Avir1.Migrations
{
    /// <inheritdoc />
    public partial class AddReminderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reminder_AspNetUsers_UserID",
                table: "Reminder");

            migrationBuilder.DropIndex(
                name: "IX_Reminder_UserID",
                table: "Reminder");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Reminder");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Reminder",
                newName: "Email");

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodStartDate",
                table: "Reminder",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeriodStartDate",
                table: "Reminder");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Reminder",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Reminder",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Reminder_UserID",
                table: "Reminder",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Reminder_AspNetUsers_UserID",
                table: "Reminder",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
