using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIs.Migrations
{
    /// <inheritdoc />
    public partial class IsDeletedAttrInCourseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "96374468-b9b3-4271-9734-fe645f894bd9");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Courses",
                type: "bit",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-123",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "569b3a29-2789-4cbc-b294-c9d75ea61878", "AQAAAAIAAYagAAAAEO35eu81LMwLQsgQvK/+2wvn5pz7loWRVppRNs9L5CNwaPMpa0dPNkeTDSPyHkP0rg==", "a9306326-4a93-4a31-8c3d-7422eda0eedd" });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsDeleted",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4a413396-497a-4276-bd66-0164fdbfd668");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Courses");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "role-admin",
                column: "ConcurrencyStamp",
                value: "9780c074-0b68-4409-8afa-7a0496d0b9c4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "96374468-b9b3-4271-9734-fe645f894bd9", "9582a550-4eb3-4759-b5b0-27472a4f27e6", "Trainer", "TRAINER" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-123",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "78f071a4-7d75-4cd0-b670-ada83d33b140", "AQAAAAIAAYagAAAAEIR/CPF4ejNoXOzZPzYPL8xHG60dWwyEQGIsA0emXsKrD8ZZHh5K8gEmVR2XjZsMlA==", "11160fde-a7aa-4b21-b941-5c15c86f3449" });
        }
    }
}
