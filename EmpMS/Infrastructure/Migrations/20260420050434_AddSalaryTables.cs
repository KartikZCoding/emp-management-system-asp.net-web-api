using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSalaryTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeId1",
                table: "LeaveRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Salaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    AnnualCTC = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Basic = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HRA = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DA = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TravelAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SpecialAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Bonus = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmployeePF = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProfessionalTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IncomeTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmployerPF = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Gratuity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalWorkingDays = table.Column<int>(type: "int", nullable: false),
                    PresentDays = table.Column<int>(type: "int", nullable: false),
                    PaidLeaveDays = table.Column<int>(type: "int", nullable: false),
                    UnpaidLeaveDays = table.Column<int>(type: "int", nullable: false),
                    HalfDays = table.Column<int>(type: "int", nullable: false),
                    AbsentDays = table.Column<int>(type: "int", nullable: false),
                    LopDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrossEarnings = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PayslipStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GeneratedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GeneratedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Salaries_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalaryStructures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComponentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ComponentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CalculationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryStructures", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SalaryStructures",
                columns: new[] { "Id", "CalculationType", "ComponentName", "ComponentType", "CreatedAt", "CreatedBy", "DisplayOrder", "IsActive", "MaxLimit", "UpdatedAt", "UpdatedBy", "Value" },
                values: new object[,]
                {
                    { 1, "PercentageOfCTC", "Basic Salary", "Earning", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", 1, true, null, null, null, 40m },
                    { 2, "PercentageOfBasic", "HRA", "Earning", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", 2, true, null, null, null, 50m },
                    { 3, "PercentageOfBasic", "Dearness Allowance", "Earning", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", 3, true, null, null, null, 10m },
                    { 4, "Fixed", "Travel Allowance", "Earning", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", 4, true, null, null, null, 1600m },
                    { 5, "Remaining", "Special Allowance", "Earning", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", 5, true, null, null, null, 0m },
                    { 6, "PercentageOfBasic", "Employee PF", "Deduction", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", 1, true, 1800m, null, null, 12m },
                    { 7, "Fixed", "Professional Tax", "Deduction", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", 2, true, 2500m, null, null, 200m },
                    { 8, "TaxSlab", "Income Tax", "Deduction", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", 3, true, null, null, null, 0m },
                    { 9, "PercentageOfBasic", "Employer PF", "EmployerContribution", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", 1, true, 1800m, null, null, 12m },
                    { 10, "PercentageOfBasic", "Gratuity", "EmployerContribution", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", 2, true, null, null, null, 4.81m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_EmployeeId1",
                table: "LeaveRequests",
                column: "EmployeeId1");

            migrationBuilder.CreateIndex(
                name: "IX_Salaries_Employee_Month_Year",
                table: "Salaries",
                columns: new[] { "EmployeeId", "Month", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalaryStructures_ComponentName",
                table: "SalaryStructures",
                column: "ComponentName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_Employees_EmployeeId1",
                table: "LeaveRequests",
                column: "EmployeeId1",
                principalTable: "Employees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_Employees_EmployeeId1",
                table: "LeaveRequests");

            migrationBuilder.DropTable(
                name: "Salaries");

            migrationBuilder.DropTable(
                name: "SalaryStructures");

            migrationBuilder.DropIndex(
                name: "IX_LeaveRequests_EmployeeId1",
                table: "LeaveRequests");

            migrationBuilder.DropColumn(
                name: "EmployeeId1",
                table: "LeaveRequests");
        }
    }
}
