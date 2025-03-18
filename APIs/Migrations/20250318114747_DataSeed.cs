using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace APIs.Migrations
{
    /// <inheritdoc />
    public partial class DataSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_TrainerId",
                table: "Courses");

            migrationBuilder.AlterColumn<string>(
                name: "TrainerId",
                table: "Courses",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "02d0f399-39b9-4af2-8bc1-d7fce382a99d", "aaf57290-c3a5-4121-a1bd-e0eba6bcc532", "Trainer", "TRAINER" },
                    { "role-admin", "31dd28a8-cf72-4e75-87f5-c93701378a96", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "FirstName", "IsApproved", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "admin-123", 0, "74cca07f-c35d-42a8-80cd-9008a21871a1", "Appuser", "admin@example.com", false, "Admin", false, "User", false, null, "ADMIN@EXAMPLE.COM", "ADMIN@EXAMPLE.COM", "AQAAAAIAAYagAAAAEAsKQORrL2QfmmT/3J1b7w0U2ljXAb0jVXBBQJx6roOxjEsa5r02TQ3dtFAHXYyCoA==", null, false, "72daa356-1c1d-42af-bd6c-3974c75a2584", false, "admin@example.com" });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Description", "EndDate", "Name", "StartDate", "TrainerId" },
                values: new object[,]
                {
                    { 1, "Learn to build RESTful APIs using ASP.NET Core.", new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ASP.NET Core Web API", new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { 2, "A comprehensive course on Angular for building SPAs.", new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Angular Frontend Development", new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { 3, "A comprehensive course on SQL SERVER.", new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Database Management System", new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "role-admin", "admin-123" });

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_TrainerId",
                table: "Courses",
                column: "TrainerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_TrainerId",
                table: "Courses");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "02d0f399-39b9-4af2-8bc1-d7fce382a99d");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "role-admin", "admin-123" });

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "role-admin");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-123");

            migrationBuilder.AlterColumn<string>(
                name: "TrainerId",
                table: "Courses",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_TrainerId",
                table: "Courses",
                column: "TrainerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
