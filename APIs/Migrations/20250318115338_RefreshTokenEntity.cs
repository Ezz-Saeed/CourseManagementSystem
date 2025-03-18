using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIs.Migrations
{
    /// <inheritdoc />
    public partial class RefreshTokenEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "02d0f399-39b9-4af2-8bc1-d7fce382a99d");

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    AppuserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => new { x.AppuserId, x.Id });
                    table.ForeignKey(
                        name: "FK_RefreshToken_AspNetUsers_AppuserId",
                        column: x => x.AppuserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "role-admin",
                column: "ConcurrencyStamp",
                value: "3fab0538-2ca7-4698-99a8-322610c0c55a");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "38b291b2-0589-4952-a720-157634bab3e8", "5225dc7c-4f4a-4960-8575-4cdd11166e8a", "Trainer", "TRAINER" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-123",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e138fc6c-d5c5-4763-a245-07966c8b0474", "AQAAAAIAAYagAAAAEE59Okt/409KBvAA0AJGE3iixBYXg6riToMHtKNxgCQeS6B3InOizOILXiotHh4dEg==", "fdcb8f4e-6e88-4a39-9d18-bca4dd3d720b" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "38b291b2-0589-4952-a720-157634bab3e8");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "role-admin",
                column: "ConcurrencyStamp",
                value: "31dd28a8-cf72-4e75-87f5-c93701378a96");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "02d0f399-39b9-4af2-8bc1-d7fce382a99d", "aaf57290-c3a5-4121-a1bd-e0eba6bcc532", "Trainer", "TRAINER" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-123",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "74cca07f-c35d-42a8-80cd-9008a21871a1", "AQAAAAIAAYagAAAAEAsKQORrL2QfmmT/3J1b7w0U2ljXAb0jVXBBQJx6roOxjEsa5r02TQ3dtFAHXYyCoA==", "72daa356-1c1d-42af-bd6c-3974c75a2584" });
        }
    }
}
