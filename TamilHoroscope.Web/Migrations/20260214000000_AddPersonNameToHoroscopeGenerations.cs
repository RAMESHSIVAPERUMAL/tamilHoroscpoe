using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TamilHoroscope.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonNameToHoroscopeGenerations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PersonName",
                table: "HoroscopeGenerations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Person name for whom the horoscope was generated");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonName",
                table: "HoroscopeGenerations");
        }
    }
}
