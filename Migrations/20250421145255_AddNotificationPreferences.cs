using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "emailnotificationsenabled",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "emailsubscriptionarn",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phonenumber",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "smsnotificationsenabled",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "smssubscriptionarn",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "email", "emailnotificationsenabled", "emailsubscriptionarn", "phonenumber", "smsnotificationsenabled", "smssubscriptionarn" },
                values: new object[] { null, false, null, null, false, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email",
                table: "users");

            migrationBuilder.DropColumn(
                name: "emailnotificationsenabled",
                table: "users");

            migrationBuilder.DropColumn(
                name: "emailsubscriptionarn",
                table: "users");

            migrationBuilder.DropColumn(
                name: "phonenumber",
                table: "users");

            migrationBuilder.DropColumn(
                name: "smsnotificationsenabled",
                table: "users");

            migrationBuilder.DropColumn(
                name: "smssubscriptionarn",
                table: "users");
        }
    }
}
