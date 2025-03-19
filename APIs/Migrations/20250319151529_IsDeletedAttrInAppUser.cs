using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIs.Migrations
{
    /// <inheritdoc />
    public partial class IsDeletedAttrInAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "38b291b2-0589-4952-a720-157634bab3e8");

            migrationBuilder.RenameColumn(
                name: "IsApproved",
                table: "AspNetUsers",
                newName: "IsDeleted");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "96374468-b9b3-4271-9734-fe645f894bd9");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "AspNetUsers",
                newName: "IsApproved");

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
    }
}
