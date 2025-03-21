using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIs.Migrations
{
    /// <inheritdoc />
    public partial class Discriminator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4a413396-497a-4276-bd66-0164fdbfd668");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "role-admin",
                column: "ConcurrencyStamp",
                value: "b21578cd-6899-40d2-9a33-b2d32c86a38a");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "ba20318a-2c9e-4599-914d-41d313970479", "f9f40358-fe10-406e-9e37-61ebcbb6440b", "Trainer", "TRAINER" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-123",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f19aec9c-3017-4323-8d80-4cd5c98b5b9d", "AQAAAAIAAYagAAAAEJGAyqAPoe9+LXsRgmaAySQxFF5GuZrLWDx/UQNoLtlg+NMtoomOG/ZONEVA6vJJLw==", "71ebb5c2-5dc8-4122-89a4-98ee11f5f7d1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ba20318a-2c9e-4599-914d-41d313970479");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-123");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "role-admin",
                column: "ConcurrencyStamp",
                value: "ad903b01-5d5c-4fff-8903-68aa220c5511");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "4a413396-497a-4276-bd66-0164fdbfd668", "700b1016-e089-4a1b-a969-6ff496ac3a4c", "Trainer", "TRAINER" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "FirstName", "IsDeleted", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "admin-123", 0, "569b3a29-2789-4cbc-b294-c9d75ea61878", "Appuser", "admin@example.com", false, "Admin", false, "User", false, null, "ADMIN@EXAMPLE.COM", "ADMIN@EXAMPLE.COM", "AQAAAAIAAYagAAAAEO35eu81LMwLQsgQvK/+2wvn5pz7loWRVppRNs9L5CNwaPMpa0dPNkeTDSPyHkP0rg==", null, false, "a9306326-4a93-4a31-8c3d-7422eda0eedd", false, "admin@example.com" });
        }
    }
}
