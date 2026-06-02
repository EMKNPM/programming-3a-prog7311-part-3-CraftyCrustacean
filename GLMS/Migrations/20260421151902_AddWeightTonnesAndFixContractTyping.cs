using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GLMS.Migrations
{
    /// <inheritdoc />
    public partial class AddWeightTonnesAndFixContractTyping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contract",
                table: "ServiceRequests");

            migrationBuilder.AddColumn<decimal>(
                name: "WeightTonnes",
                table: "ServiceRequests",
                type: "decimal(10,3)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeightTonnes",
                table: "ServiceRequests");

            migrationBuilder.AddColumn<int>(
                name: "Contract",
                table: "ServiceRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
