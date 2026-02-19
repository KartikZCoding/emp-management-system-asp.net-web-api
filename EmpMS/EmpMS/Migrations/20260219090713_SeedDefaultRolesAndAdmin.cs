using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpMS.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultRolesAndAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "IsActive", "PasswordHash", "Username" },
                values: new object[] { 1, "Admin123@gmail.com", true, "$2a$11$3TiXqeZZ1dUHslmgkYVDUusIDqGmV3Yv/E7n2iAhNI46Gvq20aAFy", "Admin" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "RoleId", "UserId" },
                values: new object[] { 1, 1, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
