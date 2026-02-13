using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TamilHoroscope.Web.Migrations
{
    /// <inheritdoc />
    public partial class MakeTrialDatesNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Make TrialStartDate nullable
            migrationBuilder.AlterColumn<DateTime>(
                name: "TrialStartDate",
                table: "Users",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            // Make TrialEndDate nullable
            migrationBuilder.AlterColumn<DateTime>(
                name: "TrialEndDate",
                table: "Users",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            // Add LastDailyFeeDeductionDate column (nullable)
            migrationBuilder.AddColumn<DateTime>(
                name: "LastDailyFeeDeductionDate",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove LastDailyFeeDeductionDate column
            migrationBuilder.DropColumn(
                name: "LastDailyFeeDeductionDate",
                table: "Users");

            // Revert TrialStartDate to NOT NULL with default
            migrationBuilder.AlterColumn<DateTime>(
                name: "TrialStartDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            // Revert TrialEndDate to NOT NULL
            migrationBuilder.AlterColumn<DateTime>(
                name: "TrialEndDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
