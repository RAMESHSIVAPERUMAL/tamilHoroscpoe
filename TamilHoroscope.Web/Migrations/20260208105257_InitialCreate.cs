using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TamilHoroscope.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemConfig",
                columns: table => new
                {
                    ConfigId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfigKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Configuration key (unique)"),
                    ConfigValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, comment: "Configuration value"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "Description of the configuration parameter"),
                    DataType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "string", comment: "Data type: decimal, int, string, or bool"),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Date when the configuration was last modified"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "Whether the configuration is active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfig", x => x.ConfigId);
                    table.CheckConstraint("CK_SystemConfig_DataType", "[DataType] IN ('decimal', 'int', 'string', 'bool')");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsMobileVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrialStartDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    TrialEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsTrialActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "HoroscopeGenerations",
                columns: table => new
                {
                    GenerationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    GenerationDate = table.Column<DateTime>(type: "date", nullable: false, comment: "Date of generation for daily tracking"),
                    BirthDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Birth date and time for the horoscope"),
                    PlaceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true, comment: "Place name where the person was born"),
                    Latitude = table.Column<decimal>(type: "decimal(10,6)", nullable: false, comment: "Latitude of the birth location"),
                    Longitude = table.Column<decimal>(type: "decimal(10,6)", nullable: false, comment: "Longitude of the birth location"),
                    AmountDeducted = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0.00m, comment: "Amount deducted from wallet (0 for trial users)"),
                    WasTrialPeriod = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Whether this generation was during trial period"),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Date and time when the record was created")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoroscopeGenerations", x => x.GenerationId);
                    table.ForeignKey(
                        name: "FK_HoroscopeGenerations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    WalletId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0.00m, comment: "Current wallet balance in INR"),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.WalletId);
                    table.CheckConstraint("CK_Wallets_Balance", "[Balance] >= 0");
                    table.ForeignKey(
                        name: "FK_Wallets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WalletId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    BalanceBefore = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ReferenceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.CheckConstraint("CK_Transactions_Type", "[TransactionType] IN ('Credit', 'Debit', 'Refund')");
                    table.ForeignKey(
                        name: "FK_Transactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Transactions_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "WalletId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoroscopeGenerations_CreatedDateTime",
                table: "HoroscopeGenerations",
                column: "CreatedDateTime",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_HoroscopeGenerations_UserId_Date",
                table: "HoroscopeGenerations",
                columns: new[] { "UserId", "GenerationDate" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_SystemConfig_IsActive",
                table: "SystemConfig",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "UQ_SystemConfig_ConfigKey",
                table: "SystemConfig",
                column: "ConfigKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionType",
                table: "Transactions",
                columns: new[] { "TransactionType", "TransactionDate" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId_Date",
                table: "Transactions",
                columns: new[] { "UserId", "TransactionDate" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_WalletId_Date",
                table: "Transactions",
                columns: new[] { "WalletId", "TransactionDate" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_MobileNumber",
                table: "Users",
                column: "MobileNumber",
                unique: true,
                filter: "[MobileNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId_Balance",
                table: "Wallets",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HoroscopeGenerations");

            migrationBuilder.DropTable(
                name: "SystemConfig");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
