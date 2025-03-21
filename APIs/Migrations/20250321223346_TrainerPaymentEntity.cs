using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIs.Migrations
{
    /// <inheritdoc />
    public partial class TrainerPaymentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ba20318a-2c9e-4599-914d-41d313970479");

            migrationBuilder.CreateTable(
                name: "TrainerPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrainerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainerPayments", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "role-admin",
                column: "ConcurrencyStamp",
                value: "9382af8b-d4e2-42d7-9101-bcc78973d134");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "dea00d73-3b9a-4926-b010-5ff4f59cdeab", "6bd1c35e-ea18-42c4-8c72-0a5660c880d5", "Trainer", "TRAINER" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-123",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "aba63831-e36d-4c73-94da-0a0632fe3148", "AQAAAAIAAYagAAAAEBm+qaBrLVwrUmbQCZ+LR4NOXQCW8U3Kwo3hQ+txE6EsR8Y/U1YZxm6z4s+vQgL7zw==", "ec27bb1b-bd4f-44b6-9c8a-9dddda390e6a" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainerPayments");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dea00d73-3b9a-4926-b010-5ff4f59cdeab");

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
    }
}
